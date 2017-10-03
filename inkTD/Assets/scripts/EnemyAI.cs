using helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool gridFull = false;

    private int towersInPlay = 0;

	// Use this for initialization
	void Start ()
    {
        currentGrid = PlayerManager.GetGrid(playerID);
        creatures = PlayerManager.GetCreatures(playerID);

        timer100ms = new TaylorTimer(100);
        timer100ms.Elapsed += Timer100ms_Elapsed;

        timer500ms = new TaylorTimer(500);
        timer500ms.Elapsed += Timer500ms_Elapsed;

        timer1000ms = new TaylorTimer(1000);
        timer1000ms.Elapsed += Timer1000ms_Elapsed;
	}

    private void Timer1000ms_Elapsed(object sender, System.EventArgs e)
    {
        //1 second update 'ticks'

        //Basic flip/flop between tower spawning and creature creating.
        if (!gridFull && state != AIStates.PlacingTowers)
        {
            if (towersInPlay < 10 || PlayerManager.GetBalance(playerID) > 200)
            {
                state = AIStates.PlacingTowers;
            }
        }

        if (state != AIStates.SpawningCreatures)
        {
            if (PlayerManager.GetBalance(playerID) < 100 && towersInPlay >= 10)
            {
                state = AIStates.SpawningCreatures;
            }
        }

        ComputeCreatureSpawn();
    }

    private void Timer500ms_Elapsed(object sender, System.EventArgs e)
    {
        //500 millisecond update 'ticks'

    }

    private void Timer100ms_Elapsed(object sender, System.EventArgs e)
    {
        //100 millisecond update 'ticks'

        ComputeTowerPlacement();
    }

    private void ComputeCreatureSpawn()
    {
        if (state != AIStates.SpawningCreatures)
            return;

        //Spawn dem stick men
        PlayerManager.CreateCreature(playerID, "Stickman_Creature", gameObject);
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

            if (PlayerManager.GetBalance(playerID) >= 10) //NOTE: Hard coded to match the archer tower's value, this is a temporary measure until we can determine if the tower we are placing is affordable.
                endIndexOffset = offset; //TODO: Determine if the AI can afford the tower, if it can, then we do this line of code. If it cannot then we do not do this line of code.

            if (possiblePositions.Count > 0)
            {
                HPosition bestPosition = possiblePositions[0];
                for (int i = 0; i < possiblePositions.Count; i++)
                {
                    if (bestPosition.value < possiblePositions[i].value)
                        bestPosition = possiblePositions[i];
                }

                //should probably check if we can afford the tower.
                bool placed = PlayerManager.PlaceTower(playerID, playerID, bestPosition.position, Quaternion.identity, "Arrow/Archer_Tower", null, "", 0);

                if (placed)
                {
                    towersInPlay += 1;
                }
            }
        }
        //Temporary Test: When it finishes the tower maze, it goes on to spawn creatures.
        else if (state != AIStates.SpawningCreatures)
        {
            state = AIStates.SpawningCreatures;
        }

        //TODO: Build a table with the prefab information for towers, so that the price of a tower can be determined without instantiating a prefab. Same for creatures,
        //so that the AI can determine if it has enough ink to purchase towers/creatures before going through the trouble of computing it.
    }

    // Update is called once per frame
    void Update ()
    {
        timer100ms.Update();
        timer500ms.Update();
        timer1000ms.Update();
    }
}
