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
    UpgradeTowers = 2,
    SpawningCreatures = 3,
    ApplyModifiers = 4
}

public class EnemyAI : MonoBehaviour {

    [Tooltip("The player ID for this AI, this AI will assume the role of a player with the given ID.")]
	public int playerID;

    [Tooltip("How fast the AI will react to different decisions. Value in Miliseconds")]
    public int TimerValue = 500;

    [Tooltip("The Range of towers to be checked before placing. Making this large can take too long and place towers way too far off the main path.")]    
    public int TowerPlacementRange = 3;

    [Tooltip("A custom tower selection distribution, leave blank for a random distribution. Should have a length of the number of base towers available")]
    public List<float> CustomBaseTowerDistribution;

	private Grid currentGrid;
    private List<Creature> creatures;

    private TaylorTimer actionTimer;

    private AIStates state = AIStates.PlacingTowers;
    private List<float> stateWeights;

    private GameLoader gameData;

    // Represents the likelyhood of selecting which base tower on tower placement, has length number of base towers
    private List<float> BaseTowerDistribution;
    private List<Towers> BaseTowers;

    private int towersInPlay = 0;

    private int TowerPlacementBackToFrontOffset = 0;

    // For creature spawning hoards
    private List<Creature> SpawnCreaturesWave;

	// Use this for initialization
	void Start ()
    {
        actionTimer = new TaylorTimer(TimerValue);
        actionTimer.Elapsed += ActionTimer_Elapsed;

        stateWeights = new List<float>();
        stateWeights.Add(0.1f);  // 10% Idle
        stateWeights.Add(0.6f);  // 60% Place Tower
        stateWeights.Add(0.05f); // 5% Upgrade Towers
        stateWeights.Add(0.2f);  // 20% Spawn a Creaure Wave
        stateWeights.Add(0.05f); // 5% Applying Modifiers

        currentGrid = PlayerManager.GetGrid(playerID);
        creatures = PlayerManager.GetCreatures(playerID);

        gameData = Help.GetGameLoader();

        BaseTowerDistribution = new List<float>();
        BaseTowers = gameData.GetBaseTowers();
        if (CustomBaseTowerDistribution == null || CustomBaseTowerDistribution.Count != BaseTowers.Count)
        {
            for (int i = 0; i < BaseTowers.Count; i++)
            {
                BaseTowerDistribution.Add(UnityEngine.Random.value);
            }
            Normalize(BaseTowerDistribution);
        }
        else
        {
            BaseTowerDistribution = CustomBaseTowerDistribution;
        }
        

        SetState(state);
    }

    // Timer update function, runs every TimerValue amount of Milseconds
    private void ActionTimer_Elapsed(object sender, System.EventArgs e)
    {
        Normalize(stateWeights);
        // Set the newly selected state
        SetState( (AIStates)SelectWeightedRandom(stateWeights) );
    }

    /// <summary>
    /// Provides a random selected index based off of a normalized list of weights
    /// </summary>
    /// <param name="weights">Must be Normalized! (All numbers in the list has to add up to 1)</param>
    /// <returns></returns>
    private int SelectWeightedRandom(List<float> weights)
    {
        return SelectWeightedRandom(weights, UnityEngine.Random.value);
    }

