using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class InkObject : MonoBehaviour
{
    [Tooltip("The maximum health the creature can have.")]
    public float maxHealth;

    [Tooltip("The health of the tower or creature.")]
    public float health;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if (value > maxHealth)
            {
                health = maxHealth;
            }
            else if (value < 0)
            {
                health = 0;
            }
            else
            {
                health = value;
            }
        }
    }

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
    [TextArea(3, 10)]
    [Tooltip("The short description of the tower or creature. Use a tag to replace it with the variable desired upon start. Tags include: [p] for price, [s] for speed, [d] for damage, and [h] for health.")]
    public string description = "";

    /// <summary>
    /// The list of currently active modifiers on the tower or creature.
    /// </summary>
    public List<Modifier> Modifiers { get { return modifiers; } }

    /// <summary>
    /// Gets the position of the creature ot tower on the grid.
    /// </summary>
    public IntVector2 GridPosition { get { return gridPos; } }

    /// <summary>
    /// The list of currently active modifiers on the tower or creature.
    /// </summary>
    [Tooltip("The list of modifiers applied to the tower or creature.")]
    private List<Modifier> modifiers = new List<Modifier>();
    
    protected IntVector2 gridPos;
    protected Vector3 pos = Vector3.zero;

    // global rotation along the y-axis
    protected float rotation;

    private int prevID = 0;

    public virtual void Start()
    {
        description = description.Replace("[p]", price.ToString());
        description = description.Replace("[s]", speed.ToString());
        description = description.Replace("[d]", damage.ToString());
        description = description.Replace("[h]", health.ToString());
    }

    public virtual void OnValidate()
    {
        if (prevID != ownerID)
        {
            OnOwnerChange();
        }
        prevID = ownerID;
    }

    /// <summary>
    /// A method that runs when the owner of the ink object changes.
    /// </summary>
    protected virtual void OnOwnerChange()
    {

    }
}
