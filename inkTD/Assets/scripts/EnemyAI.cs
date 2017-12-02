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
    ApplyModifiers = 4,
    RemoveTowers = 5
}

public class EnemyAI : MonoBehaviour {

    [Tooltip("The player ID for this AI, this AI will assume the role of a player with the given ID.")]
	public int playerID;

    [Tooltip("How fast the AI will react to different decisions. Value in Miliseconds")]
    public int TimerValue = 500;

    [Tooltip("The Range of towers to be checked before placing. Making this large can take too long and place towers way too far off the main path.")]    
    public int TowerPlacementRange = 3;

    [Tooltip("How fast the AI Creature spawns in milliseconds")]
    public int CreatureSpawnTime = 750;

    [Tooltip("The chance that the AI will choose an optimal tower placement on the first interation through the main path.")]
    public float OptimizeTowerPath = 0.8f;

    [Tooltip("A custom tower selection distribution, leave blank for a random distribution. Should have a length of the number of base towers available")]
    public List<float> CustomBaseTowerDistribution;


	private Grid currentGrid;
    private List<Creature> creatures;

    private TaylorTimer actionTimer;

    private TaylorTimer creatureQueue;

    private AIStates state = AIStates.Idle;
    private List<float> stateWeights;

    private GameLoader gameData;

    private Dictionary<IntVector2, float> TowerPathIntersects;

    // Represents the likelyhood of selecting which base tower on tower placement, has length number of base towers
    private List<float> BaseTowerDistribution;
    private List<Towers> BaseTowers;

    private int towersInPlay = 0;

    private int TowerPlacementBackToFrontOffset = 0;

    // For creature spawning hoards
    private Queue<Creatures> SpawnCreaturesWave;

