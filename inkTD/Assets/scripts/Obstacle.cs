using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	public float Price;

	public Vector2 ObstacleSize;

	public GameObject PlaceHolderObject;

	private GameObject[,] PlaceHolders;

	// Use this for initialization
	void Start () {
		PlaceHolders = new GameObject[ObstacleSize.x, ObstacleSize.y];
		foreach (GameObject i in PlaceHolders)
		{
			i = PlaceHolderObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
