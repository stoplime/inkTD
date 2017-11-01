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
    public Text rangeText;
    public Text damageText;
    public Text descriptionText;
    public Text healthText;
    public Text modifiersText;
    public Text speedText;
    public Button buyButton;
    private string towerPath;
    public Camera towerCam;


    public string objName, description;
    public string[] modifiers;
    public float cost, health, rateOfFire, damage, range, speed;

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
    /// <param name="range"></param>
    /// <param name="image"></param>
    public void RecieveTowerInfo(string towerName, string description, float cost, float damage, float range, Sprite image, string towerPath)
    {
        objName = towerName;
        this.description = description;
        this.cost = cost;
        this.damage = damage;
        this.rateOfFire = range;
        //this.image.overrideSprite = image;
        TowerSpawner.selectedTowerPath = towerPath;
    }

    public void RecieveTowerInfo(Tower i, string towerPath)
    {
        objName = i.objName;
        this.description = i.description;
        this.cost = i.price;
        this.damage = i.damage;
        this.range = i.range;
        //this.image.overrideSprite = image;
        TowerSpawner.selectedTowerPath = towerPath;
        towerCam.GetComponent<TowerCamera>().MoveCamera(i);
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
    public void RecieveCreatureInfo(string creatureName, string description, float health, float cost, float damage, float speed, string path, Sprite image)
    {
        objName = creatureName;
        this.description = description;
        this.health = health;
        this.cost = cost;
        this.damage = damage;
        this.speed = speed;
        this.image.overrideSprite = image;
        CreatureSpawner.path = path;
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
        nameText.text = objName;
        descriptionText.text = description;
        costText.text = "Cost: " + Convert.ToString(cost);
        damageText.text = "Damage: " + Convert.ToString(damage);

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
}
