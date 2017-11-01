using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

/// <summary>
/// An interface for menu classes and scripts.
/// </summary>
public class TabMenu : MonoBehaviour
{
    [Tooltip("The speed in pixels per second that the menu will move at.")]
    public float animationSpeed = 355;

    [Tooltip("The tab button this menu will correspond to.")]
    public GameObject tabButton;

    [Tooltip("The menu that appears when the menu is attached to the left or right side of the screen.")]
    public GameObject leftRightMenu;

    [Tooltip("The menu that appears when the menu is attached to the top or bottom side of the screen.")]
    public GameObject topBottomMenu;

    [Tooltip("If true the menu will be hidden the moment the game is loaded.")]
    public bool hideMenuOnStart = true;

    public TabMenu otherMenu;

    /// <summary>
    /// Gets or sets the menu face the screen is attached to.
    /// </summary>
    public UIAnchors Anchor
    {
        get { return anchor; }
        set
        {
            ChangeAnchor(value);
        }
    }

    protected UIAnchors anchor = UIAnchors.Right;
    protected RectTransform toolbar;
    protected Tab_Handler tabHandler;

    protected RectTransform leftRightRect;
    protected RectTransform topBottomRect;
    protected Vector3 leftRightPosition;
    protected Vector3 topBottomPosition;

    private bool transitioning = false;

    private bool visible = false;

    private bool leftRightOrientation = false;

    private float inwardGoal = 0f;
    private float outwardGoal;

    /// <summary>
    /// If transitionIn is true, then the menu transition in inward to the screen so it becomes visible to the player. If false it is outward to go beyond
    /// the screen becoming invisible to the player.
    /// </summary>
    private bool transitionIn;

    public virtual void Start()
    {
        Help.onResolutionChange += Help_onResolutionChange;

        tabHandler = tabButton.GetComponent<Tab_Handler>();

        if (leftRightMenu != null)
        {
            leftRightRect = leftRightMenu.GetComponent<RectTransform>();
            leftRightPosition = leftRightRect.anchoredPosition;
        }

        if (topBottomMenu != null)
        {
            topBottomRect = topBottomMenu.GetComponent<RectTransform>();
            topBottomPosition = topBottomRect.anchoredPosition;
        }

        ChangeAnchor(anchor);

        if (hideMenuOnStart)
            HideMenu();

        toolbar = GameObject.FindGameObjectWithTag("ToolbarPanel").GetComponent<RectTransform>();
    }

    private void ChangeAnchor(UIAnchors anchor)
    {
        //TODO: Change the size of the menu or otherwise the menu used if anchor changes from left-right to top-bottom.
        this.anchor = anchor;
        switch (anchor)
        {
            case UIAnchors.Bottom:
                inwardGoal = 0f;
                outwardGoal = -topBottomRect.rect.height;
                leftRightOrientation = false;

                if (leftRightMenu != null)
                    leftRightMenu.SetActive(false);
                if (topBottomMenu != null)
                    topBottomMenu.SetActive(true);
                break;
            case UIAnchors.Left:
                inwardGoal = leftRightRect.rect.width - Screen.width;
                outwardGoal = -Screen.width;
                leftRightOrientation = true;

                if (leftRightMenu != null)
                    leftRightMenu.SetActive(true);
                if (topBottomMenu != null)
                    topBottomMenu.SetActive(false);
                break;
            case UIAnchors.Right:
                inwardGoal = 0f;
                outwardGoal = leftRightRect.rect.width;
                leftRightOrientation = true;

                if (leftRightMenu != null)
                    leftRightMenu.SetActive(true);
                if (topBottomMenu != null)
                    topBottomMenu.SetActive(false);
                break;
            case UIAnchors.Top:
                inwardGoal = Screen.height - topBottomRect.rect.height;
                outwardGoal = Screen.height - toolbar.rect.height;
                leftRightOrientation = false;

                if (leftRightMenu != null)
                    leftRightMenu.SetActive(false);
                if (topBottomMenu != null)
                    topBottomMenu.SetActive(true);
                break;
        }
        HideMenu();
    }

    protected virtual void Help_onResolutionChange(object sender, System.EventArgs e)
    {
        if (leftRightOrientation)
        {
            leftRightRect.offsetMax = new Vector2(leftRightRect.offsetMax.x, toolbar.rect.height);
        }
    }

    public virtual void OnDestroy()
    {
        Help.onResolutionChange -= Help_onResolutionChange;
    }

