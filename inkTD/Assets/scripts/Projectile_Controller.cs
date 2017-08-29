using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Controller : MonoBehaviour
{
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


    private Vector3 startPosition;
    private Vector3 targetPosition; //Note if we want projectiles to track we simply store a target game object and use its position from 
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
        transform.position = helper.Help.ComputeBezier(startPosition, targetPosition,  currentLife / life);

        if (currentLife > life)
            Destroy(gameObject);
    }
}
