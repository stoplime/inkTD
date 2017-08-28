using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class Grid : MonoBehaviour {

	public static float gridSize = 2;

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
	public Object[,] grid;

	/// <summery>
	/// offset the grid 0,0 per grid unit away
	/// </summery>
	public Vector2 gridOffset;

	/// <summary>
	/// width and height of each playing field
	/// </summary>
	public int grid_width = 20;
	public int grid_height = 10;


	bool isGridEmpty(IntVector2 pos){
		if (grid[pos.x, pos.y] != null){
			return false;
		}
		return true;
	}

	/// <summery>
	/// checks if grid pos is within the grid arena
	/// </summery>
	bool inArena(IntVector2 pos){
		if (pos.x > 0 && pos.x <= grid_width){
			if (pos.y > 0 && pos.y <= grid_height){
				return true;
			}
		}
		return false;
	}

	// Use this for initialization
	void Start () {
		grid = new Object[grid_width, grid_height];
	}

	public GameObject particle;
	public GameObject highlight;
	GameObject existingHighlight = null;
	bool towerSelected = true;
	// Update is called once per frame
	void Update () {
		if (!Help.MouseOnUI){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)){
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
							grid[(int)gridPos.x, (int)gridPos.y] = newCube;
						}
					}
					else if(existingHighlight != null){
						existingHighlight.transform.position = new Vector3(0,-100,0);
					}
				}
			}
		}
	}
}
