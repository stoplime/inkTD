﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;
using System;

[System.Serializable]
public struct ObstaclePiece
{
    public GameObject obstaclePrefab;
    public int gridPositionX;
    public int gridPositionY;
}

public class Obstacle : GridSnapInkObject
{
    [Tooltip("If checked/true then the obstacle can be purchased and removed.")]
	public bool removeable;

    [Tooltip("Not currently working")]
    public ObstaclePiece[] additionalPieces;

    [Tooltip("The object that the snapshot is taken of. Leave as null if you want the object this script is attached to.")]
    public GameObject snapshotObject;

    public bool TakeSnapshot = true;

    public float snapShotDistance = 6f;

    public Vector3 snapShotOffset = Vector3.zero;

    /// <summary>
    /// Gets the obstacle's personal ID.
    /// </summary>
    public Guid ObstacleID
    {
        get { return obstacleID; }
    }

    /// <summary>
    /// Returns the name of the cached snapshot of this obstacle, if it has one.
    /// </summary>
    public string SnapShotName
    {
        get { return snapShotName; }
    }

    private Guid obstacleID = Guid.NewGuid();

    private string snapShotName;
    
    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        snapShotName = objName; //Alternatively we can use obstacleID.ToString() though it is not recommended.

        gameLoader = Help.GetGameLoader();
        if (TakeSnapshot && !gameLoader.CachedSnapShotExists(objName))
        {
            if (snapshotObject == null)
                gameLoader.TakeSnapShotOf(gameObject, snapShotName, snapShotDistance, snapShotOffset, true);
            else
                gameLoader.TakeSnapShotOf(snapshotObject, snapShotName, snapShotDistance, snapShotOffset, true);
        }
    }
}
