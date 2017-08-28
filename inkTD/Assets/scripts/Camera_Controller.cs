using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{

    public float speed = 1;

    private Transform cameraTransform;
    private Vector3 position;

	// Use this for initialization
	void Start ()
    {
        cameraTransform = GetComponent<Transform>();
        position = cameraTransform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
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

	}
}
