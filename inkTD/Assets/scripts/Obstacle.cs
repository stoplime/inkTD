using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class Obstacle : MonoBehaviour {

	public float Price;

	public IntVector2 ObstacleSize;

	public GameObject PlaceHolderObject;

	private GameObject[,] PlaceHolders;

	// Use this for initialization
	void Start () {
		PlaceHolders = new GameObject[ObstacleSize.x, ObstacleSize.y];
		for (int i = 0; i < PlaceHolders.GetLength(0); i++)
		{
			for (int j = 0; j < PlaceHolders.GetLength(1); j++)
			{
				PlaceHolders[i, j] = PlaceHolderObject;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
