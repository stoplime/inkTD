﻿using System.Collections;
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

	private Vector3 animatePos;

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
	private float animateTime = 0;

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
		Vector3[] bezier = new Vector3[2];

		Vector3 s1 = Grid.gridToPos(current);
		Vector3 s2 = Grid.gridToPos(next);
		if (startCenter)
		{
			bezier[0] = s1;
			bezier[1] = (s1+s2)/2;
		}
		else
		{
			bezier[0] = (s1+s2)/2;
			bezier[1] = s2;
		}

		return bezier;
	}

	/// <summary>
	/// Handles the movement of the creatures.
	/// </summary>
	/// <param name="gridSpeed"></param>
	/// <param name="animationSpeed"></param>
	private void move(float gridSpeed)
	{
		time += Time.deltaTime * gridSpeed;
		print(time);
		if (time > 1)
		{
			time -= 1;
			pathIndex++;
			if(pathIndex == path.Count){
				Destroy(gameObject);
			}
			else{
				gridPos = path[pathIndex];//*/Grid.posToGrid(pos);
			}
		}

		if (path.Count > 1 && pathIndex != path.Count)
		{
			if (pathIndex-1 < 0)
			{
				pos = Help.ComputeBezier(time, getGridCurve(path[pathIndex], path[pathIndex+1], true));
				// print(path.Count);
			}
			else if (path.Count-1 == pathIndex)
			{
				pos = Help.ComputeBezier(time, getGridCurve(path[pathIndex-1], path[pathIndex], false));
			}
			else
			{
				pos = Help.ComputeBezier(time, getGridCurve(path[pathIndex-1], path[pathIndex], path[pathIndex+1]));
			}
		}
	}

	private void animate(float animationSpeed)
	{
		animateTime += Time.deltaTime * animationSpeed;
		if(animateTime > 1){
			animateTime -= 1;
		}

		Vector3[] animatePoints = {Vector3.zero, new Vector3(0, animationHeight, 0), Vector3.zero};

		animatePos = Help.ComputeBezier(animateTime, animatePoints);

		Vector3 lookAtTarget = new Vector3(pos.x, transform.position.y, pos.z);
		transform.LookAt(lookAtTarget);
		transform.position = animatePos+pos;
	}

	/// <summary>
	/// recalculates the path if pathUpdateFlag is true.
	/// </summary>
	private void updatePath(IntVector2 end)
	{
		if(pathUpdateFlag)
		{
			pathUpdateFlag = false;
			path = Help.GetGridPath(gridID, gridPos, end);
			pathIndex = 0;
            gameObject.GetComponent<GridVisualizer>().SetPath(path);
		}
	}

	/// <summary>
	/// If a given gridID has changed, then all creatures in that grid will run this function.
	/// </summary>
	public void OnGridChange(Grid grid, OnGridChangeEventArgs e)
	{
		if (grid.ID == gridID)
		{
			pathUpdateFlag = true;
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
		gridEnd = new IntVector2(10,15);
		path = Help.GetGridPath(gridID, gridPos, gridEnd);
        gameObject.GetComponent<GridVisualizer>().SetPath(path);
        PlayerManager.GetGrid(gridID).OnGridChange += OnGridChange;
        
        if (path.Count == 0)
		{
			throw new System.ArgumentException("Best path does not exist", "pathing");
		}
        else
        {
            transform.position = Grid.gridToPos(path[0]);
        }
	}

    void OnDestroy()
    {
        PlayerManager.GetGrid(gridID).OnGridChange -= OnGridChange;
    }
	
	// Update is called once per frame
	void Update () {
		move(speed);
		animate(2*speed);
		updatePath(gridEnd);
	}
}