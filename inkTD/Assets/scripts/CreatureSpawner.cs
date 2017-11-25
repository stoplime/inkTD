using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour {

	public Creatures creature;
    public static string path;

	public void OnClick(int id, Creatures type)
	{
		//PlayerManager.CreateCreature(id, creatures[Random.Range(0, creatures.Count)]);
        if(id == 0)
        {
            PlayerManager.CreateCreature(id, path);
        }
        else
        {
            PlayerManager.CreateCreature(id, type, true);
        }
	}

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
