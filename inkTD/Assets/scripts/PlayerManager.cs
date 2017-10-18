﻿using helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class that holds functions, variables, and objects related to players including their inkcome, balance, grids, best paths, and so on.
/// </summary>
public static class PlayerManager
{
    /// <summary>
    /// Gets the number of grids within the player manager.
    /// </summary>
    public static int GridCount { get { return grids.Count; } }

    /// <summary>
    /// The grids of the players.
    /// </summary>
	private static Dictionary<int, Grid> grids = new Dictionary<int, Grid>();

    /// <summary>
    /// The best path of the player's grid.
    /// </summary>
    private static Dictionary<int, List<IntVector2> > bestPaths = new Dictionary<int, List<IntVector2> >();

    /// <summary>
    /// The income (ink per income cycle) of players.
    /// </summary>
    private static Dictionary<int, float> income = new Dictionary<int, float>();

    /// <summary>
    /// The current ink balance of players.
    /// </summary>
    private static Dictionary<int, float> balance = new Dictionary<int, float>();

    /// <summary>
    /// The list of creatures in a grid of the given player id. IE: the key is the playerID of the grid where the creatures are located.
    /// </summary>
    private static Dictionary<int, List<Creature> > creatures = new Dictionary<int, List<Creature> >();

    /// <summary>
    /// The current player's playerID.
    /// </summary>
    public const int CurrentPlayer = 0;

    /// <summary>
    /// Meathod to add the creature into the creatures list.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="gridId"></param>
    /// <param name="creature"></param>
    public static void AddCreature(int playerID, int gridId, Creature creature)
    {
        creatures[gridId].Add(creature);
    }

    /// <summary>
    /// Returns the list of creatures present on a player's grid.
    /// </summary>
    /// <param name="playerID">The id of the player whose grid's creatures will be returned.</param>
    /// <returns></returns>
    public static List<Creature> GetCreatures(int playerID)
    {
        return creatures[playerID];
    }

    /// <summary>
    /// Returns the current ink incom of the given player.
    /// </summary>
    /// <param name="playerID">The given player's id whose income will be returned.</param>
    /// <returns></returns>
    public static float GetBalance(int playerID)
    {
        return balance[playerID];
    }

    /// <summary>
    /// Gets the income of the player.
    /// </summary>
    /// <param name="playerID">The id of the player.</param>
    /// <returns></returns>
    public static float GetIncome(int playerID)
    {
        return income[playerID];
    }

    /// <summary>
    /// Adds a player's income to their balance.
    /// </summary>
    /// <param name="playerID">The id of the player whose balance will rise by their income.</param>
    public static void ApplyIncome(int playerID)
    {
        balance[playerID] += income[playerID];

        if (playerID == CurrentPlayer && OnCurrentPlayerBalanceChange != null)
            OnCurrentPlayerBalanceChange(null, EventArgs.Empty);
    }

