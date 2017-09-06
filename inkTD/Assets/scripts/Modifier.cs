/// <summary>
/// The different types of modifiers.
/// </summary>
public enum ModiferTypes
{
    /// <summary>
    /// No modifier type, resulting in no effects.
    /// </summary>
    None = 0,

    /// <summary>
    /// Towers with a fire modifier deal fire elemental damage. Creatures become resistant to fire elemental damage.
    /// </summary>
    Fire = 1,

    /// <summary>
    /// Towers with an ice modifier deal ice elemental damage. Creatures become resistant to ice elemental damage.
    /// </summary>
    Ice = 2,
    
    /// <summary>
    /// Towers with an acid modifier deal acid elemental damage. Creatures become resistant to acid elemental damage.
    /// </summary>
    Acid = 3,

    /// <summary>
    /// Towers with a lightning modifier deal lightning elemental damage. Creatures become restistant to lightning damage.
    /// </summary>
    Lightning = 4,

    /// <summary>
    /// Towers with a speed up modifier shoot faster, while creatures move faster.
    /// </summary>
    SpeedUp = 5,

    /// <summary>
    /// Towers with a damage up modifier deal more damage per projectile, while creatures deal more damage to the tower-castle.
    /// </summary>
    DamageUp = 6,

    /// <summary>
    /// Towers with a radius up modifier can target enemies further away.
    /// </summary>
    RadiusUp = 7,

    /// <summary>
    /// Towers with a slow modifier apply a slow modifier to creatures damaged by their projectiles, while creatures move slower.
    /// </summary>
    Slow = 8,

    /// <summary>
    /// Towers with a stun modifier have a chance of halting a creature, while creatures with a stun modifier cannot move.
    /// </summary>
    Stun = 9,

    /// <summary>
    /// Towers with a piercing modifier shoot through creature defences more easily.
    /// </summary>
    Piercing = 10,

    /// <summary>
    /// Towers with a chain modifier have their attacks jump to the nearest target.
    /// </summary>
    Chain = 11,

    /// <summary>
    /// Towers with a flying modifier float some distance in the air, while creatures can fly over certain towers.
    /// </summary>
    Flying = 12,

    /// <summary>
    /// Towers with an ethereal modifier can attack creatures also with an etheral modifier, while creatures become immune to any attacks from towers without an ethereal modifier.
    /// </summary>
    Ethereal = 13,

    /// <summary>
    /// Creatures have a chance to not be affected by a projectile.
    /// </summary>
    Evasion = 14,
}

/// <summary>
/// A structure for holding modifier related data.
/// </summary>
public struct Modifier
{
    /// <summary>
    /// The type of modifier.
    /// </summary>
    public ModiferTypes type;

    /// <summary>
    /// The tier the modifier is currently at.
    /// </summary>
    public ushort tier;

    /// <summary>
    /// Creates a new instance of a modifier with a type and tier.
    /// </summary>
    /// <param name="type">The type of modifier this will be.</param>
    /// <param name="tier">The tier/power of the modifier.</param>
    public Modifier(ModiferTypes type, ushort tier)
    {
        this.type = type;
        this.tier = tier;
    }

}
