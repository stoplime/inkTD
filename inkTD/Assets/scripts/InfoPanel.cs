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
    public Text rateOfFireText;
    public Text damageText;
    public Text descriptionText;
    public Text healthText;
    public Button buyButton;

    public string objName, description;
    public float cost, health, rateOfFire, damage;


    // Use this for initialization
    void Start ()
    {
        ClearMenu();
	}

    /// <summary>
    /// A method used to recieve the tower information from a button clicked in the buy menu
    /// </summary>
    /// <param name="towerName"></param>
    /// <param name="description"></param>
    /// <param name="cost"></param>
    /// <param name="damage"></param>
    /// <param name="rateOfFire"></param>
    /// <param name="image"></param>
    public void RecieveTowerInfo(string towerName, string description, float cost, float damage, float rateOfFire, Sprite image)
    {
        objName = towerName;
        this.description = description;
        this.cost = cost;
        this.damage = damage;
        this.rateOfFire = rateOfFire;
        this.image.overrideSprite = image;
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
    public void RecieveCreatureInfo(string creatureName, string description, float health, float cost, float damage, Sprite image)
    {
        objName = creatureName;
        this.description = description;
        this.health = health;
        this.cost = cost;
        this.damage = damage;
        this.image.overrideSprite = image;
    }

    /// <summary>
    /// A method used to recieve the modifier information from a button clicked in the buy menu
    /// </summary>
    /// <param name="modName"></param>
    /// <param name="description"></param>
    /// <param name="cost"></param>
    public static void RecieveModifierInfo(string modName, string description, float cost)
    {

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
        
        if (rateOfFireText != null)
        {
            rateOfFireText.text = "";
        }

        if (healthText != null)
        {
            healthText.text = "";
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        nameText.text = objName;
        descriptionText.text = description;
        costText.text = "Cost: " + Convert.ToString(cost);
        damageText.text = "Damage: " + Convert.ToString(damage);

        if (rateOfFireText != null)
        {
            rateOfFireText.text = "Rate of Fire: " + Convert.ToString(rateOfFire);
        }

        if (healthText != null)
        {
            healthText.text = "Health: " + Convert.ToString(health);
        }
    }
}
