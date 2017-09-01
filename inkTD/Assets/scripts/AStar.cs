using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class AStar : MonoBehaviour {

	private class Node{
		public IntVector2 Location { get; private set; }
		public float G { get; private set; }
		public float H { get; private set; }
		public float F { get { return this.G + this.H; } }
		public Node ParentNode { get; set; }
	}

	public Grid grid = GameObject.Find("Player Field").GetComponent<Grid>();

	public IntVector2 Target;

	public IntVector2 Spawn;
	
	// TODO: check if g value is less and make it not a valid move
	private LinkedList<IntVector2> getAdjacentNodes(IntVector2 currentNode, float[,] g){
		LinkedList<IntVector2> l = getAdjacentNodes(currentNode);
		float currentGvalue = g[currentNode.x, currentNode.y];
		for(int i = l.Count-1; i >= 0; i--){
			// if the next step is less than the current step
			if(g[l[i].x, l[i].y] <= currentGvalue){
				l.RemoveAt(i);
			}
		}
		return l;
	}

	/// <summery>
	/// creates a list of adjacent nodes from a current node
	/// </summery>
	private LinkedList<Node> getAdjacentNodes(Node currentNode){
		LinkedList<Node> l = new LinkedList<Node>();
		Node offsetNode;
		for (int i = 0; i < 4; i++){
			offsetNode = new Node();
			offsetNode.ParentNode = currentNode;
			switch (i)
			{
				case 1:
					offsetNode.Location = new IntVector2(currentNode.Location.x + 1, currentNode.Location.y);
					break;
				case 2:
					offsetNode.Location = new IntVector2(currentNode.Location.x, currentNode.Location.y - 1);
					break;
				case 3:
					offsetNode.Location = new IntVector2(currentNode.Location.x, currentNode.Location.y + 1);
					break;
				default:
					offsetNode.Location = new IntVector2(currentNode.Location.x - 1, currentNode.Location.y);
					break;
			}
			if(grid.inArena(offsetNode.Location) && grid.getGridObject(offsetNode) == null){
				l.Add(offsetNode);
			}

		}
		return l;
	}

	/// <summery>
	/// calculate an estimate distance value
	/// </summery>
	private float dist(IntVector2 node1, IntVector2 node2){
		float x = (float)node1.x - node2.x;
		float y = (float)node1.y - node2.y;
		return Mathf.Sqrt(x*x + y*y);
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

	private List<IntVector2> RunAStar(float[,] g, bool[,] visited, IntVector2 current, IntVector2 end, List<IntVector2> path){
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
		return path;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
