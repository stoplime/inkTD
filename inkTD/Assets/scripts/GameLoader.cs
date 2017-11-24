using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    [System.Serializable]
    public class CreatureEntry : PrefabEntry
    {
        public Sprite icon;
        public Creatures creature;
    }

    [System.Serializable]
    public class TowerEntry : PrefabEntry
    {
        public Towers tower;
    }

    [System.Serializable]
    public class PrefabEntry
    {
        public GameObject prefab;
    }

    /// <summary>
    /// A class for the gameloader to organize data related to towers.
    /// </summary>
    private class TowerData
    {
        public TowerEntry entry;
        public Towers tower;
        public Sprite towerSnapShot;
        public Tower towerScript;
        //TODO: Add the upgrade path here
    }

    /// <summary>
    /// A class for the gameloader to organize data related to creatures.
    /// </summary>
    private class CreatureData
    {
        public CreatureEntry entry;
        public Creature creatureScript;
    }

    [Header("Scene-Specific Info:")]
    [Space(20)]
    public GameObject leftRightUpgradeMenuContent;
    public GameObject upDownUpgradeMenuContent;
    public TowerInfoController towerControllerCurrentLeftRight;
    public TowerInfoController towerControllerCurrentUpDown;

    public GameObject towerTabButton;

    [Header("Basic Settings:")]
    [Space(20)]
    [Tooltip("The camera used to take snapshots of the towers for their sprites.")]
    public Camera snapshotCamera;

    [Tooltip("The distance from the camera the tower prefabs will be placed before taking a snapshot.")]
    public float towerSnapshotDistance = 6f;

    public string snapshotLayer = "TowerSnapShot";

    public int snapshotWidth = 128;
    public int snapshotHeight = 128;

    [Tooltip("8, 16, or 24 pixel depth.")]
    public int snapshotDepth = 24;

    [Tooltip("The sprite used in the event no sprite is available for a creature.")]
    public Sprite defaultCreatureSprite;

    [Header("Prefab Entries:")]
    [Space(20)]
    public CreatureEntry[] creatureEntries;

    public TowerEntry[] towerEntries;

    private Dictionary<Towers, TowerData> towers = new Dictionary<Towers, TowerData>();

    private Dictionary<Creatures, CreatureData> creatures = new Dictionary<Creatures, CreatureData>();

    private int snapshotLayerNumber = 0;

    // Use this for initialization
    void Awake ()
    {
        snapshotLayerNumber = LayerMask.NameToLayer(snapshotLayer);
        
        //Loading in creatures:
        CreatureData creatureData;
        for (int i = 0; i < creatureEntries.Length; i++)
        {
            if (creatureEntries[i].prefab != null)
            {
                creatureData = new CreatureData();
                creatureData.entry = creatureEntries[i];
                creatureData.creatureScript = creatureEntries[i].prefab.GetComponent<Creature>();

                if (creatureData.entry.icon == null)
                    creatureData.entry.icon = defaultCreatureSprite;

                creatures.Add(creatureEntries[i].creature, creatureData);
            }
        }
        
        //Loading towers and creating their snapshots:
        Sprite snapshotResult;
        RenderTexture render = new RenderTexture(snapshotWidth, snapshotHeight, snapshotDepth);
        Texture2D texture;
        TowerData data;
        GameObject snapshotTower;
        Tower script;
        int prevLayer;

        if (snapshotCamera == null)
            throw new System.Exception("GameLoader script is missing a snapshot camera!");

        for (int i = 0; i < towerEntries.Length; i++)
        {
            if (towerEntries[i].prefab != null)
            {
                script = towerEntries[i].prefab.GetComponent<Tower>();
                script.towerType = towerEntries[i].tower;

                snapshotTower = Instantiate(towerEntries[i].prefab);

                prevLayer = snapshotTower.layer;
                snapshotTower.transform.position = snapshotCamera.transform.position + snapshotCamera.transform.forward * towerSnapshotDistance;
                //NOTE: Since tower pivot point is at the bottom we must offset it. 1.5 is roughly hard coded since 2 doesn't result in the tower being centered in its snapshot.
                snapshotTower.transform.position -= snapshotTower.transform.up * (script.Height / 2f); 
                snapshotTower.layer = snapshotLayerNumber;
                snapshotCamera.targetTexture = render;
                snapshotCamera.Render();
                snapshotTower.layer = prevLayer;

                RenderTexture.active = render;
                texture = new Texture2D(snapshotWidth, snapshotHeight);
                texture.ReadPixels(new Rect(0, 0, render.width, render.height), 0, 0);
                texture.Apply();
                
                snapshotResult = Sprite.Create(texture, new Rect(0, 0, snapshotWidth, snapshotHeight), Vector2.zero);
                snapshotResult.name = towerEntries[i].tower.ToString() + " Tower Sprite";

                data = new TowerData();
                data.entry = towerEntries[i];
                data.tower = towerEntries[i].tower;
                data.towerScript = script;
                data.towerSnapShot = snapshotResult;

                towers.Add(towerEntries[i].tower, data);

                Destroy(snapshotTower);
            }
        }
        //Not setting the snapshot camera's target texture may result in the last tower's snapshot getting overwritten upon using the camera again.
        //snapshotCamera.targetTexture = null;
        RenderTexture.active = null;
    }

    /// <summary>
    /// Gets a snapshot of the given tower.
    /// </summary>
    /// <param name="tower">The given tower.</param>
    /// <returns>Returns a sprite of the given tower.</returns>
    public Sprite GetTowerSprite(Towers tower)
    {
        return towers[tower].towerSnapShot;
    }

    /// <summary>
    /// Gets the prefab associated with the given tower.
    /// </summary>
    /// <param name="tower">The given tower.</param>
    /// <returns>Returns a non-instantiated gameobject associated with the tower.</returns>
    public GameObject GetTowerPrefab(Towers tower)
    {
        return towers[tower].entry.prefab;
    }

    /// <summary>
    /// Gets the default script assocaited with the given tower. Note: This should not be modified.
    /// </summary>
    /// <param name="tower">The given tower.</param>
    /// <returns></returns>
    public Tower GetTowerScript(Towers tower)
    {
        return towers[tower].towerScript;
    }

    /// <summary>
    /// Gets the sprite associated with the given creature.
    /// </summary>
    /// <param name="creature">the given creature.</param>
    public Sprite GetCreatureSprite(Creatures creature)
    {
        return creatures[creature].entry.icon;
    }

    /// <summary>
    /// Gets the prefab associated with the given creature.
    /// </summary>
    /// <param name="creature">the given creature.</param>
    public GameObject GetCreaturePrefab(Creatures creature)
    {
        return creatures[creature].entry.prefab;
    }

    /// <summary>
    /// Gets the default script associated with the given creature. Note: This should not be modified.
    /// </summary>
    /// <param name="creature">The given creature.</param>
    /// <returns></returns>
    public Creature GetCreatureScript(Creatures creature)
    {
        return creatures[creature].creatureScript;
    }
    

}
