using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class GridVisualizer : MonoBehaviour {

    [Tooltip("The player ID that corresponds to the grid being visualized.")]
    public int gridID = 0;

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

    private static List<GameObject> createdObjects = new List<GameObject>();
    private bool visible = true;

	// Use this for initialization
	void Start ()
    {
        PlayerManager.GetGrid(gridID).OnGridChange += GridVisualizer_OnGridChange;
        ComputeBestPath();
	}

    /// <summary>
    /// toggles the visiblity of the grid visualizer.
    /// </summary>
    public void ToggleVisiblity()
    {
        SetVisiblity(!visible);
    }

    private void GridVisualizer_OnGridChange(object sender, System.EventArgs e)
    {
        if (Application.isPlaying)
            ComputeBestPath();
    }

    private void SetVisiblity(bool val)
    {
        visible = val;
        for (int i = 0; i < createdObjects.Count; i++)
        {
            createdObjects[i].SetActive(val);
        }
    }

    private void ComputeBestPath()
    {
        List<IntVector2> path = PlayerManager.GetBestPath(gridID);
        
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
            createdObjects[i].transform.position = Grid.gridToPos(new IntVector2(path[i].x, path[i].y));
        }
        for (int i = createdObjects.Count; i < path.Count; i++)
        {
            GameObject obj = Instantiate(visualizerObject);
            obj.transform.position = Grid.gridToPos(new IntVector2(path[i].x, path[i].y));
            obj.SetActive(visible);
            createdObjects.Add(obj);
        }
    }

    public void OnClick()
    {
        ComputeBestPath();
    }
}
