﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar_Handler : MonoBehaviour
{

    public float cameraScreenPercentage = 0.95f;

    private RectTransform toolbarRectangle;

    void Awake()
    {
        toolbarRectangle = GetComponent<RectTransform>();
        Align();
        helper.Help.onResolutionChange += onResolutionChange;

        GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
        foreach (GameObject g in grids)
        {
            Grid grid = g.GetComponent<Grid>();
            PlayerManager.AddGrid(grid.ID, grid);
        }
    }

    private void onResolutionChange(object sender, System.EventArgs e)
    {
        Align();
    }

    /// <summary>
    /// Aligns the toolbar by resizing it to fit the missing region outside the camera as well as the UI elements inside.
    /// </summary>
    private void Align()
    {
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
