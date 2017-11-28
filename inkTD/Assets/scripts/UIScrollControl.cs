using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScrollControl : MonoBehaviour, IScrollHandler
{

    public Scrollbar scrollBar;

    public float sensitivity = 0.5f;

    public void OnScroll(PointerEventData eventData)
    {
        if (scrollBar != null)
            scrollBar.value += eventData.scrollDelta.y * sensitivity;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
