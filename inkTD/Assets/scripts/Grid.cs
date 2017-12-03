using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;
using System;

[System.Serializable]
public class Grid : MonoBehaviour {

	public int ID;

	/// <summary>
	/// For code, use StartPosition instead.
	/// </summary>
    public int startX = 0;

	/// <summary>
	/// For code, use StartPosition instead.
	/// </summary>
    public int startY = 0;

	/// <summary>
	/// For code, use EndPosition instead.
	/// </summary>
    public int endX = 1;

	/// <summary>
	/// For code, use EndPosition instead.
	/// </summary>
    public int endY = 1;

    /// <summary>
    /// If true the objects placed in the grid will be offsetted by the terrain.
    /// </summary>
    public bool offsetByTerrain = false;
    
    /// <summary>
    /// The size of the individual grid tiles.
    /// </summary>
	public static float gridSize = 3;

	/// <summary>
	/// Takes an input Vector3 and converts it to the grid IntVector2
	/// </summary>
	/// <param name="input">World position</param>
	/// <returns></returns>
	public static IntVector2 posToGrid(Vector3 input){
		return new IntVector2((int)System.Math.Round(input.x/gridSize),
							(int)System.Math.Round(input.z/gridSize));
	}

    /// <summary>
    /// Takes an grid IntVector2 and converts it to the world pos Vector3
    /// </summary>
    /// <param name="input">grid coordinates</param>
    /// <returns></returns>
    public static Vector3 gridToPos(IntVector2 input)
    {
        Vector3 pos = new Vector3(input.x * gridSize, 0, input.y * gridSize);
        if (Terrain.activeTerrain != null)
            return new Vector3(pos.x, Terrain.activeTerrain.SampleHeight(pos), pos.z);
        return pos;
    }

    /// <summary>
	/// Takes in x and y grid coordinates and converts it to the world pos Vector3
	/// </summary>
    /// <param name="x">The horizontal grid tile to convert to world position.</param>
    /// <param name="y">The vertical grid tile to convert to world position.</param>
	/// <returns></returns>
	public static Vector3 gridToPos(int x, int y)
    {
        return new Vector3(x * gridSize, 0,
                        y * gridSize);
    }

    /// <summary>
    /// array containing the towers of the playing field
    /// </summary>
    private GameObject[,] grid;

    /// <summary>
    /// The tower castle assigned to this grid. Null if no tower castle exists for this grid.
    /// </summary>
    private GameObject towerCastle;

    /// <summary>
    /// The tower script associated with the tower castle.
    /// </summary>
    private Tower towerCastleScript;

    /// <summary>
    /// If the grid's tower castle has died.
    /// </summary>
    private bool isDead = false;

