using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar_Handler : MonoBehaviour
{

    public float cameraScreenPercentage = 0.95f;

    private RectTransform toolbarRectangle;

    void Awake()
    {
        toolbarRectangle = GetComponent<RectTransform>();
        toolbarRectangle.sizeDelta = new Vector2(toolbarRectangle.sizeDelta.x, Screen.height * (1 - cameraScreenPercentage));
    }

	// Use this for initialization
	//void Start ()
    //{
		
	//}
	
	// Update is called once per frame
	//void Update ()
    //{
		
	//}
}
