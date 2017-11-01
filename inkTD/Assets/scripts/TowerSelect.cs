using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSelect : MonoBehaviour
{

    private Ray ray;
    private RaycastHit hit;
    public Camera towerCam;
    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && !PlayerManager.PlayerSpawnTowerMode && hit.collider.gameObject.GetComponent<Tower>())
            {
                print(hit.collider.name);
                towerCam.GetComponent<TowerCamera>().MoveCamera(hit.collider.gameObject.GetComponent<Tower>());
            }
        }*/
    }

}
