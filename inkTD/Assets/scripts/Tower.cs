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

    [Tooltip("The total lifetime in milliseconds the projectile will live.")]
    public float projectileLife = 1000;

    [Tooltip("The object that is shot when this tower attacks.")]
    public GameObject projectileObject;

    [Tooltip("The height at which projectiles are spawned.")]
    public float projectileSpawnHeight;

    /// <summary>
    /// Gets or sets the target this tower is aiming at.
    /// </summary>
    public GameObject Target
    {
        get { return target; }
        set { SetTarget(value); }
    }

    private GameObject target = null;
    //TODO: Cache the creature script component of the target object, to check when health is gone the target is returned to null.

    private TaylorTimer timer;
    private MeshRenderer meshRenderer;

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
        timer = new TaylorTimer(60000 / speed);
        timer.Elapsed += Timer_Elapsed;

        meshRenderer = GetComponent<MeshRenderer>();
	}
    private void SetTarget(GameObject target)
    {
        this.target = target;
    }

    private void Timer_Elapsed(object sender, System.EventArgs e)
    {
        Quaternion rotation = Quaternion.identity;
        Vector3 spawnPos = meshRenderer.bounds.center;
        spawnPos.y += projectileSpawnHeight;
        GameObject projectile = Instantiate(projectileObject, spawnPos, rotation) as GameObject;
        Projectile_Controller controller = projectile.GetComponent<Projectile_Controller>();
        controller.SetCreator(this);
        controller.StartPosition = spawnPos;
        controller.TargetPosition = target.transform.position;
        controller.Life = projectileLife;
        projectile.transform.LookAt(target.transform);
    }

    // Update is called once per frame
    void Update ()
    {
        //TEST
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Debug");
        }
        //END TEST

		if (target != null)
        {
            timer.Update();
        }
	}
}
