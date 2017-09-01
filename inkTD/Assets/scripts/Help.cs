
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

        private class Node
        {
            public IntVector2 Location { get; set; }
            public float G { get; set; }
            public float H { get; set; }
            public float F { get { return this.G + this.H; } }
            public Node ParentNode { get; set; }
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
                    //if (availableNodes.Count > 0)
                        availableNodes.AddLast(newIt.Value);
                    //else
                        //availableNodes.AddFirst(newIt);
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
            LinkedList<Node> availableNodes = getAdjacentNodes(startNode, end, playerID);

            while (availableNodes.Count > 0)
            {
                LinkedListNode<Node> it = availableNodes.First;
                LinkedListNode<Node> minIt = availableNodes.First;
                float minF = it.Value.F;
                it = it.Next;
                while (it != null)
                {
                    if (minF > it.Value.F)
                    {
                        minF = it.Value.F;
                        minIt = it;
                    }
                    it = it.Next;
                }
                Merge(getAdjacentNodes(minIt.Value, end, playerID), ref pathMap, ref availableNodes);
                pathMap.AddLast(minIt.Value);

                if (minIt.Value.Location.Equals(end))
                {
                    return GetBestPath(pathMap);
                }
                availableNodes.Remove(minIt);
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

