using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour {

	public void OnClick()
	{
		PlayerManager.CreateCreature(0, transform, "DebugCreature");
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
