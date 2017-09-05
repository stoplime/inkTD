using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public abstract class InkObject : MonoBehaviour
{

    /// <summary>
    /// Creatures have increased damage done to th tower-castle 
    /// Towers have increased projectile damage
    /// </summary>
	[Tooltip("The base damage value excluding all modifiers. Example 10")]
    public float damage;

    /// <summary>
    /// Creatues have faster speed traversing the map
    /// Towers have faster reloading of projectiles
    /// </summary>
    [Tooltip("The number of projectiles fired per minute. Example: 30")]
    public float speed;

    /// <summary>
    /// Ink cost for both towers and creatures
    /// </summary>
    [Tooltip("The cost to purchase this tower. Example 50")]
    public int price;

    /// <summary>
    /// This variable is no longer used after the tower/creature has been created. 
    /// </summary>
    [Tooltip("The position along the playing grid's x axis.")]
    public int initialGridPositionX = 0;

    /// <summary>
    /// This variable is no longer used after the tower/creature has been created.
    /// </summary>
    [Tooltip("The position along the playing grid's y axis.")]
    public int initialGridPositionY = 0;

    /// <summary>
    /// ID for the grid parent.
    /// For towers, the id represent which grid it's on.
    /// Creatures, the ID represent which grid it's not on.
    /// Ex. creature id is the player, then the creature is spawned on all grids that's not the players.
    /// </summary>
    [Tooltip("The ID of the player which owns this tower/creature.")]
    public int ownerID = 0;


    protected int maxHealth;
    protected int health;
    
    protected float speed;
    protected IntVector2 gridPos;
    protected Vector3 pos;

    // global rotation along the y-axis
    protected float rotation;
    
    

}
