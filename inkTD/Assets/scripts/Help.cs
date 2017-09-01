
using System;
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

        /// <summary>
        /// An event that runs whenever the resolution changes.
        /// </summary>
        public static event EventHandler onResolutionChange;

    }
}