    public List<GameObject> GetAllGridObjects()
    {
        List<GameObject> allObjects = new List<GameObject>();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i,j] != null)
                {
                    allObjects.Add(grid[i,j]);
                }
            }
        }
        return allObjects;
    }

	public void setGridObject(IntVector2 xy, GameObject obj){
		grid[xy.x - gridOffset.x, xy.y - gridOffset.y] = obj;
        RunOnGridChange(xy.x, xy.y);
    }
	public void setGridObject(int x, int y, GameObject obj){
		grid[x - gridOffset.x, y - gridOffset.y] = obj;
        RunOnGridChange(x,y);
    }
	public GameObject getGridObject(IntVector2 xy){
		return grid[xy.x - gridOffset.x, xy.y - gridOffset.y];
	}
	public GameObject getGridObject(int x, int y){
		return grid[x - gridOffset.x, y - gridOffset.y];
	}

	/// <summery>
	/// offset the grid 0,0 per grid unit away
	/// </summery>
	public IntVector2 gridOffset;

	/// <summary>
	/// width and height of each playing field
	/// </summary>
	public int grid_width = 20;
	public int grid_height = 10;

    /// <summary>
    /// Gets the world position of the given tile coordinates by applying the grid's offset to them.
    /// </summary>
    /// <param name="x">The x tile to get the world coordinates of (note: 0 is the left of this grid).</param>
    /// <param name="y">The y tile to get the world coordinates of (note: 0 is the bottom of this grid).</param>
    /// <returns></returns>
    public Vector3 GetWorldPosFromLocal(int x, int y)
    {
        return Grid.gridToPos(x + gridOffset.x, y + gridOffset.y);
    }

    /// <summary>
    /// Returns the bottom left boundry of the grid in grid coordinates.
    /// </summary>
    /// <returns></returns>
    public IntVector2 GetBottomLeftBoundry()
    {
        return gridOffset;
    }

    /// <summary>
    /// Returns the top right boundry of the grid in grid coordinates.
    /// </summary>
    /// <returns></returns>
    public IntVector2 GetTopRightBoundry()
    {
        IntVector2 result = gridOffset;
        result.x += grid_width;
        result.y += grid_height;
        return result;
    }

	/// <summery>
	/// checks if grid pos is currently occupide or unplaceable aka not in arena
	/// </summery>
	public bool isGridEmpty(IntVector2 pos){
        if (!inArena(pos))
            return false;
		if (grid[pos.x - gridOffset.x, pos.y - gridOffset.y] != null)
			return false;
		return true;
	}

	/// <summery>
	/// checks if grid pos is within the grid arena
	/// </summery>
	public bool inArena(IntVector2 pos){
        if (pos.x >= gridOffset.x && pos.x < grid_width + gridOffset.x)
        {
            if (pos.y >= gridOffset.y && pos.y < grid_height + gridOffset.y)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a given grid position is within the grid's bounds.
    /// </summary>
    /// <param name="x">The horizontal x grid position.</param>
    /// <param name="y">The vertical y grid position.</param>
    /// <returns></returns>
    public bool inArena(int x, int y)
    {
        if (x >= gridOffset.x && x < grid_width + gridOffset.x)
        {
            if (y >= gridOffset.y && y < grid_height + gridOffset.y)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Damages the tower castle associated with this grid.
    /// </summary>
    /// <param name="damage">The amount being subtracted from the tower castle.</param>
    public void DamageTowerCastle(float damage)
    {
        if (towerCastleScript != null)
        {
            towerCastleScript.Health -= damage;

            //Tower castle died:
            if (towerCastleScript.Health <= 0 && !isDead)
            {
                isDead = true;
                if (OnTowerCastleDeath != null)
                {
                    OnTowerCastleDeath(this, EventArgs.Empty);
                }

                PlayerManager.SetPlayerDead(ID);
            }
        }
    }

	public int OffsetX = 0;
	public int OffsetY = 0;

	private void offsetEnds()
	{
		StartPosition = new IntVector2(startX, startY) + new IntVector2(OffsetX, OffsetY);
		EndPosition = new IntVector2(endX, endY) + new IntVector2(OffsetX, OffsetY);
	}

	void OnValidate()
	{
		offsetEnds();
	}

    void Awake() {
		gridOffset = new IntVector2(OffsetX, OffsetY);
        grid = new GameObject[grid_width, grid_height];
        // Moved the offsetEnds to here from the Start function
		offsetEnds();
    }

	// Use this for initialization
	void Start () {
        // For some reason the Start function does not get called
        // offsetEnds();
	}

	// Update is called once per frame
	void Update () {
        
	}

    public void ResetGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i,j] != null)
                {
                    Destroy(grid[i,j]);
                    grid[i,j] = null;
                }
            }
        }
    }

    /// <summary>
    /// Calls the OnGridChange event if it exists.
    /// </summary>
    private void RunOnGridChange(int x, int y) {
        if (OnGridChange != null)
            OnGridChange(this, new OnGridChangeEventArgs(ID,x ,y, x - gridOffset.x, y - gridOffset.y));
    }

    //Properties:

    /// <summary>
    /// Gets the start position for this grid.
    /// </summary>
    public IntVector2 StartPosition;

    /// <summary>
    /// Gets the end position for this grid.
    /// </summary>
    public IntVector2 EndPosition;

    /// <summary>
    /// Gets or sets the tower castle assigned to this grid. Note: This does not instantiate the tower castle, but it does force the tower castle's position to the end of the grid's path.
    /// </summary>
    public GameObject TowerCastle
    {
        get { return towerCastle; }
        set
        {
            towerCastle = value;
            towerCastleScript = towerCastle.GetComponent<Tower>();
            towerCastleScript.ownerID = ID;
            towerCastleScript.towerType = Towers.TowerCastle;
            towerCastleScript.SetGridPosition(endX + OffsetX, endY + OffsetY);
        }
    }

    /// <summary>
    /// Gets whether the tower castle in this grid is dead.
    /// </summary>
    public bool TowerCastleDead { get { return towerCastleScript.Health <= 0; } }

    //Events:

    public event OnGridChangeEventHandler OnGridChange;

    public event EventHandler OnTowerCastleDeath;
}
