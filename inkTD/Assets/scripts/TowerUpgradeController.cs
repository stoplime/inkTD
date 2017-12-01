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

        if (currentTowerController.DisplayedObject != null)
        {
            sellButtonText.gameObject.SetActive(currentTowerController.DisplayedObject.sellable);

            if (currentTowerController.DisplayedObject.sellable)
            {
                if (currentTowerController.HeldObjectType == InkObjectTypes.Tower)
                {
                    if (currentTowerController.DisplayedObject != null)
                        sellButtonText.text = "Sell for " + (currentTowerController.DisplayedObject.price * PlayerManager.ResellPercentage) + " Ink";

                    BuildTowerUpgradeList(gameLoader.TowerTabMenu.Anchor == UIAnchors.Left || gameLoader.TowerTabMenu.Anchor == UIAnchors.Right);
                }
                else if (currentTowerController.HeldObjectType == InkObjectTypes.Obstacle)
                {
                    if (currentTowerController.DisplayedObject != null)
                        sellButtonText.text = "Remove for " + (currentTowerController.DisplayedObject.price) + " Ink";
                }
            }
        }
        
    }

    private void BuildTowerUpgradeList(bool menuLeftRightOriented)
    {
        if (currentTowerController.Owner == PlayerManager.CurrentPlayer && currentTowerController.DisplayedObject != null)
        {
            Tower tower = currentTowerController.DisplayedObject as Tower;
            List<Towers> upgrades = gameLoader.GetTowerUpgrades(tower.towerType);

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

                if (menuLeftRightOriented)
                {
                    upgradeRect.anchoredPosition = new Vector3(rectTransform.rect.x, rectTransform.offsetMin.y - (spacePerUpgrade * i) - upgradeRect.rect.height * (i + 1) - upgradeOffset);
                }
                else
                {
                    upgradeRect.anchoredPosition = new Vector3(rectTransform.offsetMax.x + (spacePerUpgrade * i) + upgradeRect.rect.width * (i + 1) + upgradeOffset, rectTransform.rect.y);
                    upgradeRect.offsetMax = new Vector2(upgradeRect.offsetMax.x, rectTransform.offsetMax.y);
                }

                if (childrenInfluencedScrollBar != null)
                {
                    scrollController = obj.GetComponentInChildren<UIScrollControl>();
                    scrollController.scrollBar = childrenInfluencedScrollBar;
                    scrollController.sensitivity = scrollBarSensitivity;
                }
            }

            if (menuLeftRightOriented)
            {
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
            else
            {
                if (upgrades.Count > 0)
                {
                    //TODO: Remove this hard code:
                    contentRect.sizeDelta = new Vector2(upgradeRect.offsetMax.x - Screen.width, contentRect.sizeDelta.y); //hardcoded screen.
                    contentRect.anchoredPosition = new Vector2(0f, contentRect.anchoredPosition.y);
                }
                else
                {
                    contentRect.sizeDelta = new Vector2((rectTransform.anchorMax.x - spacePerUpgrade), contentRect.sizeDelta.y);
                    contentRect.anchoredPosition = new Vector2(0f, contentRect.anchoredPosition.y);
                }
            }
        }
    }

    public void OnSellClick()
    {
        if (currentTowerController.HeldObjectType == InkObjectTypes.Tower)
        {
            Tower tower = currentTowerController.DisplayedObject as Tower;
            PlayerManager.SellTower(tower.ownerID, tower.GridPositionX, tower.GridPositionY);
            currentTowerController.SetTower(null, tower.ownerID, PlayerManager.CurrentPlayer, -1, -1);
            Help.GetGameLoader().TowerTabMenu.ToggleMenuRollout();
        }
        else if (currentTowerController.HeldObjectType == InkObjectTypes.Obstacle)
        {
            //TODO: Remove all additional pieces of the obstacle.
            Obstacle obstacle = currentTowerController.DisplayedObject as Obstacle;
            if (PlayerManager.GetBalance(PlayerManager.CurrentPlayer) >= obstacle.price)
            {
                PlayerManager.AddBalance(PlayerManager.CurrentPlayer, -obstacle.price);
                PlayerManager.DeleteGridObject(obstacle.ownerID, obstacle.GridPositionX, obstacle.GridPositionY);
                currentTowerController.SetObstacle(null, obstacle.ownerID, PlayerManager.CurrentPlayer, -1, -1);
                Help.GetGameLoader().TowerTabMenu.ToggleMenuRollout();
            }
        }
    }
}
