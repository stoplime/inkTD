using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class Creature : InkObject
{
    [Tooltip("The increase inkcome when spawned. Example: 1, means the Inkcome will += 1 whenever this creature gets spawned.")]
    public int inkcomeValue;

	[Tooltip("How much ink is droped when killed. Ex: 2")]
    public int dropInk;

	[Tooltip("The start location of the creature's path")]
	public IntVector2 start;

	[Tooltip("The end location of the creature's path")]
	public IntVector2 end;

	
	private List<IntVector2> path;

	// follow A* path

	// animate creature

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
