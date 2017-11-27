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

    public TabMenu otherMenu; //What is this for? Why is it hardcoded to a single menu?

    [Header("Alternative Menu")]

    [Tooltip("The left right menu that is used if the alternative menu is active.")]
    public GameObject alternativeLeftRightMenu;

    [Tooltip("The top bottom menu that is used if the alternative menu is active.")]
    public GameObject alternativeTopBottomMenu;

    [Tooltip("If true, when the menu is hidden or closed, the original menu will become the active menu.")]
    public bool restoreOriginalMenuOnClose = true;

    [Tooltip("If true the alternative menu will be used instead of the actual menu.")]
    private bool alternativeMenuActive = false;

    /// <summary>
    /// Determines if the alternative menu is the active menu.
    /// </summary>
    public bool AlternativeMenuActive
    {
        get { return alternativeMenuActive; }
        set
        {
            UseAlternativeMenu(value);
        }
    }
    
    /// <summary>
    /// Returns true if the tab menu is currently visible, false otherwise.
    /// </summary>
    public bool IsVisible
    {
        get { return visible; }
    }

    /// <summary>
    /// Returns true if the menu is in a transition phase. Phase otherwise.
    /// </summary>
    public bool Transitioning
    {
        get { return transitioning; }
    }

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

    /// <summary>
    /// Gets or sets additional information for this tab menu.
    /// </summary>
    public object ExtraInfo
    {
        get { return extraInfo; }
        set { extraInfo = value; }
    }

    protected UIAnchors anchor = UIAnchors.Right;
    protected RectTransform toolbar;
    protected Tab_Handler tabHandler;

    protected RectTransform leftRightRect;
    protected RectTransform topBottomRect;
    protected Vector3 leftRightPosition;
    protected Vector3 topBottomPosition;

    private object extraInfo;

    private bool transitioning = false;

    private bool visible = false;

    private bool leftRightOrientation = false;

    private float inwardGoal = 0f;
    private float outwardGoal;

    private GameObject activeLeftRightMenu;
    private GameObject activeTopBottomMenu;

    /// <summary>
    /// If transitionIn is true, then the menu transition in inward to the screen so it becomes visible to the player. If false it is outward to go beyond
    /// the screen becoming invisible to the player.
    /// </summary>
    private bool transitionIn;

    public virtual void Start()
    {
        Help.onResolutionChange += Help_onResolutionChange;

        tabHandler = tabButton.GetComponent<Tab_Handler>();

        SetActiveLeftRightMenu(leftRightMenu);

        SetActiveTopBottomMenu(topBottomMenu);

        ChangeAnchor(anchor);

        if (hideMenuOnStart)
            HideMenu();

        toolbar = GameObject.FindGameObjectWithTag("ToolbarPanel").GetComponent<RectTransform>();
    }

    public void SetActiveLeftRightMenu(GameObject menu)
    {
        if (menu != null)
        {
            bool visiblity = false;
            if (activeLeftRightMenu != null)
            {
                visiblity = activeLeftRightMenu.activeSelf;
                HideMenu();
            }

            leftRightRect = menu.GetComponent<RectTransform>();
            leftRightPosition = leftRightRect.anchoredPosition;
            activeLeftRightMenu = menu;
            activeLeftRightMenu.SetActive(visiblity);
        }
    }

    public void SetActiveTopBottomMenu(GameObject menu)
    {
        if (menu != null)
        {
            bool visiblity = false;
            if (activeTopBottomMenu != null)
            {
                visiblity = activeTopBottomMenu.activeSelf;
                HideMenu();
            }

            topBottomRect = menu.GetComponent<RectTransform>();
            topBottomPosition = topBottomRect.anchoredPosition;
            activeTopBottomMenu = menu;
            activeTopBottomMenu.SetActive(visiblity);
        }
    }

    /// <summary>
    /// Sets the active state of the alternative menu.
    /// </summary>
    /// <param name="value">If true the alternative menu will be the active menu, if false the regular menu will be used.</param>
    public void UseAlternativeMenu(bool value)
    {
        if (alternativeMenuActive != value)
        {
            if (value)
            {
                SetActiveLeftRightMenu(alternativeLeftRightMenu);
                SetActiveTopBottomMenu(alternativeTopBottomMenu);
            }
            else
            {
                SetActiveLeftRightMenu(leftRightMenu);
                SetActiveTopBottomMenu(topBottomMenu);
            }

            alternativeMenuActive = value;
        }
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

                if (activeLeftRightMenu != null)
                    activeLeftRightMenu.SetActive(false);
                if (activeTopBottomMenu != null)
                    activeTopBottomMenu.SetActive(true);
                break;
            case UIAnchors.Left:
                inwardGoal = leftRightRect.rect.width - Screen.width;
                outwardGoal = -Screen.width;
                leftRightOrientation = true;

                if (activeLeftRightMenu != null)
                    activeLeftRightMenu.SetActive(true);
                if (activeTopBottomMenu != null)
                    activeTopBottomMenu.SetActive(false);
                break;
            case UIAnchors.Right:
                inwardGoal = 0f;
                outwardGoal = leftRightRect.rect.width;
                leftRightOrientation = true;

                if (activeLeftRightMenu != null)
                    activeLeftRightMenu.SetActive(true);
                if (activeTopBottomMenu != null)
                    activeTopBottomMenu.SetActive(false);
                break;
            case UIAnchors.Top:
                inwardGoal = Screen.height - topBottomRect.rect.height - 25;
                outwardGoal = Screen.height - toolbar.rect.height;
                leftRightOrientation = false;

                if (activeLeftRightMenu != null)
                    activeLeftRightMenu.SetActive(false);
                if (activeTopBottomMenu != null)
                    activeTopBottomMenu.SetActive(true);
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
        ToggleMenuRollout();
    }

    public void ToggleMenuRollout()
    {
        otherMenu.HideMenu();
        if (!transitioning)
        {
            transitioning = true;
            visible = true;
        }

        transitionIn = !transitionIn;
    }

    private void RestoreOriginalMenu()
    {
        SetActiveLeftRightMenu(leftRightMenu);
        SetActiveTopBottomMenu(topBottomMenu);
        alternativeMenuActive = false;
    }

    /// <summary>
    /// Hides the menu from being visible.
    /// </summary>
    public virtual void HideMenu()
    {
        if (activeLeftRightMenu != null)
        {
            leftRightPosition.x = outwardGoal;
            leftRightRect.anchoredPosition = leftRightPosition;
        }

        if (activeTopBottomMenu != null)
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

                                if (restoreOriginalMenuOnClose && alternativeMenuActive)
                                {
                                    RestoreOriginalMenu();
                                }
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

                                if (restoreOriginalMenuOnClose && alternativeMenuActive)
                                {
                                    RestoreOriginalMenu();
                                }
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

                                if (restoreOriginalMenuOnClose && alternativeMenuActive)
                                {
                                    RestoreOriginalMenu();
                                }
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

                                if (restoreOriginalMenuOnClose && alternativeMenuActive)
                                {
                                    RestoreOriginalMenu();
                                }
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