    /// <summary>
    /// Adds each player's income to their balance.
    /// </summary>
    public static void ApplyIncomeToAll()
    {
        int[] keys = new int[balance.Count];
        int index = 0;

        foreach (KeyValuePair<int, float> b in balance)
        {
            keys[index] = b.Key;
            index++;
        }

        for (int i = 0; i < keys.Length; i++)
        {
            balance[keys[i]] += income[keys[i]];

            if (keys[i] == CurrentPlayer && OnCurrentPlayerBalanceChange != null)
                OnCurrentPlayerBalanceChange(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Adds a certain amount to the given player's balance.
    /// </summary>
    /// <param name="playerID">The id of the player whose balance will be modified.</param>
    /// <param name="value">The amount to add to the balance.</param>
    public static void AddBalance(int playerID, float value)
    {
        balance[playerID] += value;

        if (playerID == CurrentPlayer && OnCurrentPlayerBalanceChange != null)
            OnCurrentPlayerBalanceChange(null, EventArgs.Empty);
    }

    /// <summary>
    /// Sets the balance (current ink) of the player whose id matches the given one.
    /// </summary>
    /// <param name="playerID">The id of the player</param>
    /// <param name="value">The amount to set the balance to.</param>
    public static void SetBalance(int playerID, float value)
    {
        balance[playerID] = value;

        if (playerID == CurrentPlayer && OnCurrentPlayerBalanceChange != null)
            OnCurrentPlayerBalanceChange(null, EventArgs.Empty);
    }

    /// <summary>
    /// Sets the income (current ink) of the player whose id matches the given one.
    /// </summary>
    /// <param name="playerID">The id of the player.</param>
    /// <param name="value">The amount to set the balance to.</param>
    public static void SetIncome(int playerID, float value)
    {
        income[playerID] = value;

        if (playerID == CurrentPlayer && OnCurrentPlayerIncomeChange != null)
            OnCurrentPlayerIncomeChange(null, EventArgs.Empty);
    }

    /// <summary>
    /// Adds a certain amount to a player's income.
    /// </summary>
    /// <param name="playerID">The id of the player whose income will be added to.</param>
    /// <param name="value">The amount of income to add.</param>
    public static void AddIncome(int playerID, float value)
    {
        income[playerID] += value;

        if (playerID == CurrentPlayer && OnCurrentPlayerIncomeChange != null)
            OnCurrentPlayerIncomeChange(null, EventArgs.Empty);
    }

    /// <summary>
    /// Adds a grid for the given player id.
    /// </summary>
    /// <param name="playerID">The given player id.</param>
    /// <param name="grid">The grid to associate with the given player id.</param>
	public static void AddGrid(int playerID, Grid grid){
		grids.Add(playerID, grid);
        creatures[playerID] = new List<Creature>(30);
        balance[playerID] = 0;
        income[playerID] = 50;

        grid.OnGridChange += Grid_OnGridChange;
	}

    /// <summary>
    /// Gets the number of creatures
    /// </summary>
    /// <returns></returns>
    public static int GetGridCount()
    {
        return grids.Count;
    }

    private static void Grid_OnGridChange(Grid grid, OnGridChangeEventArgs e)
    {
        if (bestPaths.ContainsKey(e.PlayerID))
        {
            List<IntVector2> bestPath = bestPaths[e.PlayerID];
            for (int i = 0; i < bestPath.Count; i++)
            {
                if (bestPath[i].x == e.XWorldChanged && bestPath[i].y == e.YWorldChanged)
                {
                    bestPaths[e.PlayerID] = Help.GetGridPath(e.PlayerID, grid.StartPosition, grid.EndPosition);
                    return;
                }
            }
            bestPaths[e.PlayerID] = Help.GetGridPath(e.PlayerID, grid.StartPosition, grid.EndPosition);
        }
        else
        {
            bestPaths[e.PlayerID] = Help.GetGridPath(e.PlayerID, grid.StartPosition, grid.EndPosition);
        }
    }

    /// <summary>
    /// Gets the best path for the given player's grid.
    /// </summary>
    /// <param name="playerID">The id to determine whose grid's best path will be returned.</param>
    /// <returns>Returns the best path for the given player's grid.</returns>
    public static List<IntVector2> GetBestPath(int playerID)
    {
        if (!bestPaths.ContainsKey(playerID))
        {
            Grid grid = grids[playerID];
            bestPaths[playerID] = Help.GetGridPath(playerID, grid.StartPosition, grid.EndPosition);
        }
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
        {
            IntVector2 boundryLower = grids[playerID].GetBottomLeftBoundry();
            IntVector2 boundryUpper = grids[playerID].GetTopRightBoundry();
            //Boundry checking.
            if (x < boundryLower.x)
                x = boundryLower.x;
            if (x > boundryUpper.x)
                x = boundryUpper.x;
            if (y < boundryLower.y)
                y = boundryLower.y;
            if (y > boundryUpper.y)
                y = boundryUpper.y;

            grids[playerID].setGridObject(x, y, obj);
        }
    }

    /// <summary>
    /// Removes the game object placed at x and y in the grid of the player whose id matches playerID.
    /// </summary>
    /// <param name="playerID">The id of the player whose grid will be checked.</param>
    /// <param name="x">The x position in grid coordinates/columns.</param>
    /// <param name="y">The y position in grid coordinates/rows.</param>
    public static void RemoveGameObject(int playerID, int x, int y)
    {
        if (grids.ContainsKey(playerID))
            grids[playerID].setGridObject(x, y, null);
    }

    /// <summary>
    /// Places a tower of the given prefab at the given location if the position is valid, and the player with the ID of playerID has enough ink in their balance.
    /// </summary>
    /// <param name="gridID">The ID of the grid where the tower will be placed.</param>
    /// <param name="playerID">The ID of the player whose ink will decrease for purchasing the tower.</param>
    /// <param name="gridPos">The position to place the tower.</param>
    /// <param name="orientation">The angle/rotation orientation of the tower.</param>
    /// <param name="towerPrefab">The name of the tower prefab.</param>
    /// <param name="notEnoughInkObject">The gameobject that appears when there is not enough ink.</param>
    /// <param name="notEnoughInkText">The text that is applied the the notEnoughInkObject.</param>
    /// <param name="textLife">The time the notEnoughInkObject stick around for (if applicable).</param>
    /// <returns></returns>
    public static bool PlaceTower(int gridID, int playerID, IntVector2 gridPos, Quaternion orientation, string towerPrefab, GameObject notEnoughInkObject, string notEnoughInkText, int textLife)
    {
        List<Creature> creatures = GetCreatures(playerID);
        Vector3 location = Grid.gridToPos(gridPos);
        GameObject newTower = GameObject.Instantiate(Resources.Load("Towers/" + towerPrefab), location, orientation) as GameObject; //Note: we can use an empty gameobject until we've confirmed the path is not obstructed.
        Tower ntScript = newTower.GetComponent<Tower>();
        ntScript.ownerID = playerID;
        ntScript.SetTowerPosition(gridPos);

        // Check if gridPos is a valid location for a tower to be placed
        bool pathFail = false;

        if (GetBestPath(gridID).Count == 0)
        {
            pathFail = true;
        }
        else
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].updateTempPath();
                if (!creatures[i].tempPathExists)
                {
                    pathFail = true;
                    break;
                }
            }
        }
        // print(pathFail);
        if (!pathFail)
        {
            if (GetBalance(playerID) >= ntScript.price)
            {
                AddBalance(playerID, -ntScript.price);

                for (int i = 0; i < creatures.Count; i++)
                {
                    creatures[i].updatePath();
                }
            }
            else //else... there wasn't enough ink.
            {
                pathFail = true;

                if (notEnoughInkObject != null)
                {
                    GameObject worldText = GameObject.Instantiate(notEnoughInkObject, location, orientation) as GameObject;
                    WorldText text = worldText.GetComponent<WorldText>();
                    if (text != null)
                    {
                        text.Text = notEnoughInkText;
                        text.Life = textLife;
                        text.cameraToFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                    }
                }
            }
        }

        if (pathFail)
        {
            GameObject.Destroy(newTower);
        }

        return !pathFail;
    }

