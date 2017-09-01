using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class AStar : MonoBehaviour {

	public Grid grid = GameObject.Find("Player Field").GetComponent<Grid>();

	public IntVector2 Target;

	public IntVector2 Spawn;

	/// <summery>
	/// creates a list of adjacent nodes from a current node
	/// </summery>
	private List<IntVector2> getAdjacentNodes(IntVector2 currentNode){
		List<IntVector2> l = new List<IntVector2>();
		IntVector2 offsetNode;
		for (int i = 0; i < 4; i++){
			switch (i)
			{
				case 1:
					offsetNode = new IntVector2(currentNode.x + 1, currentNode.y);
					break;
				case 2:
					offsetNode = new IntVector2(currentNode.x, currentNode.y - 1);
					break;
				case 3:
					offsetNode = new IntVector2(currentNode.x, currentNode.y + 1);
					break;
				default:
					offsetNode = new IntVector2(currentNode.x - 1, currentNode.y);
					break;
			}
			if(grid.inArena(offsetNode) && grid.getGridObject(offsetNode) == null){
				l.Add(offsetNode);
			}
		}
		return l;
	}

	/// <summery>
	/// calculate an estimate distance value
	/// </summery>
	private float dist(IntVector2 node1, IntVector2 node2){
		return Mathf.Abs(node1.x - node2.x) + Mathf.Abs(node1.y - node2.y);
	}

	private bool search(IntVector2 currentNode){

		List<IntVector2> nextNodes = getAdjacentNodes(currentNode);
		foreach (var nextNode in nextNodes){

		}
		return false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
