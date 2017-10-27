using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class TowerSpawner : MonoBehaviour {

	public static TowerNode<TowerData> TowerPrefabs = new TowerNode<TowerData>(new TowerData("ROOT"));

	public int OwnerID;

	public GameObject highlight;

    public GameObject notEnoughInkObject;

    public string notEnoughInkText = "Not Enough Ink";

    public int textLife = 6000;

	private GameObject existingHighlight = null;

	private Grid parentGrid;

    private RaycastHit hit;

	private bool isPlaceable;

    private List<Creature> creatures;

	/// <summary>
	/// Constructs the data structure for the Tower types and upgrades.
	/// </summary>
	public void InitTowerPrefabs()
	{
		// Root Tower Arrow Type
		TowerData Arrow = new TowerData("Arrow");
		TowerPrefabs.AddNode(Arrow);
		// Lvl 1
		TowerData Archer_Tower = new TowerData("Arrow/Archer_Tower");
		TowerPrefabs[Arrow].AddNode(Archer_Tower);
		// Lvl 2
		TowerData Crossbow_Tower = new TowerData("Arrow/Crossbow_Tower");
		TowerPrefabs[Archer_Tower].AddNode(Crossbow_Tower);


		// Root Tower Cannon Type
		TowerData Cannon = new TowerData("Cannon");
		TowerPrefabs.AddNode(Cannon);
		// Lvl 1
		TowerData Catapult_Tower = new TowerData("Cannon/Catapult_Tower");
		TowerPrefabs[Cannon].AddNode(Catapult_Tower);
		// Lvl 2
		TowerData Trebuchet_Tower = new TowerData("Cannon/Trebuchet_Tower");
		TowerPrefabs[Catapult_Tower].AddNode(Trebuchet_Tower);
		// Lvl 3
		TowerData Cannonball_Tower = new TowerData("Cannon/Cannonball_Tower");
		TowerPrefabs[Trebuchet_Tower].AddNode(Cannonball_Tower);
		// Lvl 4 Split Cannonball_Tower to Bomb_Tower path and Anvil_Tower path
		TowerData Bomb_Tower = new TowerData("Cannon/Bomb_Tower");
		TowerPrefabs[Cannonball_Tower].AddNode(Bomb_Tower);


		// Root Tower Magic Type
		TowerData Magic = new TowerData("Magic");
		TowerPrefabs.AddNode(Magic);
		// Lvl 1
		TowerData Magic_Tower = new TowerData("Magic/Magic_Tower");
		TowerPrefabs[Magic].AddNode(Magic_Tower);
		// Lvl 2
		TowerData Mystic_Tower = new TowerData("Magic/Mystic_Tower");
		TowerPrefabs[Magic_Tower].AddNode(Mystic_Tower);
		// Lvl 3
		TowerData Arcane_Tower = new TowerData("Magic/Arcane_Tower");
		TowerPrefabs[Mystic_Tower].AddNode(Arcane_Tower);
		// Lvl 4 Split Arcane_Tower to Psionics_Towerpath and Mana_Tower path
		TowerData Psionics_Tower = new TowerData("Magic/Psionics_Tower");
		TowerPrefabs[Arcane_Tower].AddNode(Psionics_Tower);
		TowerData Mana_Tower = new TowerData("Magic/Mana_Tower");
		TowerPrefabs[Arcane_Tower].AddNode(Mana_Tower);
		// Lvl 5
		TowerData Brain_Tower = new TowerData("Magic/Brain_Tower");
		TowerPrefabs[Psionics_Tower].AddNode(Brain_Tower);
		TowerData Aether_Tower = new TowerData("Magic/Aether_Tower");
		TowerPrefabs[Mana_Tower].AddNode(Aether_Tower);
		// Lvl 6
		TowerData Enlightenment_Tower = new TowerData("Magic/Enlightenment_Tower");
		TowerPrefabs[Brain_Tower].AddNode(Enlightenment_Tower);
		TowerData Wizard_Tower = new TowerData("Magic/Wizard_Tower");
		TowerPrefabs[Aether_Tower].AddNode(Wizard_Tower);

		
	}

    public void PlaceTower(string towerPrefab, IntVector2 gridPos, Quaternion orientation)
	{
        PlayerManager.PlaceTower(OwnerID, OwnerID, gridPos, orientation, towerPrefab, notEnoughInkObject, notEnoughInkText, textLife);
	}

	public void SelectLocation(string towerPrefab)
	{
		if (!Help.MouseOnUI){
			if (Help.GetObjectInMousePath(out hit)){
				if (hit.collider.tag == "GroundObject"){
					IntVector2 gridPos = Grid.posToGrid(hit.point);
					if (parentGrid.inArena(gridPos) && parentGrid.isGridEmpty(gridPos)){
						Vector3 target = Grid.gridToPos(gridPos);
						target.y = 0.1f;
						if(existingHighlight == null){
							existingHighlight = Instantiate(highlight, target, hit.collider.transform.rotation);
						}else{
							existingHighlight.transform.position = target;
						}
						if (Input.GetButtonDown("Fire1")){
							PlaceTower(towerPrefab, gridPos, hit.collider.transform.rotation);
						}
					}
					else if(existingHighlight != null){
						existingHighlight.transform.position = new Vector3(0,-100,0);
					}
				}
			}
		}
		else
		if(existingHighlight != null)
		{
			existingHighlight.transform.position = new Vector3(0,-100,0);
		}
	}

	// Use this for initialization
	void Start () {
		InitTowerPrefabs();
		parentGrid = gameObject.GetComponentInParent<Grid>();
		highlight.transform.localScale = new Vector3(Grid.gridSize, 0.1f, Grid.gridSize);
		isPlaceable = (OwnerID == parentGrid.ID);

        creatures = PlayerManager.GetCreatures(OwnerID);
    }
	
	// Update is called once per frame
	void Update () {
		if (isPlaceable)
		{
			SelectLocation("Arrow/Archer_Tower");
		}
	}
}