	// Use this for initialization
	void Start ()
    {
        TowerPathIntersects = new Dictionary<IntVector2, float>();

        actionTimer = new TaylorTimer(TimerValue);
        actionTimer.Elapsed += ActionTimer_Elapsed;

        stateWeights = new List<float>();
        stateWeights.Add(0.1f);  // 10% Idle
        stateWeights.Add(0.6f);  // 60% Place Tower
        stateWeights.Add(0.05f); // 5% Upgrade Towers
        stateWeights.Add(0.1f);  // 10% Spawn a Creaure Wave
        stateWeights.Add(0.05f); // 5% Applying Modifiers
        stateWeights.Add(0.1f);  // 10% Removing useless Towers

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

        creatureQueue = new TaylorTimer(CreatureSpawnTime);
        creatureQueue.Elapsed += CreatureQueueTimer_Elapsed;

        SpawnCreaturesWave = new Queue<Creatures>();
        
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
                stateWeights[(int)AIStates.PlacingTowers] -= 0.001f;
            }
        }
        else if (state == AIStates.SpawningCreatures)
        {
            ComputeCreatureSpawn();
        }
        else if (state == AIStates.UpgradeTowers)
        {
            ComputeUpgradeTowers();
        }
        else if (state == AIStates.RemoveTowers)
        {
            if (!TryTowerRemoval())
            {

            }
        }
    }

    // Timer update function, runs the Queue for creature spawning
    private void CreatureQueueTimer_Elapsed(object sender, System.EventArgs e)
    {
        SpawnNextCreature();
    }

    private void SpawnNextCreature()
    {
        if (SpawnCreaturesWave.Count > 0)
        {
            PlayerManager.CreateCreature(playerID, SpawnCreaturesWave.Dequeue(), false);
        }
    }

    private void ComputeCreatureSpawn()
    {
        if (state != AIStates.SpawningCreatures)
            return;
        
        // budget spending for creatures will be a random between 30% to 80% of ink money
        float budget = UnityEngine.Random.Range(0.3f, 0.8f) * PlayerManager.GetBalance(playerID); // UnityEngine.Random.Range(0.3f, 0.8f) *
        List<Creatures> affordableCreatures = gameData.GetAffordableCreatures(budget * 0.5f);
        List<float> creatureWeights;

        while (affordableCreatures.Count > 0)
        {
            creatureWeights = new List<float>();
            for (int i = affordableCreatures.Count - 1; i >= 0; i--)
            {
                creatureWeights.Add(1/((i+1f)*(i+1f)));
            }
            creatureWeights.Reverse();
            Normalize(creatureWeights);
            Creatures selectedCreature = affordableCreatures[affordableCreatures.Count - SelectWeightedRandom(creatureWeights) -1];
            float creaturePrice = gameData.GetCreatureScript(selectedCreature).price;
            float halfBudget = budget * 0.5f;
            while(budget >= halfBudget)
            {
                budget -= creaturePrice;
                PlayerManager.AddBalance(playerID, -creaturePrice);
                SpawnCreaturesWave.Enqueue(selectedCreature);
            }
            affordableCreatures = gameData.GetAffordableCreatures(halfBudget);
        }
        // once a list of creatures is made, set the queue to spawn them
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
        bool firstRun = false;
        HPosition randomPathPos = new HPosition();
        if (TowerPlacementBackToFrontOffset > currentPathLength /2 )
        {
            randomPathPos.position = bestPath[UnityEngine.Random.Range(0, currentPathLength -1)];
            TowerPlacementBackToFrontOffset = 1000000;
        }
        else
        {
            randomPathPos.position = bestPath[currentPathLength - TowerPlacementBackToFrontOffset -1];
            firstRun = true;
        }
        randomPathPos.value = 1;
        List<IntVector2> tempBestPath;
        int newPathLength = Help.ValidPosition(randomPathPos.position, playerID, creatures, currentGrid, out tempBestPath);
        
        List<IntVector2> towerPoses;
        float currentTowerPathIntersect = SumTowerPathIntersect(out towerPoses);
        float newTowerPathIntersect = PlayerManager.GetTotalTowerPathIntersect(playerID, towerPoses, tempBestPath);

        float deltaIntersect = newTowerPathIntersect - currentTowerPathIntersect;
        if (deltaIntersect < 0)
        {
            deltaIntersect = 0;
        }
        if (newPathLength != 0)
        {
            possiblePositions.Add(randomPathPos);

            heuristicWeights.Add(deltaIntersect +1);
            // heuristicWeights.Add(1);
        }

        // Check around the random Point on the best path based on TowerPlacementRange
        if (TowerPlacementBackToFrontOffset >= currentPathLength || OptimizeTowerPath < UnityEngine.Random.value)
        {
            for (int x = randomPathPos.position.x - TowerPlacementRange; x < randomPathPos.position.x; x++)
            {
                for (int y = randomPathPos.position.y - TowerPlacementRange; y < randomPathPos.position.y; y++)
                {
                    IntVector2 testPoint = new IntVector2(x, y);
                    newPathLength = Help.ValidPosition(testPoint, playerID, creatures, currentGrid, out tempBestPath);

                    currentTowerPathIntersect = SumTowerPathIntersect(out towerPoses);
                    newTowerPathIntersect = PlayerManager.GetTotalTowerPathIntersect(playerID, towerPoses, tempBestPath);
                    
                    deltaIntersect = newTowerPathIntersect - currentTowerPathIntersect;
                    if (deltaIntersect < 0)
                    {
                        deltaIntersect = 0;
                    }
                    // else
                    // {
                    //     print(deltaIntersect);
                    // }
                    if (newPathLength != 0)
                    {
                        HPosition validPos = new HPosition();
                        validPos.position = testPoint;
                        // Value will be 1/(x+1)^2 where x is the distance to the randomPathPos
                        // Value will be normalized first before setting it as the heuristic
                        float dist = testPoint.Dist(randomPathPos.position);
                        heuristicWeights.Add( (1/((dist+1)*(dist+1))) * (deltaIntersect+1) );
                        // heuristicWeights.Add( (1/((dist*dist)+1)) );
                        possiblePositions.Add(validPos);
                    }
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
        if (BaseTowers.Count == 0)
        {
            print("Base Towers are empty");
            return false;
        }
        Towers selectedTower = BaseTowers[SelectWeightedRandom(BaseTowerDistribution)];
        Tower selectedTowerScript = gameData.GetTowerScript(selectedTower);

        //We do not try to place the tower if the AI cannot afford the tower.
        if (PlayerManager.GetBalance(playerID) >= selectedTowerScript.price)
        {
            bool placed = PlayerManager.PlaceTower(playerID, playerID, selectedTowerPos, Quaternion.identity, selectedTowerScript.gameObject, null, "", 0);

            if (placed)
            {
                towersInPlay += 1;
                TowerPathIntersects.Add(selectedTowerPos, PlayerManager.GetTowerPathIntersect(playerID, selectedTowerPos));
                // reevaluate the towerPath Intersect
                List<IntVector2> keys = new List<IntVector2>(TowerPathIntersects.Keys);
                foreach (IntVector2 key in keys)
                {
                    TowerPathIntersects[key] = PlayerManager.GetTowerPathIntersect(playerID, key);
                }
                if (firstRun)
                {
                    TowerPlacementBackToFrontOffset++;
                }
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

    private void ComputeUpgradeTowers()
    {
        if (state != AIStates.UpgradeTowers)
            return;

        List<IntVector2> towerPoses = new List<IntVector2>();
        List<float> towerUpgradeWeight = new List<float>();
        List<IntVector2> keys = new List<IntVector2>(TowerPathIntersects.Keys);
        foreach (IntVector2 key in keys)
        {
            TowerPathIntersects[key] = PlayerManager.GetTowerPathIntersect(playerID, key);
        }
        foreach (KeyValuePair<IntVector2, float> item in TowerPathIntersects)
        {
            if (item.Value > 0)
            {
                towerPoses.Add(item.Key);
                towerUpgradeWeight.Add(item.Value);
            }
        }
        if (towerPoses.Count == 0)
        {
            return;
        }
        Normalize(towerUpgradeWeight);
        IntVector2 selectedTowerPos = towerPoses[SelectWeightedRandom(towerUpgradeWeight)];

        // Get tower script from selected tower pos
        GameObject towerObject = PlayerManager.GetGrid(playerID).getGridObject(selectedTowerPos);
        Tower selectedTowerScript = towerObject.GetComponent<Tower>();
        if (selectedTowerScript != null)
        {
            List<Towers> upgrades = gameData.GetTowerUpgrades(selectedTowerScript.towerType);
            int upgradeChoice = 0;
            if (upgrades.Count == 0)
            {
                return;
            }
            else if (upgrades.Count == 1)
            {
                // Pick one
                upgradeChoice = UnityEngine.Random.Range(0, upgrades.Count-1);
            }
            if (PlayerManager.GetBalance(playerID) >= gameData.GetTowerScript(upgrades[upgradeChoice]).price)
            {
                PlayerManager.DeleteGridObject(playerID, selectedTowerPos.x, selectedTowerPos.y);
                PlayerManager.PlaceTower(playerID, playerID, selectedTowerPos, Quaternion.identity, gameData.GetTowerPrefab(upgrades[upgradeChoice]), null, "", 0);
                TowerPathIntersects[selectedTowerPos] = PlayerManager.GetTowerPathIntersect(playerID, selectedTowerPos);
            }
        }
    }

    private bool TryTowerRemoval()
    {
        if (state != AIStates.RemoveTowers)
            return false;
        
        // check through the list of towers, if the towerPathIntersect is 0 then remove the towers
        List<IntVector2> unusedTowers = new List<IntVector2>();
        foreach (KeyValuePair<IntVector2, float> item in TowerPathIntersects)
        {
            if (item.Value == 0)
            {
                unusedTowers.Add(item.Key);
            }
        }
        int selectedTowerToRemove = 0;
        if (unusedTowers.Count < 1)
            return false;
        else if (unusedTowers.Count > 1)
            selectedTowerToRemove = UnityEngine.Random.Range(0, unusedTowers.Count-1);
        
        IntVector2 selectedTower = unusedTowers[selectedTowerToRemove];
        GameObject towerObject = PlayerManager.GetGrid(playerID).getGridObject(selectedTower);
        if (towerObject == null)
            return false;
        Tower selectedTowerScript = towerObject.GetComponent<Tower>();
        if (selectedTowerScript != null)
        {
            TowerPathIntersects.Remove(selectedTower);
            PlayerManager.SellTower(playerID, selectedTower);
            towersInPlay--;
            return true;
        }
        return false;
    }

    private float SumTowerPathIntersect(out List<IntVector2> towerPoses)
    {
        float total = 0;
        towerPoses = new List<IntVector2>();
        foreach (KeyValuePair<IntVector2, float> item in TowerPathIntersects)
        {
            towerPoses.Add(item.Key);
            total += item.Value;
        }
        return total;
    }

    // Update is called once per frame
    void Update ()
    {
        actionTimer.Update();
        creatureQueue.Update();
    }
}
