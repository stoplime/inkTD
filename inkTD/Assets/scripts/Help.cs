
using System;
using System.Collections.Generic;
using UnityEngine;

namespace helper
{
    public struct IntVector2
    {
        public int x, y;

        public IntVector2 (int[] xy) {
            x = xy[0];
            y = xy[1];
        }
        public IntVector2 (int x, int y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Determines if two IntVector2s are equal in both x and y values.
        /// </summary>
        /// <param name="other">The other IntVector2 being compared.</param>
        /// <returns>Returns true if both IntVector2 structs have the same values.</returns>
        public bool Equals(IntVector2 other){
            return other.x == this.x && other.y == this.y;
        }

        public static IntVector2 operator+(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }
    }

    public static class Help
    {
        /// <summary>
        /// A node used during A* calculations.
        /// </summary>
        private class Node
        {
            public IntVector2 Location { get; set; }
            public float G { get; set; }
            public float H { get; set; }
            public float F { get { return this.G + this.H; } }
            public Node ParentNode { get; set; }
        }

        /// <summary>
        /// Takes an input Vector3 and converts it to the grid IntVector2
        /// </summary>
        /// <param name="input">World position</param>
        /// <returns></returns>
        public static IntVector2 posToGrid(Vector3 input){
            return new IntVector2((int)System.Math.Round(input.x/gridSize),
                                (int)System.Math.Round(input.z/gridSize));
        }

        /// <summary>
        /// Takes an grid IntVector2 and converts it to the pos Vector3
        /// </summary>
        /// <param name="input">grid coordinates</param>
        /// <returns></returns>
        public static Vector3 gridToPos(IntVector2 input){
            return new Vector3(input.x*gridSize, 0,
                            input.y*gridSize);
        }

        /// <summary>
        /// If true then the mouse is currently over some UI element, false otherwise.
        /// </summary>
        public static bool MouseOnUI { get; set; }

        /// <summary>
        /// A blank button used when moving tabs around.
        /// </summary>
        public static GameObject BlankButton { get { return blankButton; } }

        /// <summary>
        /// The RectTransform component of the BlankButton GameObject.
        /// </summary>
        public static RectTransform BlankButtonRect { get { return blankButtonRect; } }

        /// <summary>
        /// Gets or sets the volume of the sound effects emitted from towers.
        /// </summary>
        public static float TowerSoundEffectVolume
        {
            get { return towerSoundEffectVolume; }
            set { towerSoundEffectVolume = value; }
        }
        
       
        private static RectTransform blankButtonRect = null;

        private static GameObject blankButton = null;

        private static float towerSoundEffectVolume = 0.50f;

