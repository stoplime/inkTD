using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	public static float gridSize = 2;

	/// <summary>
	/// Takes an input Vector2 and converts it to the grid Vector2
	/// </summary>
	public static Vector3 parseInput(Vector3 input){
		return new Vector3((float)System.Math.Round(input.x/gridSize)*gridSize, 0,
						   (float)System.Math.Round(input.z/gridSize)*gridSize);
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

	// Use this for initialization
	void Start () {
		grid = new Object[grid_width, grid_height];
	}

	public GameObject particle;
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
            if (Physics.Raycast(ray, out hit)){
				if (hit.collider.tag == "GroundObject"){
					Vector3 target = parseInput(hit.point);
					target.y = 1f;
                	Instantiate(particle, target, hit.collider.transform.rotation);
				}
			}
        }
	}
}
