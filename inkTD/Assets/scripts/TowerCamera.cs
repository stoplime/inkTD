using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCamera : MonoBehaviour
{
    public static Tower selected;
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void MoveCamera()
    {
        Debug.Log(selected.objName);
        transform.position = new Vector3(selected.gameObject.transform.position.x, selected.gameObject.transform.position.y + 8, selected.gameObject.transform.position.z - 5);
    }

    public void MoveCamera(Tower focus)
    {
        Debug.Log(focus.objName);
        transform.position = new Vector3(focus.gameObject.transform.position.x, focus.gameObject.transform.position.y + 8, focus.gameObject.transform.position.z - 5);
    }
}
