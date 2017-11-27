using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Towers purchasableTower;

    public Creatures purchasableCreature;
    
    public PurchaseType purchaseType;

    public InfoPanel infoPanel;

    private GameLoader info;
    private Tower towerScript;
    private Creature creatureScript;

	// Use this for initialization
	void Start ()
    {
        info = Help.GetGameLoader();
        PlayerManager.OnCurrentPlayerBalanceChange += PlayerManager_OnCurrentPlayerBalanceChange;

        if (purchaseType == PurchaseType.Creature)
        {
            creatureScript = info.GetCreatureScript(purchasableCreature);
        }
        else if (purchaseType == PurchaseType.Tower)
        {
            towerScript = info.GetTowerScript(purchasableTower);
        }

        DetermineInteractivity();
    }

    private void PlayerManager_OnCurrentPlayerBalanceChange(object sender, System.EventArgs e)
    {
        DetermineInteractivity();
    }

    private void DetermineInteractivity()
    {
        if (purchaseType == PurchaseType.Creature)
        {
            if (PlayerManager.GetBalance(0) < creatureScript.price)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }
        }
        else if (purchaseType == PurchaseType.Tower)
        {
            if (towerScript == null)
            {
                towerScript = info.GetTowerScript(purchasableTower);
            }

            if (PlayerManager.GetBalance(0) < towerScript.price)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }
        }
    }

    void OnDestroy()
    {
        PlayerManager.OnCurrentPlayerBalanceChange -= PlayerManager_OnCurrentPlayerBalanceChange;
    }

    /// <summary>
    /// A method that sends certain pieces of information to the InfoPanel, based on which BuyPanel the button is in
    /// </summary>
    public void OnClick()
    {
        switch(purchaseType)
        {
            case PurchaseType.Tower:
                infoPanel.RecieveTowerInfo(purchasableTower);
                break;

            case PurchaseType.Creature:
                infoPanel.RecieveCreatureInfo(purchasableCreature);
                break;

            case PurchaseType.Modifier:
                //InfoPanel.RecieveModifierInfo();
                Debug.Log("Ok");
                break;

        }
    }

    // Update is called once per frame
    void Update ()
    {
		
    }
}
