using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class BezierVisualizer : MonoBehaviour
{

    [Tooltip("The 'resolution' of the curve, or the number of points along the curve that are sampled.")]
    public int samples = 20;

    [Tooltip("The various main points on the bezier curve.")]
    public Vector3[] points;

    // Use this for initialization
    void Start()
    {

    }

    void OnDrawGizmos()
    {
        Vector3 previous;
        Vector3 next;
        float time = 0;
        for (int i = 0; i < samples; i++)
        {
            previous = Help.ComputeBezier(time, points);
            time += 1f / samples;
            next = Help.ComputeBezier(time, points);
            Gizmos.DrawLine(previous,next);
        }
    }

    // Update is called once per frame
 //   void Update ()
 //   {
		
	//}
}
