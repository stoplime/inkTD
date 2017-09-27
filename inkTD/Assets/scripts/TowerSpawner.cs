using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class TowerSpawner : MonoBehaviour {

	public int OwnerID;

	public GameObject highlight;
	private GameObject existingHighlight = null;

	private Grid parentGrid;

    private RaycastHit hit;

	private bool isPlaceable;

	public void PlaceTower(string towerPrefab, IntVector2 gridPos, Quaternion orientation)
	{
		Vector3 location = Grid.gridToPos(gridPos);
		GameObject newTower = Instantiate(Resources.Load("Towers/" + towerPrefab), location, orientation) as GameObject;
		Tower ntScript = newTower.GetComponent<Tower>();
		ntScript.ownerID = parentGrid.ID;
		ntScript.SetTowerPosition(gridPos);
		parentGrid.setGridObject(gridPos, newTower);

		// Check if gridPos is a valid location for a tower to be placed
		bool pathFail = false;
		List<Creature> creatures = PlayerManager.GetCreatures(OwnerID);
		
		if (PlayerManager.GetBestPath(OwnerID).Count == 0)
		{
			pathFail = true;
		}
		else
		{
			for (int i = 0; i < creatures.Count; i++)
			{
				creatures[i].updateTempPath();
				if (!creatures[i].tempPathExists)
				{
					pathFail = true;
					break;
				}
			}
		}
		// print(pathFail);
		if (!pathFail && PlayerManager.GetBalance(OwnerID) >= ntScript.price)
		{
			PlayerManager.AddBalance(OwnerID, -ntScript.price);
		}
		else
		{
			pathFail = true;
		}
		
		if (pathFail)
		{
			parentGrid.setGridObject(gridPos, null);
			Destroy(newTower);
		}
		else
		{
			for (int i = 0; i < creatures.Count; i++)
			{
				creatures[i].updatePath();
			}
		}
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
		parentGrid = gameObject.GetComponentInParent<Grid>();
		highlight.transform.localScale = new Vector3(Grid.gridSize, 0.1f, Grid.gridSize);
		isPlaceable = (OwnerID == parentGrid.ID);
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaceable)
		{
			SelectLocation("Arrow/Archer_Tower");
		}
	}
}
