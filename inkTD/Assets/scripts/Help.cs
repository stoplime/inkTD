
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
        
       
        private static RectTransform blankButtonRect = null;

        private static GameObject blankButton = null;

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
        /// An event that runs whenever the resolution changes.
        /// </summary>
        public static event EventHandler onResolutionChange;

    }
}

