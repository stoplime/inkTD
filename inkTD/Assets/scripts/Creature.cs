using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;
using System;

public class Creature : InkObject
{
	[Tooltip("A percentage taken off of the regular damage.")]
    public float defense;

    [Tooltip("The increase inkcome when spawned. Example: 1, means the Inkcome will += 1 whenever this creature gets spawned.")]
    public int inkcomeValue;

	[Tooltip("How much ink is droped when killed. Ex: 2")]
    public int dropInk;

	[Tooltip("The id of the actual grid the creature is in.")]
	public int gridID = 0;

	public float animationHeight = 2;

	public bool debug = false;

	public Vector3 OffsetTranslation = new Vector3(0, 1, 0);

	public Vector3 OffsetRotation = new Vector3(-270, 0, 0);

    /// <summary>
    /// Gets the unique identifier for this creature.
    /// </summary>
    public Guid UniqueID
    {
        get { return uniqueID; }
    }

	private IntVector2 gridEnd;

	private Vector3 animatePos;
    Vector3[] animatePoints = new Vector3[3];

    /// <summary>
    /// the current best path for a specific instance of a creature.
    /// </summary>
    private List<IntVector2> path;

    private List<IntVector2> tempPath;

	public bool tempPathExists
	{
		get{
			return tempPath.Count > 0;
		}
	}

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

    private Guid uniqueID = Guid.NewGuid();

	public void TakeDamage(float damage)
	{
		Health = health - damage;
	}

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

	public Vector3 PredictPos(float time)
	{
		// rounding
		int deltaIndex = (int)((time/speed)+0.5f);
		if (path != null)
		{
			if (pathIndex+deltaIndex >= path.Count)
			{
				return Grid.gridToPos(path[path.Count-1]);
			}
			return Grid.gridToPos(path[pathIndex + deltaIndex]);
		}
		return Grid.gridToPos(gridPos);
	}

	/// <summary>
	/// Handles the movement of the creatures.
	/// </summary>
	/// <param name="animationSpeed"></param>
	private void move()
	{
		time += Time.deltaTime * speed;
		// print(time);
		if (time > 1)
		{
			time -= 1;
			pathIndex++;
			if(pathIndex >= path.Count){
                //Reached the end of the path and succeeded...
                PlayerManager.GetGrid(gridID).DamageTowerCastle(damage);

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
        
        animatePoints[1] = new Vector3(0, animationHeight, 0);
        
        animatePos = Help.ComputeBezier(animateTime, animatePoints);

		Vector3 lookAtTarget = new Vector3(pos.x, transform.position.y, pos.z);
		transform.LookAt(lookAtTarget);
		transform.position = animatePos+pos+OffsetTranslation;
		transform.rotation *= Quaternion.Euler(OffsetRotation);
		// transform.position += new Vector3(0, GetTerrainHeight(), 0);
	}

	/// <summary>
	/// recalculates the path if pathUpdateFlag is true.
	/// </summary>
	public void updatePath()
	{
		path = tempPath;
		pathIndex = 0;
		if (gameObject.GetComponent<PathVisualizer>().enabled)
		{
			gameObject.GetComponent<PathVisualizer>().SetPath(path);
		}
	}

	public void updateTempPath()
	{
		tempPath = Help.GetGridPath(gridID, gridPos, gridEnd);
	}

	// /// <summary>
	// /// If a given gridID has changed, then all creatures in that grid will run this function.
	// /// </summary>
	// public void OnGridChange(Grid grid, OnGridChangeEventArgs e)
	// {
	// 	if (grid.ID == gridID)
	// 	{
	// 		// tempPath = Help.GetGridPath(gridID, gridPos, end);
	// 	}
	// }

	private void CheckDeath()
	{
		if (Health <= 0.00001)
		{
			// Check ink
			PlayerManager.AddBalance(gridID, dropInk);

			Destroy(gameObject);
		}
	}

	// Use this for initialization
	public override void Start () {
        base.Start();

		Grid grid = PlayerManager.GetGrid(gridID);
		gridPos = grid.StartPosition;
		pos = Grid.gridToPos(gridPos);

		if(debug){
			// regular creatures should get added in the CreatureSpawner class
			PlayerManager.AddCreature(ownerID, gridID, this);
		}
		
		var a = PlayerManager.GetBestPath(gridID);
		gridEnd = a[a.Count-1];
		path = Help.GetGridPath(gridID, gridPos, gridEnd);

		if(debug){
			// currently set up for paths only exists if they are in debug
        	gameObject.GetComponent<PathVisualizer>().SetPath(path);
		}
		// PlayerManager.GetGrid(gridID).OnGridChange += OnGridChange;

        animatePoints[0] = Vector3.zero;
        animatePoints[2] = Vector3.zero;

        if (path.Count == 0)
		{
			throw new System.ArgumentException("Best path does not exist", "pathing");
		}
        else
        {
            transform.position = Grid.gridToPos(path[0]);
        }

		// Add Inkcome Value
		// PlayerManager.AddIncome(ownerID, inkcomeValue);
		transform.rotation = Quaternion.Euler(OffsetRotation);
	}

    void OnDestroy()
    {
        // PlayerManager.GetGrid(gridID).OnGridChange -= OnGridChange;
        List<Creature> creatures = PlayerManager.GetCreatures(gridID);
        for (int i = creatures.Count - 1; i >= 0; i --)
        {
            if (creatures[i].uniqueID == uniqueID)
            {
                creatures.RemoveAt(i);
                break;
            }
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		move();
		animate(2*speed);
		CheckDeath();
	}
}