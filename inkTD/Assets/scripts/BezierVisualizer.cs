using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class BezierVisualizer : MonoBehaviour
{

    [Tooltip("The 'resolution' of the curve, or the number of points along the curve that are sampled.")]
    public int samples = 20;

    [Tooltip("If true the points of the bezier curve are moved relatively along with the object this script is attached to.")]
    public bool snapToObject = false;

    [Tooltip("The various main points on the bezier curve.")]
    public Vector3[] points;

    private Vector3[] realPoints;
    
    // Use this for initialization
    void Start()
    {
        if (points != null)
        {
            realPoints = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                realPoints[i] = points[i];
            }
        }
    }

    void OnValidate()
    {
        
    }

    private void AlignPoints()
    {

    }

    void OnDrawGizmos()
    {
        if (points != null && points.Length > 2)
        {
            if (realPoints == null || realPoints.Length != points.Length)
            {
                realPoints = new Vector3[points.Length];
            }
            if (snapToObject)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    realPoints[i] = points[i] + transform.position;
                }
            }
            else
            {
                for (int i = 0; i < points.Length; i++)
                {
                    realPoints[i] = points[i];
                }
            }
        }

        if (realPoints != null && realPoints.Length > 2)
        {
            Vector3 previous;
            Vector3 next;
            float time = 0;
            for (int i = 0; i < samples; i++)
            {
                previous = Help.ComputeBezier(time, realPoints);
                time += 1f / samples;
                next = Help.ComputeBezier(time, realPoints);
                Gizmos.DrawLine(previous, next);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
