using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

AI PLANS

Tower placement
- go through the optimal path backwards and look and the adjacent nodes on the path
  to find an available placement.
- use a weighted random selection between adjacent nodes to place

Creature spawning
- hard coded strategies that will use combinations of creatures
- spawn based off a random weighted list of actions, where an action is a creature or combination

 */

public class EnemyAI : MonoBehaviour {

	public int ID;

	private Grid currentGrid;

	public void getBestTowerPos()
	{
		
	}

	// Use this for initialization
	void Start () {
		currentGrid = gameObject.GetComponentsInParent<Grid>()[0];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
