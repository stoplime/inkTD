using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that handles events and other miscellanous code during the start call at the beginning of the game and updating throughout the game.
/// </summary>
public class Event_Checker : MonoBehaviour
{

    private float prevWidth = 0f;
    private float prevHeight = 0f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (prevWidth != Screen.width || prevHeight != Screen.height)
        {
            helper.Help.TriggerResolutionChangeEvent();
            prevWidth = Screen.width;
            prevHeight = Screen.height;
        }
	}
}
