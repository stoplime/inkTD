using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class GridVisualizer : MonoBehaviour {

    [Tooltip("The player ID that corresponds to the grid being visualized.")]
    public int gridID = 0;

    [Tooltip("The object that covers the individual grid blocks.")]
    public GameObject visualizerObject;

    private static List<GameObject> createdObjects = new List<GameObject>();

	// Use this for initialization
	void Start ()
    {
        PlayerManager.GetGrid(gridID).OnGridChange += GridVisualizer_OnGridChange;
	}

    private void GridVisualizer_OnGridChange(object sender, System.EventArgs e)
    {
        if (Application.isPlaying)
            ComputeBestPath();
    }

    private void ComputeBestPath()
    {
        List<IntVector2> path = PlayerManager.GetBestPath(gridID);
        
        for (int i = createdObjects.Count - 1; i >= 0; i--)
        {
            Destroy(createdObjects[i]);
        }
        createdObjects.Clear();

        for (int i = 0; i < path.Count; i++)
        {
            GameObject obj = Instantiate(visualizerObject);
            obj.transform.position = Grid.gridToPos(new IntVector2(path[i].x, path[i].y));
            createdObjects.Add(obj);
        }
    }

    public void OnClick()
    {
        ComputeBestPath();
    }
}
