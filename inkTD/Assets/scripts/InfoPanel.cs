using helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{

    public Image image;
    public Text nameText;
    public Text costText;
    public Text incomeText;
    public Text rangeText;
    public Text damageText;
    public Text descriptionText;
    public Text healthText;
    public Text modifiersText;
    public Text speedText;
    public Button buyButton;
    public Camera towerCam;

    [Tooltip("The script used to spawn towers in this scene.")]
    public TowerSpawner towerSpawner;
    
    private string objName, description;
    private float cost, income, health, rateOfFire, damage, range, speed;

    private GameLoader info;

    private CreatureQueuer creatureQueue;
    private Creatures creature;
    private Towers tower;
    private PurchaseType purchaseType;

    void Start ()
    {
        ClearMenu();
        info = Help.GetGameLoader();
        creatureQueue = GameObject.FindGameObjectWithTag("Toolbar").GetComponent<CreatureQueuer>();
	}

    public void RecieveTowerInfo(Towers tower)
    {
        this.tower = tower;
        Tower towerScript = info.GetTowerScript(tower);
        objName = towerScript.objName;
        description = towerScript.description;
        cost = towerScript.price;
        damage = towerScript.damage;
        range = towerScript.range;
        image.sprite = info.GetTowerSprite(tower);
        purchaseType = PurchaseType.Tower;
        towerSpawner.SetSelectedTower(tower);

        ApplyText();
    }

    /// <summary>
    /// A method used to recieve the creature information from a button clicked in the buy menu
    /// </summary>
    /// <param name="creatureName"></param>
    /// <param name="description"></param>
    /// <param name="health"></param>
    /// <param name="cost"></param>
    /// <param name="damage"></param>
    /// <param name="image"></param>
    public void RecieveCreatureInfo(Creatures creature)
    {
        this.creature = creature;
        Creature creatureScript = info.GetCreatureScript(creature);
        objName = creatureScript.objName;
        description = creatureScript.description;
        health = creatureScript.health;
        cost = creatureScript.price;
        income = creatureScript.inkcomeValue;
        damage = creatureScript.damage;
        speed = creatureScript.damage;
        image.overrideSprite = info.GetCreatureSprite(creature);
        purchaseType = PurchaseType.Creature;

        ApplyText();
    }

    /// <summary>
    /// A method used to recieve the modifier information from a button clicked in the buy menu
    /// </summary>
    /// <param name="modName"></param>
    /// <param name="description"></param>
    /// <param name="cost"></param>
    public void RecieveModifierInfo(string modName, string description, float cost)
    {

    }

    public void OnBuyClick()
    {
        if (purchaseType == PurchaseType.Creature)
        {
            if (PlayerManager.GetBalance(0) >= cost)
            {
                creatureQueue.AddButton(creature);
                PlayerManager.AddBalance(0, -cost);
            }
        }
        else if (purchaseType == PurchaseType.Tower)
        {

        }
    }

    private void ApplyText()
    {
        nameText.text = objName;
        descriptionText.text = description;
        costText.text = "Cost: " + Convert.ToString(cost);
        damageText.text = "Damage: " + Convert.ToString(damage);

        if (incomeText != null)
        {
            incomeText.text = "Income: " + Convert.ToString(income);
        }

        if (rangeText != null)
        {
            rangeText.text = "Range: " + Convert.ToString(range);
        }

        if (healthText != null)
        {
            healthText.text = "Health: " + Convert.ToString(health);
        }

        if (speedText != null)
        {
            speedText.text = "Speed: " + Convert.ToString(speed);
        }
    }

    /// <summary>
    /// A method used for clearing the information in the InfoPanel
    /// </summary>
    void ClearMenu()
    {
        nameText.text = "";
        descriptionText.text = "";
        costText.text = "";
        damageText.text = "";

        if (incomeText != null)
        {
            incomeText.text = "";
        }
        
        if (rangeText != null)
        {
            rangeText.text = "";
        }

        if (healthText != null)
        {
            healthText.text = "";
        }

        if (speedText != null)
        {
            speedText.text = "";
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        

        
    }
}
