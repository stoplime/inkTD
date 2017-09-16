using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class TowerSpawner : MonoBehaviour {

	public GameObject highlight;
	private GameObject existingHighlight = null;

	private Grid parentGrid;


    private RaycastHit hit;

	public void PlaceTower(string towerPrefab)
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
							target.y = 0;
							GameObject newTower = Instantiate(Resources.Load("Towers/Arrow/" + towerPrefab), target, hit.collider.transform.rotation) as GameObject;
							Tower ntScript = newTower.GetComponent<Tower>();
							ntScript.SetTowerPosition(gridPos);
							ntScript.ownerID = parentGrid.ID;
                            parentGrid.setGridObject(gridPos, newTower);
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
	}
	
	// Update is called once per frame
	void Update () {
		PlaceTower("Archer_Tower");
	}
}
