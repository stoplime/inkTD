﻿using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A script for handling multiple buttons along anchor points. This script must be placed inside of a canvas, buttons within that canvas with this script will act as a single system.
/// </summary>
public class Tab_Handler : MonoBehaviour
{

    //Editor Modifiable Values:
    public float spaceBetweenButtons = 15f;

    [Tooltip("The script of the menu that this tab corresponds to. Drag the GameObject with the script attached into this spot.")]
    public TabMenu menuScript;


    //Properties:

    /// <summary>
    /// Gets whether the button this script is attached to is being dragged or not.
    /// </summary>
    public bool BeingDragged { get { return beingDragged; } }

    /// <summary>
    /// Gets the face of the screen the button is attached to.
    /// </summary>
    public UIAnchors Anchor { get { return anchor; } }
    
    /// <summary>
    /// Gets or sets the offset from the top of the screen for top aligned tabs.
    /// </summary>
    public float OffsetTop
    {
        get { return offsetTop; }
        set
        {
            offsetTop = value;
            AlignTabs(UIAnchors.Top);
        }
    }

    /// <summary>
    /// Gets or sets the offset from the bottom of the screen for bottom aligned tabs.
    /// </summary>
    public float OffsetBottom
    {
        get { return offsetBottom; }
        set
        {
            offsetBottom = value;
            AlignTabs(UIAnchors.Bottom);
        }
    }

    /// <summary>
    /// Gets or sets the offset from the right of the screen for right aligned tabs.
    /// </summary>
    public float OffsetRight
    {
        get { return offsetRight; }
        set
        {
            offsetRight = value;
            AlignTabs(UIAnchors.Right);
        }
    }

    /// <summary>
    /// Gets or sets the offset from the left of the screen for left aligned tabs.
    /// </summary>
    public float OffsetLeft
    {
        get { return offsetLeft; }
        set
        {
            offsetLeft = value;
            AlignTabs(UIAnchors.Left);
        }
    }
    
    private float ScreenHeight { get { return canvasRect.rect.height - toolbarRect.rect.height; } }

    /// <summary>
    /// The transform used to control the placement of the button.
    /// </summary>
    private RectTransform buttonTransform;

    /// <summary>
    /// Chances are this is the panel or UI Menu that this UI element is nested in.
    /// </summary>
    private GameObject parentObject;
    private RectTransform canvasRect;
    private RectTransform toolbarRect;

    private Tab_Handler[] buttons;
    private RectTransform[] buttonTransforms;

    /// <summary>
    /// The current side of the screen this button is at.
    /// </summary>
    private UIAnchors anchor = UIAnchors.Right;
    private bool beingDragged = false;

    private static float offsetTop = 0f;
    private static float offsetBottom = 0f;
    private static float offsetLeft = 0f;
    private static float offsetRight = 0f;

    private float dragOffsetX = 0f;
    private float dragOffsetY = 0f;

	// Use this for initialization
	void Start ()
    {
        //Gathering information about this object and the canvas it acts within.
        buttonTransform = GetComponent<RectTransform>();
        parentObject = transform.parent.gameObject;
        canvasRect = parentObject.GetComponent<RectTransform>();
        buttons = parentObject.GetComponentsInChildren<Tab_Handler>();
        toolbarRect = GameObject.FindGameObjectWithTag("ToolbarPanel").GetComponent<RectTransform>();

        //Gathering the transforms of the other buttons.
        buttonTransforms = new RectTransform[buttons.Length];
        for (int i = 0; i < buttonTransforms.Length; i++)
        {
            buttonTransforms[i] = buttons[i].gameObject.GetComponent<RectTransform>();
        }

        AlignTabs(UIAnchors.Right);

        Help.onResolutionChange += onResolutionChange;

        if (Help.BlankButton == null)
        {
            Help.SetBlankButton(Instantiate(Resources.Load("Misc Prefabs/Blank_Tab")) as GameObject);
        }
	}

    private void onResolutionChange(object sender, System.EventArgs e)
    {
        AlignTabs(UIAnchors.Bottom);
        AlignTabs(UIAnchors.Left);
        AlignTabs(UIAnchors.Right);
        AlignTabs(UIAnchors.Top);
    }

    /// <summary>
    /// A method for handling the start of drag events.
    /// </summary>
    public void OnBeginDrag()
    {
        beingDragged = true;

        if (anchor == UIAnchors.Bottom || anchor == UIAnchors.Top)
        {
            dragOffsetX = Input.mousePosition.x - buttonTransform.anchoredPosition.x;
            dragOffsetY = Input.mousePosition.y - buttonTransform.anchoredPosition.y;
        }
        else if (anchor == UIAnchors.Right || anchor == UIAnchors.Left)
        {
            dragOffsetX = Help.BlankButtonRect.rect.width / 2;
            dragOffsetY = Help.BlankButtonRect.rect.height / 2;
        }
        
        Help.BlankButton.transform.SetParent(parentObject.transform, false);
        Help.BlankButton.SetActive(true);

        if (menuScript != null)
        {
            menuScript.HideMenu();
        }
    }

