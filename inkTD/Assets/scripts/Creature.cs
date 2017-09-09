using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class Creature : InkObject
{
	[Tooltip("max Health.")]
    public float maxHealth;

	[Tooltip("current health.")]
    public float health;

	[Tooltip("A percentage taken off of the regular damage.")]
    public float defense;

    [Tooltip("The increase inkcome when spawned. Example: 1, means the Inkcome will += 1 whenever this creature gets spawned.")]
    public int inkcomeValue;

	[Tooltip("How much ink is droped when killed. Ex: 2")]
    public int dropInk;

	[Tooltip("The id of the actual grid the creature is in.")]
	public int gridID = 0;

	public float animationHeight = 2;

	private IntVector2 currentGridDirection;

	private List<IntVector2> path;

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

	private void move(IntVector2 currentGrid, IntVector2 nextGrid, float gridSpeed, float animationSpeed)
	{
		Vector3 current = Grid.gridToPos(currentGrid);
		Vector3 next = Grid.gridToPos(nextGrid);

		// Vector3[] bezier = {current, new Vector3((current.x + next.x)/2, current.x + animationHeight, (current.z + next.z)/2), next};

		// pos = Help.ComputeBezier(bezier, gridSpeed);
	}

	private void animate(float time)
	{

	}

	// Use this for initialization
	void Start () {
		path = PlayerManager.GetBestPath(gridID);
		if (path.Count >= 2)
		{
			currentGridDirection = path[1] - path[0];
		}
		else if (path.Count > 0)
		{
			currentGridDirection = new IntVector2(0, 0);
		}
		else
		{
			throw new System.ArgumentException("Best path does not exist", "pathing");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
