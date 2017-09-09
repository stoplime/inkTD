using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTraverser : MonoBehaviour {

    [Tooltip("The name of the object which will traverse the grid path.")]
    public string traverseeName = "DebugCube";

    public int gridID = 0;

    [Tooltip("Speed in seconds for the time it takes to complete the movement of 1 grid position.")]
    public float speed = 1f;

    public bool moving = false;

    public int currentPathIndex = 0;

    private GameObject traversee;
    private List<IntVector2> bestPath;
    
    private float progress = 0f;
    private Vector3 currentPos;
    private Vector3 startPos;
    private Vector3 endPos;

	// Use this for initialization
	void Start ()
    {
        traversee = GameObject.Find(traverseeName);
	}

    public void OnClick()
    {
        moving = !moving;
        bestPath = PlayerManager.GetBestPath(gridID);
        currentPathIndex = bestPath.Count - 1;
        UpdatePositions(currentPathIndex);
    }
	
    private void UpdatePositions(int i)
    {
        currentPos = Grid.gridToPos(bestPath[i]);
        currentPos.y += 0.3f;
        startPos = currentPos;
        if (i == 0)
            endPos = Grid.gridToPos(bestPath[bestPath.Count - 1]);
        else
            endPos = Grid.gridToPos(bestPath[i - 1]);
        endPos.y += 0.3f;
    }

	// Update is called once per frame
	void Update ()
    {
		if (moving)
        {
            progress += Time.deltaTime;
            Vector3[] points = {startPos, new Vector3(startPos.x, startPos.y+2, startPos.z), new Vector3(endPos.x, startPos.y+2, endPos.z), endPos};
            traversee.transform.position = helper.Help.ComputeBezier(progress / speed, points);
            if (progress > speed)
            {
                currentPathIndex--;
                progress = 0f;
                if (currentPathIndex < 0)
                {
                    moving = false;
                    currentPathIndex = 0;
                }
                UpdatePositions(currentPathIndex);
            }
        }
	}
}
