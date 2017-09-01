using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

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

    [Tooltip("The target the tower will attempt to fire at.")]
    public TargetTypes priorityTarget = TargetTypes.First;

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

    /// <summary>
    /// Gets or sets the target this tower is aiming at.
    /// </summary>
    public GameObject Target
    {
        get { return target; }
        set { SetTarget(value); }
    }

    /// <summary>
    /// Sets this tower's position along the playing grid x axis.
    /// </summary>
    public int GridPositionX
    {
        get { return gridPositionX; }
        set { SetTowerPosition(value, gridPositionY); }
    }

    /// <summary>
    /// Sets this tower's position along the playing grid's y axis.
    /// </summary>
    public int GridPositionY
    {
        get { return gridPositionY; }
        set { SetTowerPosition(gridPositionX, value); }
    }

    private GameObject target = null;
    private MeshRenderer targetRenderer = null;
    //TODO: Cache the creature script component of the target object, to check when health is gone the target is returned to null.

    private TaylorTimer timer;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    private int gridPositionX;
    private int gridPositionY;

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

    void Awake()
    {
        gridPositionX = initialGridPositionX;
        gridPositionY = initialGridPositionY;
    }
    
	// Use this for initialization
	void Start ()
    {
        timer = new TaylorTimer(60000 / speed);
        timer.Elapsed += Timer_Elapsed;

        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();

        SetTowerPosition(gridPositionX, gridPositionY);
	}

    void OnValidate()
    {
        SetTowerPosition(initialGridPositionX, initialGridPositionY);
    }

    private void SetTarget(GameObject target)
    {
        this.target = target;

        targetRenderer = target.GetComponent<MeshRenderer>();
    }

    private void Timer_Elapsed(object sender, System.EventArgs e)
    {
        Quaternion rotation = Quaternion.identity;
        Vector3 spawnPos = meshRenderer.bounds.center;
        spawnPos.y += projectileSpawnHeight;
        GameObject projectile = Instantiate(projectileObject, spawnPos, rotation) as GameObject;
        Projectile_Controller controller = projectile.GetComponent<Projectile_Controller>();
        controller.SetCreator(this);
        controller.Life = projectileLife;
        controller.Target = target;
        controller.StartPosition = spawnPos;

        if (targetRenderer == null)
        {
            controller.TargetPosition = target.transform.position;
        }
        else
        {
            controller.TargetPosition = targetRenderer.bounds.center;
        }

        projectile.transform.LookAt(target.transform);

        if (audioSource != null && shootSoundEffect != null)
        {
            //audioSource.PlayOneShot(shootSoundEffect, Help.TowerSoundEffectVolume);
            audioSource.PlayOneShot(shootSoundEffect);
        }
    }

    /// <summary>
    /// Aligns the tower to the given x and y within the playing grid.
    /// </summary>
    /// <param name="x">The x axis grid block number.</param>
    /// <param name="y">The y axis grid block number.</param>
    public void SetTowerPosition(int x, int y)
    {
        //empty the grid position at gridPositionX, gridPositionY
        Vector3 realPos = Grid.gridToPos(new IntVector2(x, y));
        transform.position = new Vector3(realPos.x, transform.position.y, realPos.z);
        gridPositionX = x;
        gridPositionY = y;
        //fill the grid position at x, y
        PlayerManager.SetGameObject(ownerID, gameObject, x, y);
    }

    // Update is called once per frame
    void Update ()
    {
        //DEBUG ONLY
        if (target == null)
        {
            SetTarget(GameObject.FindGameObjectWithTag("Debug"));
        }
        //end

		if (target != null)
        {
            timer.Update();
        }
	}
}
