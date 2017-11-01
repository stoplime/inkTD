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

	// Use this for initialization
	void Start ()
    {
		PlayerManager.SetBalance(ID, StartInk);
		PlayerManager.SetIncome(ID, StartInkcome);

        //Instantiating and setting the tower castle:
        towerCastleObject = Instantiate(Resources.Load<GameObject>("Towers/" + towerCastlePrefabName));
        PlayerManager.GetGrid(ID).TowerCastle = towerCastleObject;
	}
}
