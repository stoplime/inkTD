using helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoController : MonoBehaviour
{

    public GameObject content;
    public Text statText;
    public Text description;
    public Image towerPreview;
    public Text title;

    public Button button;

    /// <summary>
    /// Gets the object this info controller is displaying.
    /// </summary>
    public GridSnapInkObject DisplayedObject
    {
        get { return selectedObj; }
    }

    public int Owner
    {
        get { return owner; }
    }

    public int GridX { get { return gridX; } }

    public int GridY { get { return gridY; } }

    public InkObjectTypes HeldObjectType
    {
        get { return objectHeldType; }
    }

    private GameLoader gameLoader;
    private GridSnapInkObject selectedObj;
    private int owner;
    private int gridX;
    private int gridY;

    private InkObjectTypes objectHeldType = InkObjectTypes.Tower;
    
    // Use this for initialization
    void Start ()
    {
        gameLoader = Help.GetGameLoader();
	}

    /// <summary>
    /// Sets the obstacle info to correspond to the given tower.
    /// </summary>
    /// <param name="obstacle">The given obstacle whose info will be displayed.</param>
    public void SetObstacle(Obstacle obstacle, int playerID, int playerWhoSelected, int gridX, int gridY)
    {
        if (gameLoader == null)
            gameLoader = Help.GetGameLoader();

        this.gridX = gridX;
        this.gridY = gridY;
        selectedObj = obstacle;
        owner = playerID;

        if (obstacle == null)
        {
            objectHeldType = InkObjectTypes.None;
            towerPreview.sprite = gameLoader.defaultCreatureSprite;
            statText.text = "";
            description.text = "Sold!";
            title.text = "Sold!";
            button.gameObject.SetActive(false);
        }
        else
        {
            objectHeldType = InkObjectTypes.Obstacle;
            string spriteName = obstacle.ObstacleID.ToString();
            if (gameLoader.CachedSnapShotExists(spriteName))
                towerPreview.sprite = gameLoader.GetCachedSnapShot(spriteName);
            else
                towerPreview.sprite = gameLoader.defaultCreatureSprite;

            statText.text = obstacle.GetStatString();
            description.text = obstacle.GetPostDescription();
            title.text = obstacle.objName;

            button.gameObject.SetActive(playerID == playerWhoSelected);
        }

        if (OnNewTower != null)
            OnNewTower(this, EventArgs.Empty);
    }

    /// <summary>
    /// Sets the tower info to correspond to the given tower.
    /// </summary>
    /// <param name="tower">The given tower whose info will be displayed.</param>
    public void SetTower(Tower tower, int playerID, int playerWhoSelected, int gridX, int gridY)
    {
        if (gameLoader == null)
            gameLoader = Help.GetGameLoader();

        this.gridX = gridX;
        this.gridY = gridY;
        selectedObj = tower;
        owner = playerID;

        if (tower == null)
        {
            objectHeldType = InkObjectTypes.None;
            towerPreview.sprite = gameLoader.defaultCreatureSprite;
            statText.text = "";
            description.text = "Sold!";
            title.text = "Sold!";
            button.gameObject.SetActive(false);
        }
        else
        {
            objectHeldType = InkObjectTypes.Tower;
            towerPreview.sprite = gameLoader.GetTowerSprite(tower.towerType);

            statText.text = tower.GetStatString();
            description.text = tower.GetPostDescription();
            title.text = tower.objName;
            
            button.gameObject.SetActive(playerID == playerWhoSelected);
        }

        if (OnNewTower != null)
            OnNewTower(this, EventArgs.Empty);
    }

    public void OnUpgradePress()
    {
        if (objectHeldType == InkObjectTypes.Tower)
        {
            TryUpgradeTower();
        }
    }
    private void TryUpgradeTower()
    {
        Tower tower = selectedObj as Tower;
        if (gameLoader.GetTowerScript(tower.towerType).price <= PlayerManager.GetBalance(owner))
        {
            PlayerManager.ReplaceTower(owner, gridX, gridY, tower.towerType);
            Grid grid = PlayerManager.GetGrid(owner);
            Tower towerScript = grid.getGridObject(gridX, gridY).GetComponent<Tower>();
            towerScript.Pressed();

            if (UpgradePressed != null)
            {
                UpgradePressed(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler OnNewTower;

    public event EventHandler UpgradePressed;
}
