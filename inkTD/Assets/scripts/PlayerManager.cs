using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerManager {
	private static Dictionary<int, Grid> grids = new Dictionary<int, Grid>();
    private static Dictionary<int, List<IntVector2> > bestPaths = new Dictionary<int, List<IntVector2> >();

    /// <summary>
    /// Adds a grid for the given player id.
    /// </summary>
    /// <param name="playerID">The given player id.</param>
    /// <param name="grid">The grid to associate with the given player id.</param>
	public static void AddGrid(int playerID, Grid grid){
		grids.Add(playerID, grid);
        grid.OnGridChange += Grid_OnGridChange;
	}

    private static void Grid_OnGridChange(Grid grid, OnGridChangeEventArgs e)
    {
        if (bestPaths.ContainsKey(e.PlayerID))
        {
            List<IntVector2> bestPath = bestPaths[e.PlayerID];
            for (int i = 0; i < bestPath.Count; i++)
            {
                if (bestPath[i].x == e.XChanged && bestPath[i].y == e.YChanged)
                {
                    bestPaths[e.PlayerID] = Help.GetGridPath(e.PlayerID, new IntVector2(grid.startX, grid.startY), new IntVector2(grid.endX, grid.endY));
                    return;
                }
            }
        }
        else
        {
            bestPaths[e.PlayerID] = Help.GetGridPath(e.PlayerID, new IntVector2(grid.startX, grid.startY), new IntVector2(grid.endX, grid.endY));
        }
    }

    /// <summary>
    /// Gets the best path for the given player's grid.
    /// </summary>
    /// <param name="playerID">The id to determine whose grid's best path will be returned.</param>
    /// <returns>Returns the best path for the given player's grid.</returns>
    public static List<IntVector2> GetBestPath(int playerID)
    {
        return bestPaths[playerID];
    }

    /// <summary>
    /// Returns the grid associated with the given player id.
    /// </summary>
    /// <param name="playerID">The given player id.</param>
    /// <returns>Returns the grid associated with the given player id.</returns>
    public static Grid GetGrid(int playerID){
		return grids[playerID];
	}

    /// <summary>
    /// Sets a gameobject to the grid at the given x and y coordinates.
    /// </summary>
    /// <param name="playerID">The id of the player whose grid is being modified.</param>
    /// <param name="obj">The object to add to the grid.</param>
    /// <param name="x">The x coordinate on the grid to place the object.</param>
    /// <param name="y">The y coordinate on the grid to place the object.</param>
    public static void SetGameObject(int playerID, GameObject obj, int x, int y)
    {
        if (grids.ContainsKey(playerID))
            grids[playerID].setGridObject(x, y, obj);
    }

    public static void RemoveGameObject(int playerID, int x, int y)
    {
        if (grids.ContainsKey(playerID))
            grids[playerID].setGridObject(x, y, null);
    }
}
