using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerManager {
	private static Dictionary<int, Grid> grids = new Dictionary<int, Grid>();

	public static void AddGrid(int playerID, Grid grid){
		grids.Add(playerID, grid);
	}

	public static Grid GetGrid(int playerID){
		return grids[playerID];
	}
}
