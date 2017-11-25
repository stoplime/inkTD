using helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

AI PLANS

Tower placement
- go through the optimal path backwards and look and the adjacent nodes on the path
  to find an available placement.
- use a weighted random selection between adjacent nodes to place

Creature spawning
- hard coded strategies that will use combinations of creatures
- spawn based off a random weighted list of actions, where an action is a creature or combination

 */
/// <summary>
/// Defines a structure for a heuristic position that holds a heuristic value and a position.
/// </summary>
public struct HPosition
{
    /// <summary>
    /// The position.
    /// </summary>
    public IntVector2 position;

    /// <summary>
    /// The heuristic of the position. A higher value denotes a better position.
    /// </summary>
    public int value;
}

/// <summary>
/// A list of states the AI can be in.
/// </summary>
public enum AIStates
{
    Idle = 0,
    PlacingTowers = 1,
    SpawningCreatures = 2,

}

public class EnemyAI : MonoBehaviour {

    [Tooltip("The player ID for this AI, this AI will assume the role of a player with the given ID.")]
	public int playerID;

	private Grid currentGrid;
    private List<Creature> creatures;
    
    private TaylorTimer timer100ms;
    private TaylorTimer timer500ms;
    private TaylorTimer timer1000ms;

    private int endIndexOffset = 1;

    private AIStates state = AIStates.PlacingTowers;
    private Towers targetTower = Towers.Bomb;
    private Tower targetTowerScript;

    private bool gridFull = false;

    private int towersInPlay = 0;

    private GameLoader gameData;

	// Use this for initialization
	void Start ()
    {
        currentGrid = PlayerManager.GetGrid(playerID);
        creatures = PlayerManager.GetCreatures(playerID);

        gameData = Help.GetGameLoader();

        SetState(state);
    }

    /// <summary>
    /// Sets the current tower the AI is attempted to place during its place tower phase.
    /// </summary>
    /// <param name="tower">The tower the AI will attempt to place</param>
    private void SetTargetTower(Towers tower)
    {
        targetTower = tower;
        targetTowerScript = gameData.GetTowerScript(tower);
    }

    private void SetState(AIStates state)
    {
        this.state = state;

        if (state == AIStates.PlacingTowers)
        {
            SetTargetTower(targetTower);
        }
    }

    private void ComputeCreatureSpawn()
    {
        if (state != AIStates.SpawningCreatures)
            return;
        
        PlayerManager.CreateCreature(playerID, Creatures.BatteringRam, true);
    }

    private void ComputeTowerPlacement()
    {
        if (state != AIStates.PlacingTowers || gridFull)
            return;

        List<IntVector2> bestPath = PlayerManager.GetBestPath(playerID);
        
        gridFull = bestPath.Count - endIndexOffset <= 0;

        if (!gridFull) //If the AI did not already scan the entire path...
        {
            List<HPosition> possiblePositions = new List<HPosition>();
            bool towerPositionFound = false;
            int offset = endIndexOffset;

            while (!towerPositionFound && offset != bestPath.Count)
            {
                //TODO: Expand this, by finding adjacent tiles to the bestPath[bestPath.Count - offset], so that we can add more tiles the AI can choose from (whether they're top heuristic or not
                //is up to the AI to determine if it wants to maximize the heuristic.)
                towerPositionFound = Help.ValidPosition(bestPath[bestPath.Count - offset], playerID, creatures, currentGrid);

                if (towerPositionFound)
                {
                    HPosition resultPosition = new HPosition();
                    IntVector2 position = bestPath[bestPath.Count - offset];
                    int heuristic = 0;

                    //Defining the heuristic of the position, is this position good? If so, here's the criteria to determine how good:

                    //Looping through ± 3 positions (3 above, 3 below) from the current position to determine if this position we're looking at is on the best path.
                    for (int i = Math.Min(bestPath.Count - 1, bestPath.Count - offset + 3); i >= 0 && i >= bestPath.Count - offset - 3; i--)
                    {
                        if (bestPath[i].x == position.x && bestPath[i].y == position.y)
                        {
                            heuristic += 20; //20 points for being on the best path.
                            break;
                        }
                    }

                    //Adding this position to the possible positions list.
                    resultPosition.value = heuristic;
                    resultPosition.position = position;
                    possiblePositions.Add(resultPosition);
                }
                //Incrementing the offset from the last position in the best path.
                offset++;
            }

            //We do not try to place the tower is the AI cannot afford the tower.
            if (PlayerManager.GetBalance(playerID) >= targetTowerScript.price)
            {
                endIndexOffset = offset; //By setting the endIndexOffset to the offset, we are .. actually I don't know anymore.

                if (possiblePositions.Count > 0)
                {
                    HPosition bestPosition = possiblePositions[0];
                    for (int i = 0; i < possiblePositions.Count; i++)
                    {
                        if (bestPosition.value < possiblePositions[i].value)
                            bestPosition = possiblePositions[i];
                    }

                    //should probably check if we can afford the tower.
                    bool placed = PlayerManager.PlaceTower(playerID, playerID, bestPosition.position, Quaternion.identity, targetTowerScript.gameObject, null, "", 0);

                    if (placed)
                    {
                        towersInPlay += 1;
                    }
                }
            }
        }
        //Temporary Test: When it finishes the tower maze, it goes on to spawn creatures.
        else if (state != AIStates.SpawningCreatures)
        {
            SetState(AIStates.SpawningCreatures);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // timer100ms.Update();
        // timer500ms.Update();
        // timer1000ms.Update();
    }
}
