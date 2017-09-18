using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{

    public float speed = 1;

    private Transform cameraTransform;
    private Vector3 position;

    private float zoom;

    private float defaultAngle;

    public Vector3[] bezierPoints;

    private float i = 0.0f;

    // Use this for initialization
    void Start ()
    {
        cameraTransform = GetComponent<Transform>();
        position = cameraTransform.position;
        bezierPoints = new Vector3[] { new Vector3(0, -3, 2), new Vector3(0, -3, 4), new Vector3(0, -3, 6), new Vector3(0, -3, 8)};
        defaultAngle = 37.59f;
    }
	
	// Update is called once per frame
	void Update ()
    {
		//Arrow keys for X and Z movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            position.z += speed * Time.timeScale;
            cameraTransform.position = position;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            position.z -= speed * Time.timeScale;
            cameraTransform.position = position;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            position.x -= speed * Time.timeScale;
            cameraTransform.position = position;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            position.x += speed * Time.timeScale;
            cameraTransform.position = position;
        }

        //Mouse Wheel for Y and Z motion
        zoom = Input.GetAxis("Mouse ScrollWheel");
        
        if(transform.position.y >= 15)
        {
            transform.rotation = Quaternion.Euler(defaultAngle, 0, 0);
            if (zoom > 0f)
            {
                transform.Translate(Vector3.forward * Time.timeScale * 5);
                position = cameraTransform.position;
            }
            else if (zoom < 0f)
            {
                transform.Translate(-Vector3.forward * Time.timeScale * 5);
                position = cameraTransform.position;
            }
        }
        else
        {
            if (i <= 1f && i >= 0f)
            {
                if (zoom > 0f)
                {
                    i += 0.25f;
                    Vector3 target = new Vector3(0, ComputeBezierCurve(i).y, ComputeBezierCurve(i).z);
                    position += target;
                    transform.Rotate(Vector3.right, -9.25f);
                    cameraTransform.position = position;
                }
                else if (zoom < 0f)
                {
                    i -= 0.25f;
                    Vector3 target = new Vector3(0, ComputeBezierCurve(i).y, ComputeBezierCurve(i).z);
                    position -= target;
                    transform.Rotate(Vector3.right, 9.25f);
                    cameraTransform.position = position;
                }

            }

            if (i > 1)
            {
                i = 1;
            }

            if (i < 0)
            {
                i = 0;
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
