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
    /// ID for the grid parent.
    /// For towers, the id represent which grid it's on.
    /// Creatures, the ID represent which grid it's not on.
    /// Ex. creature id is the player, then the creature is spawned on all grids that's not the players.
    /// </summary>
    [Tooltip("The ID of the player which owns this tower/creature.")]
    public int ownerID = 0;

    /// <summary>
    /// The description for the tower or creature.
    /// </summary>
    [Tooltip("The short description of the tower or creature.")]
    public string description = "";

    /// <summary>
    /// The list of currently active modifiers on the tower or creature.
    /// </summary>
    public List<Modifier> Modifiers { get { return modifiers; } }

    /// <summary>
    /// The list of currently active modifiers on the tower or creature.
    /// </summary>
    [Tooltip("The list of modifiers applied to the tower or creature.")]
    private List<Modifier> modifiers = new List<Modifier>();

    protected int maxHealth;
    protected int health;
    
    protected IntVector2 gridPos;
    protected Vector3 pos;

    // global rotation along the y-axis
    protected float rotation;
    
    

}
