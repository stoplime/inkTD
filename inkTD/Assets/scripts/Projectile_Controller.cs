using System.Collections;
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
    /// If true the projectile will track the target object.
    /// </summary>
    [Tooltip("IF true, the projectile will track the target object.")]
    public bool trackingProjectile = true;
    
    private GameObject target;
    
    private Vector3 targetPosition;
    private float life = 1;
    private float currentLife;

    private Tower creator;
    
    public Vector3 curveStart;
    public Vector3 curveMid;
    public Vector3 curveEnd;
    private Vector3 currentBezierCurve;

    // Use this for initialization
    void Start ()
    {
        currentBezierCurve = Help.ComputeBezier((currentLife + Time.deltaTime) / life, curveStart, curveMid, curveEnd);
        transform.LookAt(currentBezierCurve);
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
            transform.LookAt(target.transform);
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
            target.GetComponent<Creature>().TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
}