        /// <summary>
        /// Triggers the onResolutionChange event when called.
        /// </summary>
        public static void TriggerResolutionChangeEvent()
        {
            if (onResolutionChange != null) //Don't simplify this, Unity can't use null propagating
            {
                onResolutionChange(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the blank button of the Help class. The given object must have a rect transform.
        /// </summary>
        /// <param name="button">The blank tab prefab loaded, or otherwise a compatable gameobject with a rect transform component.</param>
        public static void SetBlankButton(GameObject button)
        {
            blankButton = button;
            blankButtonRect = blankButton.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Computes a three point bezier returning the third point along the curve.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="time">The time along the curve (percentage from point1 to point2).</param>
        /// <returns></returns>
        public static Vector3 ComputeBezier(Vector3 point1, Vector3 point2, float time)
        {
            //TODO: Actually compute a bezier curve
            return Vector3.Lerp(point1, point2, time);
        }

        /// <summary>
        /// Gets the object directly in front of the mouse. Returns true if an object was hit, false otherwise.
        /// </summary>
        /// <param name="hit">The raycast hit results.</param>
        /// <returns>Returns true if there was an object that was hit, false otherwise.</returns>
        public static bool GetObjectInMousePath(out RaycastHit hit)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit);
        }

        /// <summery>
        /// creates a list of adjacent nodes from a current node
        /// </summery>
        private static LinkedList<Node> getAdjacentNodes(Node currentNode, IntVector2 end, int playerID)
        {
            LinkedList<Node> l = new LinkedList<Node>();
            IntVector2 offset;
            Node offsetNode;
            for (int i = 0; i < 4; i++)
            {
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
                if (PlayerManager.GetGrid(playerID).inArena(offset) && PlayerManager.GetGrid(playerID).getGridObject(offset) == null)
                {
                    offsetNode = new Node();
                    offsetNode.ParentNode = currentNode;
                    offsetNode.G = currentNode.G + 1;
                    offsetNode.H = Dist(offset, end);
                    //offsetNode.H = Math.Abs(end.x - offset.x) + Math.Abs(end.y - offset.y);
                    offsetNode.Location = offset;
                    l.AddLast(offsetNode);
                }
            }
            return l;
        }

        /// <summery>
        /// calculate an estimate distance value
        /// </summery>
        private static float Dist(IntVector2 node1, IntVector2 node2)
        {
            float x = (float)node1.x - node2.x;
            float y = (float)node1.y - node2.y;
            return Mathf.Sqrt(x * x + y * y);
        }

        private static void Merge(LinkedList<Node> newNodes, ref LinkedList<Node> pathMap, ref LinkedList<Node> availableNodes)
        {
            for (LinkedListNode<Node> newIt = newNodes.First; newIt != null; newIt = newIt.Next)
            {
                bool exists = false;
                for (LinkedListNode<Node> pathIt = pathMap.First; pathIt != null; pathIt = pathIt.Next)
                {
                    if (pathIt.Value.Location.Equals(newIt.Value.Location))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    for (LinkedListNode<Node> availIt = availableNodes.First; availIt != null; availIt = availIt.Next)
                    {
                        if (availIt.Value.Location.Equals(newIt.Value.Location))
                        {
                            exists = true;
                            if (newIt.Value.F < availIt.Value.F)
                            {
                                availIt.Value = newIt.Value;
                            }
                            break;
                        }
                    }
                }
                if (!exists)
                {
                    availableNodes.AddLast(newIt.Value);
                }
            }
            newNodes.Clear();
        }

        private static LinkedList<Node> GetBestPath(LinkedList<Node> pathMap)
        {
            LinkedList<Node> path = new LinkedList<Node>();
            Node node = pathMap.Last.Value;
            while (node != null)
            {
                path.AddLast(node);
                node = node.ParentNode;
            }
            // path.Reverse();
            return path;
        }

        private static LinkedList<Node> AStart(IntVector2 start, IntVector2 end, int playerID)
        {
            Node startNode = new Node();
            startNode.Location = start;
            LinkedList<Node> pathMap = new LinkedList<Node>();
            pathMap.AddFirst(startNode);
            LinkedList<Node> availableNodes = getAdjacentNodes(startNode, end, playerID); //new LinkedList<Node>();
            LinkedList<Node> immediateNodes = new LinkedList<Node>();//getAdjacentNodes(startNode, end, playerID);
            LinkedListNode<Node> it;
            LinkedListNode<Node> minIt;
            Node minNode;

            while (availableNodes.Count > 0 || immediateNodes.Count > 0)
            {
                //if (immediateNodes.Count == 0)
                //{//loop through available nodes if there are no immediate nodes.
                    it = availableNodes.First;
                    minIt = availableNodes.First;
                    minNode = it.Value;
                    it = it.Next;

                    //TODO: Instead of looping through all available nodes to find the lowest F, it would instead be beneficial to make
                    //available nodes into a priority queue (something like https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp).
                    while (it != null)
                    {
                        if (minNode.F > it.Value.F)
                        {
                            minNode = it.Value;
                            minIt = it;
                        }
                        it = it.Next;
                    }
                    availableNodes.Remove(minIt);
                    Merge(getAdjacentNodes(minNode, end, playerID), ref pathMap, ref availableNodes);
                //}
                //else
                //{//loop through immediate nodes (nodes within 1 distance of the currentNode) if there's at least 1 option.
                //    it = immediateNodes.First;
                //    minIt = immediateNodes.First;
                //    minNode = it.Value;
                //    it = it.Next;
                //    while (it != null)
                //    {
                //        if (minNode.F > it.Value.F)
                //        {
                //            minNode = it.Value;
                //            minIt = it;
                //        }
                //        it = it.Next;
                //    }
                //    immediateNodes.Remove(minIt);
                //    Merge(immediateNodes, ref pathMap, ref availableNodes); //Merge leftover immediate nodes into available nodes.
                //    immediateNodes.Clear();
                //}

                //Merge(getAdjacentNodes(minNode, end, playerID), ref pathMap, ref immediateNodes); //Get the new list of immediate nodes.

                pathMap.AddLast(minNode);

                if (minNode.Location.Equals(end))
                {
                    return GetBestPath(pathMap);
                }
            }

            return new LinkedList<Node>();
        }

        /// <summary>
        /// Gets the best path within the grid of the given player id.
        /// </summary>
        /// <param name="playerID">The player's id who controls the grid which the path will be determined.</param>
        /// <param name="start">The starting point in the grid.</param>
        /// <param name="end">The end point in the grid.</param>
        /// <returns></returns>
        public static List<IntVector2> GetGridPath(int playerID, IntVector2 start, IntVector2 end)
        {
            LinkedList<Node> nodes = AStart(start, end, playerID);
            List<IntVector2> path = new List<IntVector2>(nodes.Count);
            for (LinkedListNode<Node> it = nodes.First; it != null; it = it.Next)
            {
                path.Add(it.Value.Location);
            }
            return path;
        }

        /// <summary>
        /// An event that runs whenever the resolution changes.
        /// </summary>
        public static event EventHandler onResolutionChange;

    }
}

