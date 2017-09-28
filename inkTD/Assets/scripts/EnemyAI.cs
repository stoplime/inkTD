using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
