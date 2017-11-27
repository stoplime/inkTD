using helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerClickHandler : MonoBehaviour
{

    private GameLoader gameLoader;
    private RaycastHit hit;
    // Use this for initialization
    void Start ()
    {
        gameLoader = Help.GetGameLoader();
    }

    private void RemoveExtraInfo(TabMenu menu)
    {
        if (menu.ExtraInfo != null)
        {
            Tower prevTower = menu.ExtraInfo as Tower;
            prevTower.visualizeRadius = false;
            prevTower.UpdateRadiusVisiblity();
            menu.ExtraInfo = null;
        }
    }

    public void Update()
    {
        if (!Help.MouseOnUI && Input.GetButtonDown("Fire1") && Help.GetObjectInMousePath(out hit))
        {
            if (gameLoader.TowerTabMenu.ExtraInfo != null)
            {
                RemoveExtraInfo(gameLoader.TowerTabMenu);
            }

            InkObject obj = hit.collider.gameObject.GetComponent<InkObject>();
            if (obj != null)
            {
                obj.Pressed();
            }
            else if (gameLoader.TowerTabMenu.AlternativeMenuActive 
                && gameLoader.TowerTabMenu.IsVisible
                && !gameLoader.TowerTabMenu.Transitioning)
            {
                RemoveExtraInfo(gameLoader.TowerTabMenu);
                gameLoader.TowerTabMenu.ToggleMenuRollout();
            }
        }
    }
}
