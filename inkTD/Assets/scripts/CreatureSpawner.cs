using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour {

	public List<string> creatures;

	public void OnClick(int id)
	{
		PlayerManager.CreateCreature(id, creatures[Random.Range(0, creatures.Count)], gameObject);
		
	}

	// Use this for initialization
	void Start () {
		creatures = new List<string>();
		creatures.Add("Stickman_Creature");
		creatures.Add("Skeleton_Creature");
		creatures.Add("MushroomMan_Creature");
		creatures.Add("Ghost_Creature");
		creatures.Add("Ogre_Creature");
		creatures.Add("StickChimera_Creature");
		creatures.Add("BatteringRam_Creature");
		creatures.Add("Troll_Creature");
		creatures.Add("Frog_Creature");
		creatures.Add("MudGolem_Creature");
		creatures.Add("Banshee_Creature");
		creatures.Add("Griffon_Creature");
		creatures.Add("Beholder_Creature");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
