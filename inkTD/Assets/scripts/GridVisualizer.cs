using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [Tooltip("The color of the visualizer's lines.")]
    public Color color = Color.white;

    [Tooltip("The ID of the grid to visualize.")]
    public int gridID = 0;

    private Grid grid;
    private int prevID = 0;
    private Vector3 offset;

	// Use this for initialization
	void Start ()
    {
        offset = new Vector3(Grid.gridSize / 2, 0, Grid.gridSize / 2);
        grid = PlayerManager.GetGrid(gridID);
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        if (prevID != gridID && gridID < PlayerManager.GridCount && gridID >= 0)
        {
            grid = PlayerManager.GetGrid(gridID);
        }

        prevID = gridID;

        Gizmos.color = color;
    }

    void OnValidate()
    {
        UpdateGrid();
    }

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            Gizmos.color = color;
            for (int i = 0; i <= grid.grid_width; i++)
            {
                Gizmos.DrawLine(grid.GetWorldPosFromLocal(i, 0) - offset, grid.GetWorldPosFromLocal(i, grid.grid_height) - offset);
            }
            for (int i = 0; i <= grid.grid_height; i++)
            {
                Gizmos.DrawLine(grid.GetWorldPosFromLocal(0, i) - offset, grid.GetWorldPosFromLocal(grid.grid_width, i) - offset);
            }
        }
    }

      // Update is called once per frame
 //   void Update ()
 //   {
		
	//}
}
