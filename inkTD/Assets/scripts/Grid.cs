using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;
using System;

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
	/// Takes an grid IntVector2 and converts it to the pos Vector3
	/// </summary>
	/// <param name="input">grid coordinates</param>
	/// <returns></returns>
	public static Vector3 gridToPos(IntVector2 input){
		return new Vector3(input.x*gridSize, 0,
						input.y*gridSize);
	}

	/// <summary>
	/// array containing the towers of the playing field
	/// </summary>
	private GameObject[,] grid;

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

	/// <summery>
	/// checks if grid pos is currently occupide with a tower
	/// </summery>
	public bool isGridEmpty(IntVector2 pos){
		if (grid[pos.x - gridOffset.x, pos.y - gridOffset.y] != null){
			return false;
		}
		return true;
	}

	/// <summery>
	/// checks if grid pos is within the grid arena
	/// </summery>
	public bool inArena(IntVector2 pos){
		if (pos.x >= gridOffset.x && pos.x < grid_width+gridOffset.x){
			if (pos.y >= gridOffset.y && pos.y < grid_height+gridOffset.y){
				return true;
			}
		}
		return false;
	}
	public int OffsetX;
	public int OffsetY;

    private RaycastHit hit;

	void OnValidate()
	{
		StartPosition = new IntVector2(startX, startY) + new IntVector2(OffsetX, OffsetY);
		EndPosition = new IntVector2(endX, endY) + new IntVector2(OffsetX, OffsetY);
	}

    void Awake() {
		gridOffset = new IntVector2(OffsetX, OffsetY);
        grid = new GameObject[grid_width, grid_height];
    }

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

	}

    /// <summary>
    /// Calls the OnGridChange event if it exists.
    /// </summary>
    private void RunOnGridChange(int x, int y) {
        if (OnGridChange != null)
            OnGridChange(this, new OnGridChangeEventArgs(ID, x, y));
    }

    //Properties:

    /// <summary>
    /// Gets the start position for this grid.
    /// </summary>
    public IntVector2 StartPosition { get; private set; }

    /// <summary>
    /// Gets the end position for this grid.
    /// </summary>
    public IntVector2 EndPosition { get; private set; }

    //Events:

    public event OnGridChangeEventHandler OnGridChange;
}
