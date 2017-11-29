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
    /// Gets the tower this tower info controller is displaying.
    /// </summary>
    public Tower Tower
    {
        get { return tower; }
    }

    public int Owner
    {
        get { return owner; }
    }

    public int GridX { get { return gridX; } }

    public int GridY { get { return gridY; } }

    private GameLoader gameLoader;
    private StringBuilder builder;
    private Tower tower;
    private int owner;
    private int gridX;
    private int gridY;

    // Use this for initialization
    void Start ()
    {
        gameLoader = Help.GetGameLoader();
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
        this.tower = tower;
        owner = playerID;

        if (tower == null)
        {
            towerPreview.sprite = gameLoader.defaultCreatureSprite;

            builder = new StringBuilder();
            builder.AppendLine("HP");
            builder.AppendLine(" Range");
            builder.AppendLine(" Projectiles/Minute");
            builder.AppendLine(" Damage");
            builder.AppendLine("$");

            statText.text = builder.ToString();
            description.text = "Sold!";
            title.text = "Sold!";
            button.gameObject.SetActive(false);
        }
        else
        {
            towerPreview.sprite = gameLoader.GetTowerSprite(tower.towerType);

            builder = new StringBuilder();
            builder.AppendLine(tower.maxHealth.ToString() + "HP");
            builder.AppendLine(tower.range.ToString() + " Range");
            builder.AppendLine(tower.speed.ToString() + " Projectiles/Minute");
            builder.AppendLine(tower.damage.ToString() + " Damage");
            builder.AppendLine("$" + tower.price.ToString());

            statText.text = builder.ToString();
            description.text = tower.GetPostDescription();
            title.text = tower.objName;
            
            button.gameObject.SetActive(playerID == playerWhoSelected);
        }

        if (OnNewTower != null)
            OnNewTower(this, EventArgs.Empty);
    }

    public void OnUpgradePress()
    {
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