    /// <summary>
    /// Creates a creature at the given player's grid whose id is playerID. Returns true if the creature was succesfully created, false otherwise.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="creaturePrefab"></param>
    /// <param name="spawner"></param>
    /// <returns></returns>
    public static bool CreateCreature(int playerID, string creaturePrefab, GameObject spawner)
    {
        GameObject prefab = Resources.Load("Creatures/" + creaturePrefab, typeof(GameObject)) as GameObject;
        Creature clone = prefab.GetComponent<Creature>();
        if (PlayerManager.GetBalance(playerID) >= clone.price)
		{
			PlayerManager.AddBalance(playerID, -clone.price);
		}
        else
        {
            return false;
        }
        GameObject creature;
        AddIncome(playerID, clone.inkcomeValue);
        foreach (KeyValuePair<int, List<Creature>> v in creatures)
        {
            if (playerID != v.Key)
            {
                creature = MonoBehaviour.Instantiate(prefab) as GameObject;
                clone = creature.GetComponent<Creature>();
                clone.gridID = v.Key;
                clone.ownerID = playerID;
                AddCreature(playerID, v.Key, clone);
            }
        }
        return true;
    }

    /// <summary>
    /// An event that runs when the current player's balance of ink changes.
    /// </summary>
    public static event EventHandler OnCurrentPlayerBalanceChange;

    /// <summary>
    /// An event that runs when the current player's income of ink changes.
    /// </summary>
    public static event EventHandler OnCurrentPlayerIncomeChange;

    

    // public static bool 
}
