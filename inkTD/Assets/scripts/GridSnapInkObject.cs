using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnapInkObject : InkObject
{
    /// <summary>
    /// Sets this tower's position along the playing grid x axis.
    /// </summary>
    public int GridPositionX
    {
        get { return gridPos.x; }
        set { SetGridPosition(value, gridPos.y); }
    }

    /// <summary>
    /// Sets this tower's position along the playing grid's y axis.
    /// </summary>
    public int GridPositionY
    {
        get { return gridPos.y; }
        set { SetGridPosition(gridPos.x, value); }
    }

    [Header("Grid Snap Settings:")]
    [Space(10)]

    /// <summary>
    /// This variable is no longer used after the tower/creature has been created. 
    /// </summary>
    [Tooltip("The position along the playing grid's x axis.")]
    public int initialGridPositionX = 0;

    /// <summary>
    /// This variable is no longer used after the tower/creature has been created.
    /// </summary>
    [Tooltip("The position along the playing grid's y axis.")]
    public int initialGridPositionY = 0;

    [Header("Special Settings")]
    [Tooltip("If true, the ink object can change available positions in the grid. If false it will have no affect to the grid it is placed within.")]
    public bool existsInGrid = true;

    [Tooltip("If true, the ink object can be sold.")]
    public bool sellable = true;
    

    protected GameLoader gameLoader;

    public virtual void Awake()
    {
        gridPos.x = initialGridPositionX;
        gridPos.y = initialGridPositionY;
    }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        SetGridPosition(initialGridPositionX, initialGridPositionY);
        gameLoader = Help.GetGameLoader();
    }

    public virtual void OnDestroy()
    {
        if (PlayerManager.GetGrid(ownerID).getGridObject(gridPos.x, gridPos.y) == gameObject)
        {
            PlayerManager.SetGameObject(ownerID, null, gridPos.x, gridPos.y);
        }
    }

    protected virtual void OnGridPositionChange()
    {

    }

    /// <summary>
    /// Aligns the ink object to the given x and y within the playing grid.
    /// </summary>
    /// <param name="x">The x axis grid block number.</param>
    /// <param name="y">The y axis grid block number.</param>
    public void SetGridPosition(int x, int y)
    {
        //TODO: empty the grid position at gridPositionX, gridPositionY
        // Vector3 realPos = Grid.gridToPos(new IntVector2(x, y));
        transform.position = Grid.gridToPos(new IntVector2(x, y));
        initialGridPositionX = x;
        initialGridPositionY = y;
        gridPos.x = x;
        gridPos.y = y;
        //fill the grid position at x, y
        if (existsInGrid)
            PlayerManager.SetGameObject(ownerID, gameObject, x, y);

        OnGridPositionChange();
    }

    /// <summary>
    /// Aligns the ink object to the given x and y within the playing grid.
    /// </summary>
    /// <param name="xy">The IntVector2 grid position.</param>
    public void SetGridPosition(IntVector2 xy)
    {
        //TODO: empty the grid position at gridPositionX, gridPositionY
        // Vector3 realPos = Grid.gridToPos(xy);
        transform.position = Grid.gridToPos(xy);
        gridPos = xy;
        initialGridPositionX = xy.x;
        initialGridPositionY = xy.y;
        //fill the grid position at x, y
        if (existsInGrid)
            PlayerManager.SetGameObject(ownerID, gameObject, xy.x, xy.y);

        OnGridPositionChange();
    }

    /// <summary>
    /// Event that runs when the tower is selected.
    /// </summary>
    /// <param name="eventData"></param>
    public override void Pressed()
    {
        base.Pressed();

        if (gameLoader == null)
        {
            gameLoader = Help.GetGameLoader();
        }

        if (gameLoader.TowerTabMenu.AlternativeMenuActive || !gameLoader.TowerTabMenu.IsVisible)
        { //The upgrade menu can only be shown if the alternative menu is active or if the menu is simply not visible.
            GameObject leftRightMenu = GameObject.FindGameObjectWithTag("TowerSelectMenuHor");
            GameObject upDownMenu = GameObject.FindGameObjectWithTag("TowerSelectMenuVer");

            GameObject selectedMenu;
            TowerInfoController currentController;

            if (gameLoader.TowerTabMenu.Anchor == UIAnchors.Bottom || gameLoader.TowerTabMenu.Anchor == UIAnchors.Top)
            {
                selectedMenu = upDownMenu;
                currentController = gameLoader.towerControllerCurrentUpDown;
            }
            else
            {
                selectedMenu = leftRightMenu;
                currentController = gameLoader.towerControllerCurrentLeftRight;
            }

             

            if (this is Tower)
            {
                Tower thisTower = this as Tower;
                currentController.SetTower(thisTower, ownerID, PlayerManager.CurrentPlayer, gridPos.x, gridPos.y);
                thisTower.visualizeRadius = true;
                thisTower.UpdateRadiusVisiblity();
                gameLoader.TowerTabMenu.AlternativeMenuActive = true;
                gameLoader.TowerTabMenu.ExtraInfo = this;
            }
            else if (this is Obstacle)
            {
                currentController.SetObstacle(this as Obstacle, ownerID, PlayerManager.CurrentPlayer, gridPos.x, gridPos.y);
                gameLoader.TowerTabMenu.AlternativeMenuActive = true;
            }
            
            if (!gameLoader.TowerTabMenu.IsVisible)
                gameLoader.TowerTabMenu.ToggleMenuRollout();
        }
    }

    public override void OnValidate()
    {
        base.OnValidate();

        SetGridPosition(initialGridPositionX, initialGridPositionY);
    }
}
