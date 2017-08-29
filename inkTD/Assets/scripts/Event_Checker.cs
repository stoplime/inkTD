using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that handles events and other miscellanous code during the start call at the beginning of the game and updating throughout the game.
/// </summary>
public class Event_Checker : MonoBehaviour
{

    private Resolution prevResolution = new Resolution();

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (prevResolution.width != Screen.currentResolution.width && prevResolution.height != Screen.currentResolution.height)
        {
            helper.Help.TriggerResolutionChangeEvent();
        }
	}
}
