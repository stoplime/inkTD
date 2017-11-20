 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class PathVisualizer : MonoBehaviour {

    [Tooltip("The player ID that corresponds to the grid being visualized.")]
    public int gridID = 0;

    [Tooltip("The color the grid visualizer blocks take on the closer they get to the start.")]
    public Color startColor = Color.gray;

    [Tooltip("The color the grid visualizer blocks take on the closer they get to the end.")]
    public Color endColor = Color.gray;

    [Tooltip("The object that covers the individual grid blocks.")]
    public GameObject visualizerObject;

    /// <summary>
    /// Gets or sets whether the visualizer is visible.
    /// </summary>
    public bool Visible
    {
        get { return visible; }
        set { SetVisiblity(value); }
    }

    /// <summary>
    /// Gets or sets whether the path to visualize is manually set.
    /// </summary>
    public bool VisualizeManualPath
    {
        get { return visualizeManualPath; }
        set { visualizeManualPath = value; }
    }

    private List<GameObject> createdObjects = new List<GameObject>();
    private bool visible = true;
    private bool visualizeManualPath = false;
    private List<IntVector2> path = new List<IntVector2>();

	// Use this for initialization
	void Start ()
    {
        visualizerObject.transform.localScale = new Vector3(Grid.gridSize, visualizerObject.transform.localScale.y, Grid.gridSize);
        PlayerManager.GetGrid(gridID).OnGridChange += GridVisualizer_OnGridChange;
        if (path.Count == 0)
        {
            path = PlayerManager.GetBestPath(gridID);
        }
        VisualizePath();
    }

    void OnDestroy()
    {
        PlayerManager.GetGrid(gridID).OnGridChange -= GridVisualizer_OnGridChange;
        for (int i = createdObjects.Count - 1; i >= 0; i--)
        {
            Destroy(createdObjects[i]);
        }
    }

    /// <summary>
    /// toggles the visiblity of the grid visualizer.
    /// </summary>
    public void ToggleVisiblity()
    {
        SetVisiblity(!visible);
    }

    /// <summary>
    /// Forces the grid visualizer to manually visualize the given path.
    /// </summary>
    /// <param name="path"></param>
    public void SetPath(List<IntVector2> path)
    {
        this.path = path;
        visualizeManualPath = true;
        VisualizePath();
    }

    private void GridVisualizer_OnGridChange(object sender, System.EventArgs e)
    {
        if (Application.isPlaying && !visualizeManualPath)
        {
            path = PlayerManager.GetBestPath(gridID);
            VisualizePath();
        }
    }

    private void SetVisiblity(bool val)
    {
        visible = val;
        for (int i = 0; i < createdObjects.Count; i++)
        {
            createdObjects[i].SetActive(val);
        }
    }

    /// <summary>
    /// Visualizes a given path.
    /// </summary>
    /// <param name="path">The path to visualize.</param>
    private void VisualizePath()
    {
        if (createdObjects.Count > path.Count)
        {
            int start = createdObjects.Count - 1;
            for (int i = start; i >= path.Count; i--)
            {
                Destroy(createdObjects[i]);
            }
            createdObjects.RemoveRange(path.Count, start - path.Count + 1);
        }

        for (int i = 0; i < createdObjects.Count; i++)
        {
            if (createdObjects[i] != null)
            {
                createdObjects[i].transform.position = Grid.gridToPos(new IntVector2(path[i].x, path[i].y));
                // createdObjects[i].transform.position += new Vector3(0, Terrain.activeTerrain.SampleHeight(createdObjects[i].transform.position), 0);
                createdObjects[i].GetComponent<MeshRenderer>().material.color = Color.Lerp(endColor, startColor, (float)i / path.Count);
            }
        }
        for (int i = createdObjects.Count; i < path.Count; i++)
        {
            GameObject obj = Instantiate(visualizerObject);
            obj.transform.position = Grid.gridToPos(new IntVector2(path[i].x, path[i].y));
            obj.GetComponent<MeshRenderer>().material.color = Color.Lerp(endColor, startColor, (float)i / path.Count);
            obj.SetActive(visible);
            createdObjects.Add(obj);
        }
    }
}
