using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public string objName, description;
    public float health, cost, damage, rateOfFire;
    public Sprite image;

    public enum ButtonType { Tower, Creature, Modifier };

    public ButtonType type;

    public InfoPanel infoPanel;

	// Use this for initialization
	void Start ()
    {
		
	}

    /// <summary>
    /// A method that sends certain pieces of information to the InfoPanel, based on which BuyPanel the button is in
    /// </summary>
    public void OnClick()
    {
        switch(type)
        {
            case ButtonType.Tower:
                infoPanel.RecieveTowerInfo(objName, description, cost, damage, rateOfFire, image);
                Debug.Log("Ok");
                break;

            case ButtonType.Creature:
                infoPanel.RecieveCreatureInfo(objName, description, health, cost, damage, image);
                break;

            case ButtonType.Modifier:
                //InfoPanel.RecieveModifierInfo();
                break;

            default:
                Debug.Log("No type assigned to this button");
                break;
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
