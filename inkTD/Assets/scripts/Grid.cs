using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;
using System;

public class Grid : MonoBehaviour {

	public int ID;

    public int startX = 0;
    public int startY = 0;
    public int endX = 1;
    public int endY = 1;

	public static float gridSize = 3;

	/// <summary>
	/// Takes an input Vector3 and converts it to the grid IntVector2
	/// </summary>
	public static IntVector2 posToGrid(Vector3 input){
		return new IntVector2((int)System.Math.Round(input.x/gridSize),
						   	  (int)System.Math.Round(input.z/gridSize));
	}

	/// <summary>
	/// Takes an grid IntVector2 and converts it to the pos Vector3
	/// </summary>
	public static Vector3 gridToPos(IntVector2 input){
		return new Vector3(input.x*gridSize, 0,
						   input.y*gridSize);
	}

	/// <summary>
	/// array containing the towers of the playing field
	/// </summary>
	private GameObject[,] grid;

	public void setGridObject(IntVector2 xy, GameObject obj){
		grid[xy.x, xy.y] = obj;
        RunOnGridChange(xy.x, xy.y);
    }
	public void setGridObject(int x, int y, GameObject obj){
		grid[x, y] = obj;
        RunOnGridChange(x,y);
    }
	public GameObject getGridObject(IntVector2 xy){
		return grid[xy.x, xy.y];
	}
	public GameObject getGridObject(int x, int y){
		return grid[x, y];
	}

	/// <summery>
	/// offset the grid 0,0 per grid unit away
	/// </summery>
	public Vector2 gridOffset;

	/// <summary>
	/// width and height of each playing field
	/// </summary>
	public int grid_width = 20;
	public int grid_height = 10;

	/// <summery>
	/// checks if grid pos is currently occupide with a tower
	/// </summery>
	public bool isGridEmpty(IntVector2 pos){
		if (grid[pos.x, pos.y] != null){
			return false;
		}
		return true;
	}

	/// <summery>
	/// checks if grid pos is within the grid arena
	/// </summery>
	public bool inArena(IntVector2 pos){
		if (pos.x >= 0 && pos.x < grid_width){
			if (pos.y >= 0 && pos.y < grid_height){
				return true;
			}
		}
		return false;
	}

	public GameObject particle;
	public GameObject highlight;
	GameObject existingHighlight = null;
	bool towerSelected = true;

    private RaycastHit hit;

    void Awake() {
        grid = new GameObject[grid_width, grid_height];
    }

	// Use this for initialization
	void Start () {
		highlight.transform.localScale = new Vector3(gridSize, 0.1f, gridSize);
	}

	// Update is called once per frame
	void Update () {
		if (!Help.MouseOnUI){
			if (Help.GetObjectInMousePath(out hit)){
				if (hit.collider.tag == "GroundObject"){
					IntVector2 gridPos = posToGrid(hit.point);
					if (inArena(gridPos) && isGridEmpty(gridPos)){
						Vector3 target = gridToPos(gridPos);
						target.y = 0.1f;
						if(existingHighlight == null){
							existingHighlight = Instantiate(highlight, target, hit.collider.transform.rotation);
						}else{
							existingHighlight.transform.position = target;
						}
						if (Input.GetButtonDown("Fire1")){
							target.y = 1f;
							GameObject newCube = Instantiate(particle, target, hit.collider.transform.rotation);
                            setGridObject(gridPos, newCube);
						}
					}
					else if(existingHighlight != null){
						existingHighlight.transform.position = new Vector3(0,-100,0);
					}
				}
			}
		}
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
