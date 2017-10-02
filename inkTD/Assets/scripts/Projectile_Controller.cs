﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

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
    /// The position the projectile is heading to.
    /// </summary>
    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set
        {
            targetPosition = value;
            curveEnd = targetPosition;
        }
    }

    /// <summary>
    /// The number of milliseconds the projectile will take to reach its target position and self destruct.
    /// </summary>
    public float Life
    {
        get { return life; }
        set { life = value / 1000f; }
    }

    public float Damage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the area of effect of the projectile. 0 denotes no AOE.
    /// </summary>
    public float AOERadius
    {
        get { return areaEffectRadius; }
        set
        {
            areaEffectRadius = value;
            CorrectColliderRadius();
        }
    }

    /// <summary>
    /// If true the projectile will track the target object.
    /// </summary>
    [Tooltip("IF true, the projectile will track the target object.")]
    public bool trackingProjectile = true;
    
    private GameObject target;
    
    private Vector3 targetPosition;
    private float life = 1;
    private float currentLife;
    private float areaEffectRadius = 0f;

    private Tower creator;
    
    public Vector3 curveStart;
    public Vector3 curveMid;
    public Vector3 curveEnd;
    private Vector3 currentBezierCurve;

    private SphereCollider collider;

    // Use this for initialization
    void Start ()
    {
        currentBezierCurve = Help.ComputeBezier((currentLife + Time.deltaTime) / life, curveStart, curveMid, curveEnd);
        transform.LookAt(currentBezierCurve);
        
        CorrectColliderRadius();
    }

    private void CorrectColliderRadius()
    {
        if (collider == null)
        {
            collider = GetComponent<SphereCollider>();
        }

        collider.radius = (areaEffectRadius / ((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3)) * 0.80f; //For some reason sphere collider radii are off by 20%.
    }

    /// <summary>
    /// Sets the points the projectile will curve along.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="mid">The middle of the bezier curve.</param>
    /// <param name="end">The end of the bezier curve.</param>
    public void SetCurvePoints(Vector3 start, Vector3 mid, Vector3 end)
    {
        curveStart = start;
        curveMid = mid;
        curveEnd = end;
        currentBezierCurve = Help.ComputeBezier((currentLife + Time.deltaTime) / life, curveStart, curveMid, curveEnd);
        transform.LookAt(currentBezierCurve);
        ComputeMovement();
    }

    /// <summary>
    /// Sets the tower which created this projectile.
    /// </summary>
    /// <param name="t">The tower which created this projectile.</param>
    public void SetCreator(Tower t)
    {
        creator = t;
    }

    private void ComputeMovement()
    {
        if (trackingProjectile)
        {
            transform.position = Help.ComputeBezier(currentLife / life, curveStart, curveMid, curveEnd);
            if (target != null)
            {
                transform.LookAt(target.transform);
            }
            else if (currentLife != life)
            {
                transform.LookAt(Help.ComputeBezier((currentLife+1) / life, curveStart, curveMid, curveEnd));
            }
        }
        else
        {
            currentBezierCurve = Help.ComputeBezier(currentLife / life, curveStart, curveMid, curveEnd);
            transform.LookAt(currentBezierCurve);
            transform.position = currentBezierCurve;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentLife += Time.deltaTime;
        ComputeMovement();

        if (currentLife > life)
        {
            //apply damage to target here.
            if (target != null)
            {
                if (AOERadius != 0f)
                {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, areaEffectRadius);

                    foreach (Collider c in colliders)
                    {
                        if (c.attachedRigidbody != null)
                        {
                            Creature creature = c.attachedRigidbody.gameObject.GetComponent<Creature>();
                            if (creature != null)
                            {
                                creature.TakeDamage(Damage);
                            }
                        }
                    }
                }
                else
                {
                    target.GetComponent<Creature>().TakeDamage(Damage);
                }
            }
            Destroy(gameObject);
        }
    }
}
