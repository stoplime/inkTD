using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class InkObject : MonoBehaviour {

	[Tooltip("The base damage value excluding all modifiers. Example 10")]
    public float damage;

    [Tooltip("The number of projectiles fired per minute. Example: 30")]
    public float speed;
    
    [Tooltip("The distance in all directions around the tower in world coordinates that can be fired upon. Example: 5")]
    public float range;

    [Tooltip("The cost to purchase this tower. Example 50")]
    public int price;

    /// <summary>
    /// This variable is no longer used after the tower has been created. 
    /// </summary>
    [Tooltip("The position along the playing grid's x axis.")]
    public int initialGridPositionX = 0;

    /// <summary>
    /// This variable is no longer used after the tower has been created.
    /// </summary>
    [Tooltip("The position along the playing grid's y axis.")]
    public int initialGridPositionY = 0;

    [Tooltip("The total lifetime in milliseconds the projectile will live.")]
    public float projectileLife = 1000;

    [Tooltip("The object that is shot when this tower attacks.")]
    public GameObject projectileObject;

    [Tooltip("The height at which projectiles are spawned.")]
    public float projectileSpawnHeight;

    [Tooltip("The sound effect that plays whenever a projectile is fired from this tower.")]
    public AudioClip shootSoundEffect;

    [Tooltip("The ID of the player which owns this tower.")]
    public int ownerID = 0;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
