using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCamera : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void MoveCamera(Tower focus)
    {
        transform.position = new Vector3(focus.gameObject.transform.position.x - 5, focus.gameObject.transform.position.y + 5, focus.gameObject.transform.position.z);
    }
}
