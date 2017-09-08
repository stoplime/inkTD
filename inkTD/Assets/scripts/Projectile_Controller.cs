using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Controller : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the target this projectile will apply damage to.
    /// </summary>
    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }

    /// <summary>
    /// The position the projectile begins at.
    /// </summary>
    public Vector3 StartPosition
    {
        get { return startPosition; }
        set { startPosition = value; }
    }

    /// <summary>
    /// The position the projectile is heading to.
    /// </summary>
    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; }
    }

    /// <summary>
    /// The number of milliseconds the projectile will take to reach its target position and self destruct.
    /// </summary>
    public float Life
    {
        get { return life; }
        set { life = value / 1000f; }
    }

    /// <summary>
    /// If true the projectile will track the target object.
    /// </summary>
    [Tooltip("IF true, the projectile will track the target object.")]
    public bool trackingProjectile = true;
    
    private GameObject target;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float life = 1;
    private float currentLife;

    private Tower creator;

	// Use this for initialization
	void Start ()
    {
	}

    /// <summary>
    /// Sets the tower which created this projectile.
    /// </summary>
    /// <param name="t">The tower which created this projectile.</param>
    public void SetCreator(Tower t)
    {
        creator = t;
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentLife += Time.deltaTime;
        if (trackingProjectile)
        {
            Vector3[] points = {startPosition, new Vector3((startPosition.x+target.transform.position.x)/2, startPosition.y+2, (startPosition.z+target.transform.position.x)/2), target.transform.position};
            transform.position = helper.Help.ComputeBezier(points, currentLife / life);
            transform.LookAt(target.transform);
        }
        else
        {
            transform.position = helper.Help.ComputeBezier(startPosition, targetPosition, currentLife / life);
        }

        if (currentLife > life)
        {
            //apply damage to target here.

            Destroy(gameObject);
        }
    }
}