    public virtual void OnClick()
    {
        otherMenu.HideMenu();
        if (!transitioning)
        {
            transitioning = true;
            visible = true;
        }

        transitionIn = !transitionIn;
    }

    /// <summary>
    /// Hides the menu from being visible.
    /// </summary>
    public virtual void HideMenu()
    {
        if (leftRightMenu != null)
        {
            leftRightPosition.x = outwardGoal;
            leftRightRect.anchoredPosition = leftRightPosition;
        }

        if (topBottomMenu != null)
        {
            topBottomPosition.y = outwardGoal;
            topBottomRect.anchoredPosition = topBottomPosition;
        }

        transitioning = false;
        transitionIn = false;

        if (tabHandler != null && visible)
        {
            switch (tabHandler.Anchor)
            {
                case UIAnchors.Bottom:
                    tabHandler.OffsetBottom = 0;
                    break;
                case UIAnchors.Left:
                    tabHandler.OffsetLeft = 0;
                    break;
                case UIAnchors.Right:
                    tabHandler.OffsetRight = 0;
                    break;
                case UIAnchors.Top:
                    tabHandler.OffsetTop = 0;
                    break;
            }
        }

        visible = false;
    }

    void CheckOtherMenus()
    {
        //List<GameObject> menus = GameObject.Find();

    }

    // Update is called once per frame
    void Update()
    {
        if (transitioning)
        {
            switch (tabHandler.Anchor)
            {
                case UIAnchors.Bottom:
                    {
                        if (transitionIn)
                        {
                            topBottomPosition.y += (animationSpeed * Time.deltaTime);
                            if (topBottomPosition.y > inwardGoal)
                            {
                                topBottomPosition.y = inwardGoal;
                                transitioning = false;
                                visible = true;
                            }
                        }
                        else
                        {
                            topBottomPosition.y -= (animationSpeed * Time.deltaTime);
                            if (topBottomPosition.y < outwardGoal)
                            {
                                topBottomPosition.y = outwardGoal;
                                transitioning = false;
                                visible = false;
                            }
                        }

                        tabHandler.OffsetBottom = topBottomPosition.y + topBottomRect.rect.height;
                    }
                    break;
                case UIAnchors.Left:
                    {
                        if (transitionIn)
                        {
                            leftRightPosition.x += (animationSpeed * Time.deltaTime);
                            if (leftRightPosition.x > inwardGoal)
                            {
                                leftRightPosition.x = inwardGoal;
                                transitioning = false;
                                visible = true;
                            }
                        }
                        else
                        {
                            leftRightPosition.x -= (animationSpeed * Time.deltaTime);
                            if (leftRightPosition.x < outwardGoal)
                            {
                                leftRightPosition.x = outwardGoal;
                                transitioning = false;
                                visible = false;
                            }
                        }
                        tabHandler.OffsetLeft = leftRightPosition.x - outwardGoal;
                    }
                    break;
                case UIAnchors.Right:
                    {
                        if (transitionIn)
                        {
                            leftRightPosition.x -= (animationSpeed * Time.deltaTime);
                            if (leftRightPosition.x < inwardGoal)
                            {
                                leftRightPosition.x = inwardGoal;
                                transitioning = false;
                                visible = true;
                            }
                        }
                        else
                        {
                            leftRightPosition.x += (animationSpeed * Time.deltaTime);
                            if (leftRightPosition.x > outwardGoal)
                            {
                                leftRightPosition.x = outwardGoal;
                                transitioning = false;
                                visible = false;
                            }
                        }
                        tabHandler.OffsetRight = leftRightRect.rect.width - leftRightPosition.x;
                    }
                    break;
                case UIAnchors.Top:
                    {
                        if (transitionIn)
                        {
                            topBottomPosition.y -= (animationSpeed * Time.deltaTime);
                            if (topBottomPosition.y < inwardGoal)
                            {
                                topBottomPosition.y = inwardGoal;
                                transitioning = false;
                                visible = true;
                            }
                        }
                        else
                        {
                            topBottomPosition.y += (animationSpeed * Time.deltaTime);
                            if (topBottomPosition.y > outwardGoal)
                            {
                                topBottomPosition.y = outwardGoal;
                                transitioning = false;
                                visible = false;
                            }
                        }

                        tabHandler.OffsetTop = outwardGoal - topBottomPosition.y;
                    }
                    break;
            }

            if (leftRightOrientation)
            {
                leftRightRect.anchoredPosition = leftRightPosition;
            }
            else
            {
                topBottomRect.anchoredPosition = topBottomPosition;
            }
        }
    }


}