    /// <summary>
    /// a method for handling the end of drag events.
    /// </summary>
    public void OnEndDrag()
    {
        beingDragged = false;
        Help.BlankButton.SetActive(false);

        Vector3 mousePos = Input.mousePosition; //Note: y = 0 is the bottom.
        
        //Determining the closest edge of the screen.
        if (mousePos.y >= canvasRect.rect.height / 2)
        {
            //Mouse is within the top of the screen.
            if (mousePos.x <= canvasRect.rect.width / 2)
            {
                //Mouse is within the left side of the screen.
                if (mousePos.x < canvasRect.rect.height - mousePos.y)
                    RotateButton(UIAnchors.Left);
                else
                    RotateButton(UIAnchors.Top);
            }
            else
            {
                //Mouse is within the right side of the screen.
                if (canvasRect.rect.width - mousePos.x < canvasRect.rect.height - mousePos.y)
                    RotateButton(UIAnchors.Right);
                else
                    RotateButton(UIAnchors.Top);
            }
        }
        else
        {
            //Mouse is within the bottom of the screen.
            if (mousePos.x <= canvasRect.rect.width / 2)
            {
                //Mouse is within the left side of the screen.
                if (mousePos.x < mousePos.y)
                    RotateButton(UIAnchors.Left);
                else
                    RotateButton(UIAnchors.Bottom);
            }
            else
            {
                //Mouse is within the right side of the screen.
                if (canvasRect.rect.width - mousePos.x < mousePos.y)
                    RotateButton(UIAnchors.Right);
                else
                    RotateButton(UIAnchors.Bottom);
            }
        }
    }

    /// <summary>
    /// Rotates the button this script is attached to, so that it fits along the given anchor point.
    /// </summary>
    /// <param name="anchor"></param>
    private void RotateButton(UIAnchors anchor)
    {
        UIAnchors previousAnchor = this.anchor;
        this.anchor = anchor;
        
        if (anchor == UIAnchors.Right)
            SetButtonRotation(90);
        else if (anchor == UIAnchors.Left)
            SetButtonRotation(-90);
        else
            SetButtonRotation(0);

        AlignTabs(anchor);
        AlignTabs(previousAnchor);

        if (menuScript != null)
        {
            menuScript.Anchor = anchor;
        }
    }

    /// <summary>
    /// Aligns the positions of all the tabs along the given anchor point.
    /// </summary>
    /// <param name="anchor">The anchor point or side of the screen to align.</param>
    private void AlignTabs(UIAnchors anchor)
    {
        List<RectTransform> buttonsOnAnchor = new List<RectTransform>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].anchor == anchor)
            {
                buttonsOnAnchor.Add(buttonTransforms[i]);
            }
        }

        Vector3 alignmentPoint = Vector3.zero;
        if (anchor == UIAnchors.Right)
        {
            //The buttons are aligned via their bottom.
            alignmentPoint.x = canvasRect.rect.width - offsetRight;
            alignmentPoint.y = ScreenHeight / 2 + ((buttonsOnAnchor.Count - 1) * spaceBetweenButtons);
            foreach (RectTransform t in buttonsOnAnchor)
            {
                alignmentPoint.y += t.rect.width / 2;
            }

            foreach (RectTransform t in buttonsOnAnchor)
            {
                alignmentPoint.y -= spaceBetweenButtons + t.rect.width;
                t.position = alignmentPoint;
            }
        }
        else if (anchor == UIAnchors.Left)
        {
            alignmentPoint.x = offsetLeft;
            alignmentPoint.y = ScreenHeight / 2 + ((buttonsOnAnchor.Count - 1) * spaceBetweenButtons);
            foreach (RectTransform t in buttonsOnAnchor)
            {
                alignmentPoint.y += t.rect.width / 2;
            }

            foreach (RectTransform t in buttonsOnAnchor)
            {
                t.position = alignmentPoint;
                alignmentPoint.y -= spaceBetweenButtons + t.rect.width;
            }
        }
        else if (anchor == UIAnchors.Top)
        {
            alignmentPoint.x = canvasRect.rect.width / 2 + ((buttonsOnAnchor.Count - 1) * spaceBetweenButtons);
            foreach (RectTransform t in buttonsOnAnchor)
            {
                alignmentPoint.x += t.rect.width / 2;
            }

            foreach (RectTransform t in buttonsOnAnchor)
            {
                alignmentPoint.y = ScreenHeight - t.rect.height - offsetTop;
                alignmentPoint.x -= spaceBetweenButtons + t.rect.width;
                t.position = alignmentPoint;
                
            }
        }
        else if (anchor == UIAnchors.Bottom)
        {
            alignmentPoint.y = offsetBottom;
            alignmentPoint.x = canvasRect.rect.width / 2 + ((buttonsOnAnchor.Count - 1) * spaceBetweenButtons);
            foreach (RectTransform t in buttonsOnAnchor)
            {
                alignmentPoint.x += t.rect.width / 2;
            }

            foreach (RectTransform t in buttonsOnAnchor)
            {
                alignmentPoint.x -= spaceBetweenButtons + t.rect.width;
                t.position = alignmentPoint;
            }
        }
    }

    /// <summary>
    /// Rotates the button that this script is attached to, to the given angle on the z-axis.
    /// </summary>
    /// <param name="angle">The angle to rotate the button on the z-axis.</param>
    private void SetButtonRotation(float angle)
    {
        buttonTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Update is called once per frame
    void Update()
    {
        if (beingDragged)
        {
            Vector3 pos = Input.mousePosition;
            pos.x -= dragOffsetX;
            pos.y -= dragOffsetY;
            Help.BlankButton.transform.position = pos;
        }
	}

    void OnDestroy()
    {
        Help.onResolutionChange -= onResolutionChange;
    }
}

/// <summary>
/// An enumeration containing the different types of available anchor points for a UI button.
/// </summary>
public enum UIAnchors
{
    Right = 1,
    Bottom = 2,
    Left = 3,
    Top = 4
}