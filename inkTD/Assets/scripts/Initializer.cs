using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class Initializer : MonoBehaviour {

	public int ID = 0;

	public float StartInk = 100;

	public float StartInkcome = 10;

    public string towerCastlePrefabName = "Tower_Castle";

    private GameObject towerCastleObject;

	void Awake ()
    {
        //Instantiating and setting the tower castle:
        towerCastleObject = Instantiate(Resources.Load<GameObject>("Towers/" + towerCastlePrefabName));
        towerCastleObject.name = "Player " + (ID + 1).ToString() + "'s Tower Castle";
        GetComponent<Grid>().TowerCastle = towerCastleObject;
        //Debug.Log("ID: "+ID);
	}

    void Start()
    {
        PlayerManager.SetBalance(ID, StartInk);
        PlayerManager.SetIncome(ID, StartInkcome);
    }
}
