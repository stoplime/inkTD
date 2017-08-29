using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    [Tooltip("The base damage value excluding all modifiers. Example 10")]
    public float damage;

    [Tooltip("The number of projectiles fired per minute. Example: 30")]
    public float speed;

    [Tooltip("The distance in all directions around the tower in world coordinates that can be fired upon. Example: 5")]
    public float range;

    [Tooltip("The cost to purchase this tower. Example 50")]
    public int price;

    [Tooltip("The target the tower will attempt to fire at.")]
    public TargetTypes priorityTarget = TargetTypes.First;

    /// <summary>
    /// A list of targets a tower can target.
    /// </summary>
    public enum TargetTypes
    {
        First = 0,
        Last = 1,
        LowestLife = 2,
        HighestLife = 3,
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
