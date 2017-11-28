using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeController : MonoBehaviour
{

    public TowerInfoController currentTowerController;

    public Text sellButtonText;

    public TowerInfoController upgradeControllerPrefab;

    public GameObject baseContent;

    public float spacePerUpgrade = 15f;

    public float upgradeOffset = 35f;

    [Tooltip("toolbar that is affect by the children being scrolled within.")]
    public Scrollbar childrenInfluencedScrollBar;

    public float scrollBarSensitivity = 0.5f;

    private List<TowerInfoController> upgradeControllers = new List<TowerInfoController>();

    private GameLoader gameLoader;
    private RectTransform rectTransform;
    private RectTransform contentRect;

	// Use this for initialization
	void Start ()
    {
        currentTowerController.OnNewTower += CurrentTowerController_OnNewTower;
        gameLoader = Help.GetGameLoader();
        rectTransform = currentTowerController.gameObject.GetComponent<RectTransform>();
        contentRect = baseContent.GetComponent<RectTransform>();
    }

    private void CurrentTowerController_OnNewTower(object sender, System.EventArgs e)
    {
        if (upgradeControllers.Count != 0)
        {
            for (int i = 0; i< upgradeControllers.Count; i++)
            {
                Destroy(upgradeControllers[i].gameObject);
            }
            upgradeControllers.Clear();
        }
        if (currentTowerController.Tower != null)
            sellButtonText.text = "Sell for " + (currentTowerController.Tower.price * PlayerManager.ResellPercentage) + " Ink";

        if (currentTowerController.Owner == PlayerManager.CurrentPlayer && currentTowerController.Tower != null)
        {
            List<Towers> upgrades = gameLoader.GetTowerUpgrades(currentTowerController.Tower.towerType);

            GameObject obj;
            UIScrollControl scrollController;
            TowerInfoController controller;
            RectTransform upgradeRect = new RectTransform();
            for (int i = 0; i < upgrades.Count; i++)
            {
                obj = Instantiate(upgradeControllerPrefab.gameObject, baseContent.transform) as GameObject;
                controller = obj.GetComponent<TowerInfoController>();
                controller.SetTower(gameLoader.GetTowerScript(upgrades[i]), currentTowerController.Owner, PlayerManager.CurrentPlayer, currentTowerController.GridX, currentTowerController.GridY);
                upgradeControllers.Add(controller);
                upgradeRect = obj.GetComponent<RectTransform>();
                upgradeRect.anchoredPosition = new Vector3(rectTransform.rect.x, rectTransform.offsetMin.y - (spacePerUpgrade * i) - upgradeRect.rect.height * (i + 1) - upgradeOffset);

                if (childrenInfluencedScrollBar != null)
                {
                    scrollController = obj.GetComponentInChildren<UIScrollControl>();
                    scrollController.scrollBar = childrenInfluencedScrollBar;
                    scrollController.sensitivity = scrollBarSensitivity;
                }
            }

            if (upgrades.Count > 0)
            {
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, -upgradeRect.offsetMin.y);
                contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0f);
            }
            else
            {
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, -(rectTransform.anchorMin.y - spacePerUpgrade));
                contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0f);
            }
        }
    }

    public void OnSellClick()
    {
        Tower tower = currentTowerController.Tower;
        PlayerManager.SellTower(tower.ownerID, tower.GridPositionX, tower.GridPositionY);
        currentTowerController.SetTower(null, tower.ownerID, PlayerManager.CurrentPlayer,-1,-1);
        Help.GetGameLoader().TowerTabMenu.ToggleMenuRollout();
    }
}
