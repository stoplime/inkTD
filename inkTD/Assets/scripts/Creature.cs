using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class Creature : InkObject
{
	// [Tooltip("max Health.")]
    // public float maxHealth;

	// [Tooltip("current health.")]
    // public float health;

	[Tooltip("A percentage taken off of the regular damage.")]
    public float defense;

    [Tooltip("The increase inkcome when spawned. Example: 1, means the Inkcome will += 1 whenever this creature gets spawned.")]
    public int inkcomeValue;

	[Tooltip("How much ink is droped when killed. Ex: 2")]
    public int dropInk;

	[Tooltip("The id of the actual grid the creature is in.")]
	public int gridID = 0;

	public float animationHeight = 2;

	private IntVector2 gridEnd;

	/// <summary>
	/// the current best path for a specific instance of a creature.
	/// </summary>
	private List<IntVector2> path;
	/// <summary>
	/// the current index of the path. If the path updates, index needs to reset to 0.
	/// </summary>
	private int pathIndex = 0;

	/// <summary>
	/// a flag that represents if the path needs to be updated.
	/// </summary>
	private bool pathUpdateFlag = false;

	private float time = 0;

	private Vector3[] getGridCurve(IntVector2 previous, IntVector2 current, IntVector2 next)
	{
		Vector3[] bezier = new Vector3[3];

		Vector3 s0 = Grid.gridToPos(previous);
		Vector3 s1 = Grid.gridToPos(current);
		Vector3 s2 = Grid.gridToPos(next);
		bezier[0] = (s0+s1)/2;
		bezier[1] = s1;
		bezier[2] = (s1+s2)/2;

		return bezier;
	}
	
	private Vector3[] getGridCurve(IntVector2 current, IntVector2 next, bool startCenter)
	{
		Vector3[] bezier = new Vector3[3];

		Vector3 s1 = Grid.gridToPos(current);
		Vector3 s2 = Grid.gridToPos(next);
		if (startCenter)
		{
			bezier[1] = s1;
			bezier[2] = (s1+s2)/2;
		}
		else
		{
			bezier[1] = (s1+s2)/2;
			bezier[1] = s2;
		}

		return bezier;
	}

	/// <summary>
	/// Handles the movement of the creatures.
	/// </summary>
	/// <param name="gridSpeed"></param>
	/// <param name="animationSpeed"></param>
	private void move(ref float time, float gridSpeed, float animationSpeed)
	{
		time += Time.deltaTime * gridSpeed;
		// print(time);
		if (time > 1)
		{
			time -= 1;
			pathIndex++;
			gridPos = Grid.posToGrid(pos);
		}

		if (path.Count > 1)
		{
			if (pathIndex-1 < 0)
			{
				pos = Help.ComputeBezier(time, getGridCurve(gridPos, path[pathIndex+1], true));
<<<<<<< HEAD
				//print(path.Count);
=======
				// print(path.Count);
>>>>>>> e551e3d32e269a6266424ba4cfe3d4d192e28f1b
			}
			else if (path.Count-1 == pathIndex+1)
			{
				pos = Help.ComputeBezier(time, getGridCurve(gridPos, path[pathIndex+1], false));
			}
			else if (path.Count-1 > pathIndex+1)
			{
				// TODO: Creature has finished the path and needs to be destroyed
				return;
			}
			else
			{
				pos = Help.ComputeBezier(time, getGridCurve(path[pathIndex-1], gridPos, path[pathIndex+1]));
			}
		}
		transform.position = pos;
	}

	private void animate(float time)
	{

	}

	/// <summary>
	/// recalculates the path if pathUpdateFlag is true.
	/// </summary>
	private void updatePath(IntVector2 end)
	{
		if(pathUpdateFlag)
		{
<<<<<<< HEAD
			//print("PAth Chaned");
=======
			// print("PAth Chaned");
>>>>>>> e551e3d32e269a6266424ba4cfe3d4d192e28f1b
			pathUpdateFlag = false;
			path = Help.GetGridPath(gridID, gridPos, end);
		}
	}

	/// <summary>
	/// If a given gridID has changed, then all creatures in that grid will run this function.
	/// </summary>
	public void OnGridChange(Grid grid, OnGridChangeEventArgs e)
	{
		// print("GRid Chaned");
		if (grid.ID == gridID)
		{
			pathUpdateFlag = true;
			if(gridEnd.x != grid.endX || gridEnd.y != grid.endY)
				gridEnd = new IntVector2(grid.endX, grid.endY);
			gameObject.GetComponent<GridVisualizer>().VisualizePath(path);
		}
	}

	public int initX;
	public int initY;

	// Use this for initialization
	void Start () {
		gridPos = new IntVector2(initX, initY);
		pos = Grid.gridToPos(gridPos);
		PlayerManager.SpawnCreature(ownerID, gridID, this);
		var a = PlayerManager.GetBestPath(gridID);
		gridEnd = a[a.Count-1];
		path = Help.GetGridPath(gridID, gridPos, gridEnd);
		
		if (path.Count == 0)
		{
			throw new System.ArgumentException("Best path does not exist", "pathing");
		}
	}
	
	// Update is called once per frame
	void Update () {
		move(ref time, speed, speed);
		updatePath(gridEnd);
	}
}
