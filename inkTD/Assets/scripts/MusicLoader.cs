using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class MusicLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<AudioSource>().volume = Help.MusicVolume;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
