using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class AStar : MonoBehaviour {

	public Grid grid = GameObject.Find("Player Field").GetComponent<Grid>();

	public IntVector2 Target;

	public IntVector2 Spawn;
	
	// TODO: check if g value is less and make it not a valid move
	private List<IntVector2> getAdjacentNodes(IntVector2 currentNode, float[,] g){
		List<IntVector2> l = getAdjacentNodes(currentNode);
		float currentGvalue = g[currentNode.x, currentNode.y];
		for(int i = l.Count-1; i >= 0, i--){
			// if the next step is less than the current step
			if(g[l[i].x, l[i].y] <= current){
				l.RemoveAt(i);
			}
		}
		return l;
	}

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
			if(grid.inArena(offsetNode)){
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

	private float calculateF(float g, IntVector2 i, IntVector2 end){ // use ref if inefficent
		return g + dist(i, end);
	}

	public bool PathExists(IntVector2 currentNode){
		
		List<IntVector2> nextNodes = getAdjacentNodes(currentNode);
		foreach (var nextNode in nextNodes){

		}
		return false;
	}

	private List<IntVector2> AStar(float[,] g, bool[,] visited, IntVector2 current, IntVector2 end, List<IntVector2> path){
		List<IntVector2> validMoves = getAdjacentNodes(current, g);

		if(validMoves.Count == 0){
			return path;
		}

		float minFValue = calculateF(g[validMoves[0].x, validMoves[0].y], current, end);
		IntVector2 minIndex = validMoves[0];
		if(validMoves.Count > 1){
			float fValue;
			for (int i = 0; i < validMoves.Count; i++){
				fValue = calculateF(g[validMoves[i].x, validMoves[i].y], current, end);
				if(minFValue > fValue){
					minFValue = fValue;
					minIndex = validMoves[i];
				}
				
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
