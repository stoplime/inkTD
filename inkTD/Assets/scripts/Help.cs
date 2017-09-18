
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
        private class Node
        {
            public IntVector2 Location { get; set; }
            public float G { get; set; }
            public float H { get; set; }
            public float epsilon { get; set; }

            public float F;

            //public float F { get { return this.G + epsilon*this.H; } }
            public Node ParentNode { get; set; }
            public byte listNumb { get; set; }
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
        private static LinkedList<Node> getAdjacentNodes(byte[,] nodes, Grid playerGrid, Node currentNode, IntVector2 end, int playerID, float epsilon)
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
                if (playerGrid.inArena(offset) && playerGrid.isGridEmpty(offset))
                {
                    offsetNode = new Node();
                    offsetNode.ParentNode = currentNode;
                    offsetNode.epsilon = epsilon;
                    offsetNode.G = currentNode.G + 1;
                    offsetNode.H = Dist(offset, end);
                    offsetNode.F = offsetNode.G + epsilon * offsetNode.H;
                    //offsetNode.H = Math.Abs(end.x - offset.x) + Math.Abs(end.y - offset.y);
                    offsetNode.Location = offset;
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

        private static void Merge(byte[,] nodes, LinkedList<Node> newNodes, ref LinkedList<Node> pathMap, ref LinkedList<Node> availableNodes, Grid g)
        {
            int currentNode = 0;
            for (LinkedListNode<Node> newIt = newNodes.First; newIt != null; newIt = newIt.Next)
            {
                bool exists = false;
                currentNode = nodes[newIt.Value.Location.x - g.OffsetX, newIt.Value.Location.y - g.OffsetY];

                if (currentNode == 1)
                    exists = true;

                if (!exists)
                {
                    //for (LinkedListNode<Node> availIt = availableNodes.First; availIt != null; availIt = availIt.Next)
                    //{
                    //    if (availIt.Value.Location.Equals(newIt.Value.Location))
                    //    {
                    //        exists = true;
                    //        if (newIt.Value.F < availIt.Value.F)
                    //        {
                    //            availIt.Value = newIt.Value;
                    //        }
                    //        break;
                    //    }
                    //}
                    if (currentNode == 2)
                    {
                        exists = true;
                        LinkedListNode<Node> availIt;
                        for (availIt = availableNodes.First; availIt != null; availIt = availIt.Next)
                        {
                            if (newIt.Value.Location.x == availIt.Value.Location.x &&
                                newIt.Value.Location.y == availIt.Value.Location.y)
                            {
                                break;
                            }
                        }
                        if (newIt.Value.F < availIt.Value.F)
                        {
                            //replacing the node
                            availIt.Value = newIt.Value;
                        }
                    }
                }
                if (!exists)
                {
                    nodes[newIt.Value.Location.x - g.OffsetX, newIt.Value.Location.y - g.OffsetY] = 2; //2 for availableNodes.
                    availableNodes.AddLast(newIt.Value);
                }
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

        private static LinkedList<Node> AStart(IntVector2 start, IntVector2 end, int playerID)
        {
            return AStart(start, end, playerID, Epsilon);
        }

        private static LinkedList<Node> AStart(IntVector2 start, IntVector2 end, int playerID, float eps)
        {
            // if (!PlayerManager.GetGrid(playerID).inArena(start))
            // {
                MonoBehaviour.print("start x " + start.x.ToString());
                MonoBehaviour.print("start y " + start.y.ToString());
                MonoBehaviour.print("grid id " + playerID.ToString());
                MonoBehaviour.print("grid bl x" + PlayerManager.GetGrid(playerID).gridOffset.x.ToString());
                MonoBehaviour.print("grid bl y" + PlayerManager.GetGrid(playerID).gridOffset.y.ToString());
            // }
            if (!PlayerManager.GetGrid(playerID).isGridEmpty(start))
                return new LinkedList<Node>();

            Grid grid = PlayerManager.GetGrid(playerID);
            int height = grid.grid_height;
            int width = grid.grid_width;
            byte[,] nodeArray = new byte[width,height];

            Node startNode = new Node();
            startNode.Location = start;
            LinkedList<Node> pathMap = new LinkedList<Node>();
            pathMap.AddFirst(startNode);
            LinkedList<Node> availableNodes = getAdjacentNodes(nodeArray, grid, startNode, end, playerID, eps); //*/new LinkedList<Node>();
            LinkedListNode<Node> it;
            LinkedListNode<Node> minIt;
            Node minNode;

            while (availableNodes.Count > 0)
            {
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
                Merge(nodeArray, getAdjacentNodes(nodeArray, grid, minNode, end, playerID, eps), ref pathMap, ref availableNodes, grid);

                nodeArray[minNode.Location.x - grid.OffsetX, minNode.Location.y  - grid.OffsetY] = 1; //1 for pathMap
                pathMap.AddLast(minNode);

                if (minNode.Location.Equals(end))
                {
                    return /*pathMap; //*/GetBestPath(pathMap);
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
            for (LinkedListNode<Node> it = nodes.Last; it != null; it = it.Previous)
            {
                path.Add(it.Value.Location);
            }
            return path;
        }

        /// <summary>
        /// Creates a circular mesh.
        /// </summary>
        /// <param name="faces">The number of outter faces (edges).</param>
        /// <param name="range">The range of the mesh, the distance the circle extends outward.</param>
        /// <returns></returns>
        public static Mesh CreateCircularMesh(int faces, float range)
        {
            Mesh m = new Mesh();
            List<Vector3> vertices = new List<Vector3>(faces * 3);
            List<Vector3> normals = new List<Vector3>(vertices.Count);
            float angle = Mathf.PI * 2;
            float step = angle / faces;
            Vector3 B;
            Vector3 C;
            Vector3 crossProduct;
            for (int i = 0; i < faces; i++)
            {
                vertices.Add(Vector3.zero);

                B = Vector3.zero;
                B.x = range * Mathf.Cos(angle);
                B.z = range * Mathf.Sin(angle);
                vertices.Add(B);

                if (i == faces - 1)
                {
                    angle = 0f;
                }
                else
                {
                    angle -= step;
                }
                C = Vector3.zero;
                C.x = range * Mathf.Cos(angle);
                C.z = range * Mathf.Sin(angle);
                vertices.Add(C);

                crossProduct = Vector3.Cross(B, C);
                normals.Add(crossProduct);
                normals.Add(crossProduct);
                normals.Add(crossProduct);
            }
            m.SetVertices(vertices);

            int[] triangleArray = new int[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
                triangleArray[i] = i;

            m.SetTriangles(triangleArray, 0);
            m.SetNormals(normals);

            m.name = "Circle {" + faces + "} R " + range ;
            return m;
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

