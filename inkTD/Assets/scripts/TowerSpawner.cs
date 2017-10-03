using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class TowerSpawner : MonoBehaviour {

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
