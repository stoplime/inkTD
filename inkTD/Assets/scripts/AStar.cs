using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class AStar : MonoBehaviour {

	private class Node{
		public IntVector2 Location { get; set; }
		public float G { get; set; }
		public float H { get; set; }
		public float F { get { return this.G + this.H; } }
		public Node ParentNode { get; set; }
	}

	public Grid grid = GameObject.Find("Player Field").GetComponent<Grid>();

	public IntVector2 Target;

	public IntVector2 Spawn;

	/// <summery>
	/// creates a list of adjacent nodes from a current node
	/// </summery>
	private LinkedList<Node> getAdjacentNodes(Node currentNode, IntVector2 end, int playerID){
		LinkedList<Node> l = new LinkedList<Node>();
		IntVector2 offset;
		Node offsetNode;
		for (int i = 0; i < 4; i++){
			offset = currentNode.Location;
			switch (i)
			{
				case 1:
					offset.x += 1;
					break;
				case 2:
					offset.y += 1;
					break;
				case 3:
					offset.x -= 1;
					break;
				default:
					offset.y -= 1;
					break;
			}
			if(PlayerManager.GetGrid(playerID).inArena(offset) && PlayerManager.GetGrid(playerID).getGridObject(offset) == null){
				offsetNode = new Node();
				offsetNode.ParentNode = currentNode;
				offsetNode.G = currentNode.G + 1;
				offsetNode.H = Dist(offset, end);
				l.AddLast(offsetNode);
			}
		}
		return l;
	}

	/// <summery>
	/// calculate an estimate distance value
	/// </summery>
	private float Dist(IntVector2 node1, IntVector2 node2){
		float x = (float)node1.x - node2.x;
		float y = (float)node1.y - node2.y;
		return Mathf.Sqrt(x*x + y*y);
	}

	private void Merge(LinkedList<Node> newNodes, ref LinkedList<Node> pathMap, ref LinkedList<Node> availableNodes){
		for(LinkedListNode<Node> newIt = newNodes.First; newIt != null; newIt = newIt.Next){
			bool exists = false;
			for(LinkedListNode<Node> pathIt = pathMap.First; pathIt != null; pathIt = pathIt.Next){
				if(pathIt.Value.Location.Equals(newIt.Value.Location)){
					exists = true;
					break;
				}
			}
			if(!exists){
				for(LinkedListNode<Node> availIt = availableNodes.First; availIt != null; availIt = availIt.Next){
					if(availIt.Value.Location.Equals(newIt.Value.Location)){
						exists = true;
						if(newIt.Value.F < availIt.Value.F){
							availIt.Value = newIt.Value;
						}
						break;
					}
				}
			}
			if(!exists){
				availableNodes.AddLast(newIt);
			}
		}
		newNodes.Clear();
	}

	private LinkedList<Node> GetBestPath(LinkedList<Node> pathMap){
		LinkedList<Node> path = new LinkedList<Node>();
		Node node = pathMap.Last.Value;
		while (node != null){
            path.AddLast(node);
            node = node.ParentNode;
        }
        // path.Reverse();
		return path;
	}

	private LinkedList<Node> AStart(IntVector2 start, IntVector2 end, int playerID){
		Node startNode = new Node();
		startNode.Location = start;
		LinkedList<Node> pathMap = new LinkedList<Node>();
		LinkedList<Node> availableNodes = getAdjacentNodes(startNode, end, playerID);
		
		while(availableNodes.Count > 0){
			LinkedListNode<Node> it = availableNodes.First;
			LinkedListNode<Node> minIt = availableNodes.First;
			float minF = it.Value.F;
			it = it.Next;
			while(it != null){
				if(minF > it.Value.F){
					minF = it.Value.F;
					minIt = it;
				}
				it = it.Next;
			}
			Merge(getAdjacentNodes(minIt.Value, end, playerID), ref pathMap, ref availableNodes);
			pathMap.AddLast(minIt);

			if(minIt.Value.Location.Equals(end)){
				return GetBestPath(pathMap);
			}
			availableNodes.Remove(minIt);
		}
		
		
		return new LinkedList<Node>();
	}

	// private List<IntVector2> RunAStar(float[,] g, bool[,] visited, IntVector2 current, IntVector2 end, List<IntVector2> path){
	// 	List<IntVector2> validMoves = getAdjacentNodes(current, g);

	// 	if(validMoves.Count == 0){
	// 		return path;
	// 	}

	// 	float minFValue = calculateF(g[validMoves[0].x, validMoves[0].y], current, end);
	// 	IntVector2 minIndex = validMoves[0];
	// 	if(validMoves.Count > 1){
	// 		float fValue;
	// 		for (int i = 0; i < validMoves.Count; i++){
	// 			fValue = calculateF(g[validMoves[i].x, validMoves[i].y], current, end);
	// 			if(minFValue > fValue){
	// 				minFValue = fValue;
	// 				minIndex = validMoves[i];
	// 			}
				
	// 		}
	// 	}
	// 	return path;
	// }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
