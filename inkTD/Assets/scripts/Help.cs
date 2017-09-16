
using System;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

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

        public static IntVector2 operator-(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }
    }

    public static class Help
    {
        private static float Epsilon = 1.5f;

        /// <summary>
        /// A node used during A* calculations.
        /// </summary>
        private class Node : FastPriorityQueueNode
        {
            public int x;

            public int y;
            public float G { get; set; }
            public float H { get; set; }
            public float epsilon { get; set; }

            public float F;

            //public float F { get { return this.G + epsilon*this.H; } }
            public Node ParentNode { get; set; }
            public byte listNumb { get; set; }

            public override bool Equals(object obj)
            {
                Node n = obj as Node;
                if (n != null)
                    return n.x == x && n.y == y;

                return false;
            }
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
        public static Vector3 ComputeBezier(float time, Vector3 point1, Vector3 point2, Vector3 point3)
        {
            float inverseTime = 1 - time;
            return inverseTime * inverseTime * point1 + 2 * inverseTime * time * point2 + time * time * point3;
        }


        /// <summary>
        /// Steffen Lim universal bezier curve
        /// Computes an n point bezier returning the final point along the curve.
        /// </summary>
        /// <param name="points">The first point.</param>
        /// <param name="time">The time along the curve (percentage from start to finish). Value from 0 to 1</param>
        /// <returns> Vector 3 point of the position at the given time step</returns>
        public static Vector3 ComputeBezier(float time, params Vector3[] points)
        {
            if (points.Length == 1)
                return points[0];
            
            Vector3[] subPoints = new Vector3[points.Length-1];

            for (int i = 0; i < points.Length-1; i++)
            {
                subPoints[i] = points[i] + time * (points[i+1] - points[i]);
            }

            return ComputeBezier(time, subPoints);
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
        private static LinkedList<Node> getAdjacentNodes(Node[,] nodes, Grid playerGrid, Node currentNode, IntVector2 end, float epsilon)
        {
            LinkedList<Node> l = new LinkedList<Node>();
            IntVector2 offset;
            Node offsetNode;
            for (int i = 0; i < 4; i++)
            {
                offset = new IntVector2(currentNode.x, currentNode.y);
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
                if (playerGrid.inArena(offset) && playerGrid.isGridEmpty(offset))
                {
                    offsetNode = new Node();
                    offsetNode.ParentNode = currentNode;
                    offsetNode.epsilon = epsilon;
                    offsetNode.G = currentNode.G + 1;
                    offsetNode.H = Dist(offset, end);
                    offsetNode.F = offsetNode.G + epsilon * offsetNode.H;
                    //offsetNode.H = Math.Abs(end.x - offset.x) + Math.Abs(end.y - offset.y);
                    offsetNode.x = offset.x;
                    offsetNode.y = offset.y;
                    offsetNode.listNumb = 0; //0 for no list.
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

        private static void Merge(Node[,] nodes, LinkedList<Node> newNodes, ref LinkedList<Node> pathMap, ref FastPriorityQueue<Node> availableNodes, Grid g)
        {
            Node currentNode;
            bool exists = false;
            for (LinkedListNode<Node> newIt = newNodes.First; newIt != null; newIt = newIt.Next)
            {
                currentNode = nodes[newIt.Value.x - g.OffsetX, newIt.Value.y - g.OffsetY];

                if (currentNode != null)
                {
                    exists = currentNode.listNumb == 1;

                    if (!exists)
                    {
                        exists = currentNode.listNumb == 2;
                        if (exists)
                        {
                            Node availableNode = nodes[currentNode.x - g.OffsetX, currentNode.y - g.OffsetY];
                            if (newIt.Value.F < availableNode.F)
                            {
                                //replacing the node
                                availableNodes.Remove(availableNode);
                                availableNodes.Enqueue(newIt.Value, newIt.Value.F);
                            }
                        }
                    }
                }
                else
                {
                    nodes[newIt.Value.x - g.OffsetX, newIt.Value.y - g.OffsetY] = newIt.Value;
                    newIt.Value.listNumb = 2;//2 for availableNodes.
                    availableNodes.Enqueue(newIt.Value, newIt.Value.F);
                }

                //if (currentNode == null || !exists)
                //{
                    
                //}
            }
            //newNodes.Clear();
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

        private static LinkedList<Node> AStart(IntVector2 start, IntVector2 end, int gridID)
        {
            return AStart(start, end, gridID, Epsilon);
        }

        private static LinkedList<Node> AStart(IntVector2 start, IntVector2 end, int gridID, float eps)
        {
            Grid grid = PlayerManager.GetGrid(gridID);
            
            if (grid == null || !grid.isGridEmpty(start))
                return new LinkedList<Node>();

            int height = grid.grid_height;
            int width = grid.grid_width;
            Node[,] nodeArray = new Node[width,height];

            Node startNode = new Node();
            startNode.x = start.x;
            startNode.y = start.y;
            LinkedList<Node> pathMap = new LinkedList<Node>();
            pathMap.AddFirst(startNode);
            FastPriorityQueue<Node> availableNodes = new FastPriorityQueue<Node>(width * height);
            LinkedList<Node> adjacents = getAdjacentNodes(nodeArray, grid, startNode, end, eps);

            foreach (Node n in adjacents)
            {
                availableNodes.Enqueue(n, n.F);
            }
            Node minNode;

            while (availableNodes.Count > 0)
            {
                minNode = availableNodes.First;
                availableNodes.Dequeue();
                Merge(nodeArray, getAdjacentNodes(nodeArray, grid, minNode, end, eps), ref pathMap, ref availableNodes, grid);

                minNode.listNumb = 1;
                nodeArray[minNode.x - grid.OffsetX, minNode.y  - grid.OffsetY] = minNode; //1 for pathMap
                pathMap.AddLast(minNode);

                if (minNode.x == end.x && minNode.y == end.y)
                {
                    return /*pathMap; //*/GetBestPath(pathMap);
                }
            }

            return new LinkedList<Node>();
        }

        /// <summary>
        /// Gets the best path within the grid of the given player id.
        /// </summary>
        /// <param name="gridID">The grid id which the path will be determined.</param>
        /// <param name="start">The starting point in the grid.</param>
        /// <param name="end">The end point in the grid.</param>
        /// <returns></returns>
        public static List<IntVector2> GetGridPath(int gridID, IntVector2 start, IntVector2 end)
        {
            LinkedList<Node> nodes = AStart(start, end, gridID);
            List<IntVector2> path = new List<IntVector2>(nodes.Count);
            for (LinkedListNode<Node> it = nodes.Last; it != null; it = it.Previous)
            {
                path.Add(new IntVector2(it.Value.x, it.Value.y));
            }
            return path;
        }

        /// <summary>
        /// Gets the partcile prefab name of the given modifier type. Returns an empty string if no prefab exists.
        /// </summary>
        /// <param name="type">The type of modifier whose particle prefab name will be returned.</param>
        /// <returns></returns>
        public static string GetModifierParticlePrefab(ModiferTypes type)
        {
            switch (type)
            {
                default: return string.Empty;
                case ModiferTypes.Fire: return "Flames";
                case ModiferTypes.Ice: return "Ice";
                case ModiferTypes.Acid: return "Acid";
            }
        }

        /// <summary>
        /// An event that runs whenever the resolution changes.
        /// </summary>
        public static event EventHandler onResolutionChange;

    }
}

