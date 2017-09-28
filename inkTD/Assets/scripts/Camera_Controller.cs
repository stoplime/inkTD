using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class Camera_Controller : MonoBehaviour
{
    public float panSpeed = 20;
    public float zoomSpeed = 50;

    private Transform cameraTransform;
    private Vector3 position;
    private Vector3 zoomPosition;

    public float totalZoom;

    private float defaultAngle;

    public Vector3[] bezierPoints;

    private bool isOnCurve = false;

    private BezierVisualizer bVisualizer;

    // Use this for initialization
    void Start ()
    {
        totalZoom = -0.2f;
        cameraTransform = GetComponent<Transform>();
        position = cameraTransform.position;
        bezierPoints = new Vector3[] { new Vector3(0, 0, 0), transform.rotation * Vector3.forward * 6, new Vector3(0, -12, 14), new Vector3(0, -12, 20)};
        defaultAngle = transform.eulerAngles.x;
        bVisualizer = GetComponent<BezierVisualizer>();
        if (bVisualizer != null)
        {
            bVisualizer.points = bezierPoints;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
		//Arrow keys for X and Z movement
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            position.z += panSpeed * Time.deltaTime;
            cameraTransform.position = position;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            position.z -= panSpeed * Time.deltaTime;
            cameraTransform.position = position;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            position.x -= panSpeed * Time.deltaTime;
            cameraTransform.position = position;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            position.x += panSpeed * Time.deltaTime;
            cameraTransform.position = position;
        }

        //Mouse Wheel for Y and Z motion
        totalZoom += Time.deltaTime * zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        if (totalZoom > 1)
        {
            totalZoom = 1;
        }
        else if (totalZoom < -3)
        {
            totalZoom = -3;
        }

        if (isOnCurve)
        {
            //do on curve stuff
            float smallDelta = 0.00001f;
            Vector3 target = Help.ComputeBezier(totalZoom, bezierPoints);
            Vector3 targetLook = Help.ComputeBezier(totalZoom+smallDelta, bezierPoints);
            target.x = 0;
            cameraTransform.position = position + target;
            transform.LookAt(targetLook + position);

            if (totalZoom <= 0)
            {
                isOnCurve = false;
                transform.rotation = Quaternion.Euler(defaultAngle, 0, 0);
                //etc
            }
        } 
        else
        {
            //Do off curve stuff
            zoomPosition = transform.forward * totalZoom * zoomSpeed;
            transform.position = position + zoomPosition;

            if (totalZoom > 0)
            {
                isOnCurve = true;
                //etc
            }

        }
	}

    /// <summary>
    /// A method used to calculate the Bezier curve from the camera's position
    /// </summary>
    Vector3 ComputeBezierCurve(float time)
    {
        float inverseTime = 1 - time;
        return ((inverseTime * inverseTime * inverseTime) * bezierPoints[0]) + ((3 * time * inverseTime * inverseTime) * bezierPoints[1]) + ((3 * time * time * inverseTime) * bezierPoints[2]) + ((time * time * time) * bezierPoints[3]);
    }

}