    /// <summary>
    /// Provides a random selected index based off of a list of weights (does not have to be normalized)
    /// </summary>
    /// <param name="weights">Distribution of Weights</param>
    /// <param name="randomValue">A selected Value between 0 and the sum of all the Weights</param>
    /// <returns>if returns -1 then it broke</returns>
    private int SelectWeightedRandom(List<float> weights, float randomValue)
    {
        float percentBracket = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            percentBracket += weights[i];
            if (randomValue <= percentBracket)
            {
                return i;
            }
        }
        return -1;
    }

    private void Normalize(List<float> collection)
    {
        float sum = 0;
        for (int i = 0; i < collection.Count; i++)
        {
            sum += collection[i];
        }
        for (int i = 0; i < collection.Count; i++)
        {
            collection[i] = collection[i]/sum;
        }
    }

    // initializes an onStateChangeTo function
    private void SetState(AIStates state)
    {
        this.state = state;

        if (state == AIStates.PlacingTowers)
        {
            if (!TryTowerPlacement())
            {
                // Maybe decrease the chance to trying to spawn a tower?
            }
        }
        else if (state == AIStates.SpawningCreatures)
        {
            ComputeCreatureSpawn();
        }
    }

    private void ComputeCreatureSpawn()
    {
        if (state != AIStates.SpawningCreatures)
            return;
        
        PlayerManager.CreateCreature(playerID, Creatures.BatteringRam, true);
    }

    private bool TryTowerPlacement()
    {
        if (state != AIStates.PlacingTowers)
            return false;

        List<IntVector2> bestPath = PlayerManager.GetBestPath(playerID);

        Grid grid = PlayerManager.GetGrid(playerID);
        
        // possiblePositions and heuristicWeights has to have the same length
        List<HPosition> possiblePositions = new List<HPosition>();
        List<float> heuristicWeights = new List<float>();

        IntVector2 selectedTowerPos;

        int currentPathLength = bestPath.Count;

        // picks a random point on the best path as the starting tower placement choice
        HPosition randomPathPos = new HPosition();
        // if (TowerPlacementBackToFrontOffset >= currentPathLength)
        // {
            randomPathPos.position = bestPath[UnityEngine.Random.Range(0, currentPathLength -1)];
        // }
        // else
        // {
        //     randomPathPos.position = bestPath[currentPathLength - TowerPlacementBackToFrontOffset - 1];
        //     TowerPlacementBackToFrontOffset++;
        // }
        randomPathPos.value = 1;
        int newPathLength = Help.ValidPosition(randomPathPos.position, playerID, creatures, currentGrid);
        int deltaPathLength = newPathLength - currentPathLength;
        if (deltaPathLength < 0)
        {
            deltaPathLength = 0;
        }
        if (newPathLength != 0)
        {
            possiblePositions.Add(randomPathPos);

            heuristicWeights.Add(deltaPathLength +1);
            // heuristicWeights.Add(1);
        }

        // Check around the random Point on the best path based on TowerPlacementRange
        for (int x = randomPathPos.position.x - TowerPlacementRange; x < randomPathPos.position.x; x++)
        {
            for (int y = randomPathPos.position.y - TowerPlacementRange; y < randomPathPos.position.y; y++)
            {
                IntVector2 testPoint = new IntVector2(x, y);
                newPathLength = Help.ValidPosition(testPoint, playerID, creatures, currentGrid);
                deltaPathLength = newPathLength - currentPathLength;
                if (deltaPathLength < 0)
                {
                    deltaPathLength = 0;
                }
                if (newPathLength != 0)
                {
                    HPosition validPos = new HPosition();
                    validPos.position = testPoint;
                    // Value will be 1/(x+1) where x is the distance to the randomPathPos
                    // Value will be normalized first before setting it as the heuristic
                    float dist = testPoint.Dist(randomPathPos.position);
                    heuristicWeights.Add( (1/((dist*dist)+1)) * (deltaPathLength+1) );
                    // heuristicWeights.Add( (1/((dist*dist)+1)) );
                    possiblePositions.Add(validPos);
                }
            }
        }

        // if none of the looked positions are available, then break out of towerplacement
        if (possiblePositions.Count == 0)
        {
            return false;
        }

        // normalize and select a random tower
        Normalize(heuristicWeights);
        selectedTowerPos = possiblePositions[SelectWeightedRandom(heuristicWeights)].position;
        
        // Once a valid Tower location has been selected, find the appropriate tower to go in that location
        Towers selectedTower = BaseTowers[SelectWeightedRandom(BaseTowerDistribution)];
        Tower selectedTowerScript = gameData.GetTowerScript(selectedTower);

        //We do not try to place the tower if the AI cannot afford the tower.
        if (PlayerManager.GetBalance(playerID) >= selectedTowerScript.price)
        {
            bool placed = PlayerManager.PlaceTower(playerID, playerID, selectedTowerPos, Quaternion.identity, selectedTowerScript.gameObject, null, "", 0);

            if (placed)
            {
                towersInPlay += 1;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            // Not enough money to place the tower
            return false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        actionTimer.Update();
    }
}
