using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public TowerData(){}
        public TowerData(Towers tower)
        {
            this.tower = tower;
        }
        private static bool calculateEquals(TowerData obj1, TowerData obj2)
        {
            if (obj1.tower.Equals(obj2.tower))
            {
                return true;
            }
            return false;
        }

        public static bool operator ==(TowerData obj1, TowerData obj2)
        {
            return calculateEquals(obj1, obj2);
        }
        
        public static bool operator !=(TowerData obj1, TowerData obj2)
        {
            return !calculateEquals(obj1, obj2);
        }

        public override bool Equals(object obj)
        {
            return calculateEquals(this, obj as TowerData);
        }
        public override int GetHashCode() 
        {
            return tower.GetHashCode();
        }
    }

    /// <summary>
    /// A class for the gameloader to organize data related to creatures.
    /// </summary>
    private class CreatureData
    {
        public CreatureEntry entry;
        public Creature creatureScript;
    }

    /// <summary>
    /// Tree structure which holds the upgrade tree of the towers
    /// </summary>
    private class TowerNode<T> where T : TowerData
    {
        public T data;
        public List<TowerNode<T>> children;

        public TowerNode(T data)
        {
            this.data = data;
            children = new List<TowerNode<T>>();
        }

        public void AddNode(T data)
        {
            children.Add(new TowerNode<T>(data));
        }

        public int GetChildrenCount()
        {
            return children.Count;
        }

        public TowerNode<T> GetChild(int i)
        {
            if (i < children.Count && i >= 0)
                return children[i];
            return null;
        }

        public TowerNode<T> this[Towers id]
        {
            get
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].data.tower == id)
                    {
                        return children[i];
                    }
                    else
                    {
                        TowerNode<T> find = children[i][id];
                        if (find != null)
                        {
                            return find;
                        }
                    }
                }
                return null;
            }
        }
        public TowerNode<T> this[T dataValue]
        {
            get
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].data == dataValue)
                    {
                        return children[i];
                    }
                    else
                    {
                        TowerNode<T> find = children[i][dataValue];
                        if (find != null)
                        {
                            return find;
                        }
                    }
                }
                return null;
            }
        }
    }

    //Public variables:

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

    //Properties:

    /// <summary>
    /// Gets the tower tab menu.
    /// </summary>
    public TabMenu TowerTabMenu
    {
        get { return towerTabMenu; }
    }

    //Private Variables:

    private TowerNode<TowerData> TowerUpgradeTree;

    private Dictionary<Towers, TowerData> towers = new Dictionary<Towers, TowerData>();

    private Dictionary<Creatures, CreatureData> creatures = new Dictionary<Creatures, CreatureData>();

    private Dictionary<string, Sprite> cachedSnapShots = new Dictionary<string, Sprite>();

    private int snapshotLayerNumber = 0;

    private TabMenu towerTabMenu;

    private Quaternion defaultSnapShotCameraRotation;
    private Vector3 defaultSnapShotCameraPosition;
    
    // Use this for initialization
    void Awake ()
    {
        snapshotLayerNumber = LayerMask.NameToLayer(snapshotLayer);
        
        if (snapshotCamera != null)
        {
            defaultSnapShotCameraPosition = snapshotCamera.transform.position;
            defaultSnapShotCameraRotation = snapshotCamera.transform.rotation;
        }

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

        LoadCreatureStats();

        Tower script;
        TowerData data;
        for (int i = 0; i < towerEntries.Length; i++)
        {
            if (towerEntries[i].prefab != null)
            {
                script = towerEntries[i].prefab.GetComponent<Tower>();
                script.towerType = towerEntries[i].tower;

                data = new TowerData();
                data.entry = towerEntries[i];
                data.tower = towerEntries[i].tower;
                data.towerScript = script;
                towers.Add(towerEntries[i].tower, data);
            }
        }

        //Builds the tree design for the tower upgrades
        if (TowerUpgradeTree == null)
        BuildTowerUpgradeTree(out TowerUpgradeTree);

        if (towerTabButton != null)
        {
            towerTabMenu = towerTabButton.GetComponent<TabMenu>();
        }
    }

    void Start()
    {
        TakeTowerSnapShots();
    }

    private void LoadCreatureStats()
    {
        try
        {
            TextAsset txt = (TextAsset)Resources.Load("CreatureStats", typeof(TextAsset));
            string content = txt.text;
            string[] fields = txt.text.Split('\t');

            string name;
            Creatures creatureType;
            float health;
            float damage;
            float speed;
            float price;
            int inkcome;
            int droppedInk;
            //Starts at 7 because the first line is just a header.
            for (int i = 7; i < fields.Length - 7; i += 7) //Is the last creature making it through?
            {
                name = fields[i].Replace('\r', ' ').Replace('\n',' ').Trim();
                health = float.Parse(fields[i + 1]);
                damage = float.Parse(fields[i + 2]);
                speed = float.Parse(fields[i + 3]);
                price = float.Parse(fields[i + 4]);
                inkcome = int.Parse(fields[i + 5]);
                droppedInk = int.Parse(fields[i + 6]);
                creatureType = (Creatures)Enum.Parse(typeof(Creatures), name, true);

                if (creatures.ContainsKey(creatureType))
                {
                    creatures[creatureType].creatureScript.creatureType = creatureType;
                    creatures[creatureType].creatureScript.maxHealth = health;
                    creatures[creatureType].creatureScript.health = health;
                    creatures[creatureType].creatureScript.damage = damage;
                    creatures[creatureType].creatureScript.speed = speed;
                    creatures[creatureType].creatureScript.price = price;
                    creatures[creatureType].creatureScript.inkcomeValue = inkcome;
                    creatures[creatureType].creatureScript.dropInk = droppedInk;
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error loading creature stats: " + e.Message);
        }
    }

    private void TakeTowerSnapShots()
    {
        //Loading towers and creating their snapshots:
        Sprite snapshotResult;
        RenderTexture render = new RenderTexture(snapshotWidth, snapshotHeight, snapshotDepth);
        Texture2D texture;
        GameObject snapshotTower;
        int prevLayer;
        Vector3 pos;

        if (snapshotCamera == null)
            throw new System.Exception("GameLoader script is missing a snapshot camera!");

        snapshotCamera.transform.position = defaultSnapShotCameraPosition;
        snapshotCamera.transform.rotation = defaultSnapShotCameraRotation;

        
        for (int i = 0; i < towerEntries.Length; i++)
        {
            if (towerEntries[i].prefab != null)
            {
                snapshotTower = Instantiate(towerEntries[i].prefab);

                prevLayer = snapshotTower.layer;

                pos = snapshotCamera.transform.position;
                //if (Terrain.activeTerrain != null)
                //    pos.y = Terrain.activeTerrain.SampleHeight(pos);
                pos += snapshotCamera.transform.forward * towerSnapshotDistance;

                snapshotTower.transform.position = pos;
                //NOTE: Since tower pivot point is at the bottom we must offset it. 1.5 is roughly hard coded since 2 doesn't result in the tower being centered in its snapshot.
                snapshotTower.transform.position -= snapshotTower.transform.up * (towers[towerEntries[i].tower].towerScript.Bounds.extents.y / 1.1f);
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
                
                towers[towerEntries[i].tower].towerSnapShot = snapshotResult;
                Destroy(snapshotTower);
            }
        }
        //Not setting the snapshot camera's target texture may result in the last tower's snapshot getting overwritten upon using the camera again.
        //snapshotCamera.targetTexture = null;
        RenderTexture.active = null;
    }

    /// <summary>
    /// Takes a snapshot of an object.
    /// </summary>
    /// <param name="obj">The object having a snapshot taken of.</param>
    /// <param name="name">The name of the snapshot, if cached this will need to be known.</param>
    /// <param name="cache">If true the gameloader will cache the snapshot for use later.</param>
    public Sprite TakeSnapShotOf(GameObject obj, string name, float snapShotDistance, bool cache)
    {
        return TakeSnapShotOf(obj, name, snapShotDistance, Vector3.zero, cache);
    }

    /// <summary>
    /// Takes a snapshot of an object.
    /// </summary>
    /// <param name="obj">The object having a snapshot taken of.</param>
    /// <param name="name">The name of the snapshot, if cached this will need to be known.</param>
    /// <param name="cache">If true the gameloader will cache the snapshot for use later.</param>
    /// <returns></returns>
    public Sprite TakeSnapShotOf(GameObject obj, string name, float snapShotDistance, Vector3 snapShotOffset, bool cache)
    {
        if (obj == null)
            throw new System.Exception("Snapshot object is null.");

        //Loading towers and creating their snapshots:
        Sprite snapshotResult = new Sprite();
        RenderTexture render = new RenderTexture(snapshotWidth, snapshotHeight, snapshotDepth);
        Texture2D texture;
        int[] prevLayers;

        prevLayers = new int[obj.transform.childCount + 1];
        prevLayers[0] = obj.layer;
        for (int i = 1; i < obj.transform.childCount; i++)
        {
            prevLayers[i] = obj.transform.GetChild(i - 1).gameObject.layer;
        }

        if (snapshotCamera == null)
            throw new System.Exception("GameLoader script is missing a snapshot camera!");

        Vector3 pos = obj.transform.position;
        if (Terrain.activeTerrain != null)
            pos.y = Terrain.activeTerrain.SampleHeight(pos);
        pos += obj.transform.forward * snapShotDistance;

        snapshotCamera.transform.position = pos;
        snapshotCamera.transform.LookAt(obj.transform);
        snapshotCamera.transform.position += snapShotOffset;
        obj.layer = snapshotLayerNumber;
        for (int i = 1; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i - 1).gameObject.layer = snapshotLayerNumber;
        }
        snapshotCamera.targetTexture = render;
        snapshotCamera.Render();

        obj.layer = prevLayers[0];
        for (int i = 1; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i - 1).gameObject.layer = prevLayers[i];
        }

        RenderTexture.active = render;
        texture = new Texture2D(snapshotWidth, snapshotHeight);
        texture.ReadPixels(new Rect(0, 0, render.width, render.height), 0, 0);
        texture.Apply();

        snapshotResult = Sprite.Create(texture, new Rect(0, 0, snapshotWidth, snapshotHeight), Vector2.zero);
        snapshotResult.name = name;

        //Not setting the snapshot camera's target texture may result in the last tower's snapshot getting overwritten upon using the camera again.
        //snapshotCamera.targetTexture = null;
        RenderTexture.active = null;
        
        if (cache)
        {
            cachedSnapShots.Add(snapshotResult.name, snapshotResult);
        }

        return snapshotResult;
    }

    /// <summary>
    /// Gets the cached sprite with the given name.
    /// </summary>
    /// <param name="name">The name of the sprite.</param>
    /// <returns></returns>
    public Sprite GetCachedSnapShot(string name)
    {
        return cachedSnapShots[name];
    }

    /// <summary>
    /// Returns true if a cached snapshot with the given name exists in the game loader.
    /// </summary>
    /// <param name="name">The name of the sprite.</param>
    /// <returns></returns>
    public bool CachedSnapShotExists(string name)
    {
        return cachedSnapShots.ContainsKey(name);
    }

    /// <summary>
    /// Changes to the Tower Upgrades go here
    /// </summary>
    private void BuildTowerUpgradeTree(out TowerNode<TowerData> tree)
    {
        tree = new TowerNode<TowerData>(new TowerData(Towers.Root));
        //Design the Tower Upgrade here
        TowerData Arrow = new TowerData(Towers.ArrowCatagory);
		tree.AddNode(Arrow);
		// Lvl 1
		TowerData Archer_Tower = new TowerData(Towers.Archer);
		tree[Arrow].AddNode(Archer_Tower);
        // Lvl 2
        TowerData Crossbow_Tower = new TowerData(Towers.Crossbow);
        tree[Archer_Tower].AddNode(Crossbow_Tower);
        // Lvl 3
        TowerData Musket_Tower = new TowerData(Towers.Musket);
        tree[Crossbow_Tower].AddNode(Musket_Tower);

        // Root Tower Cannon Type
        TowerData Cannon = new TowerData(Towers.CannonCatagory);
		tree.AddNode(Cannon);
		// Lvl 1
		TowerData Catapult_Tower = new TowerData(Towers.Catapult);
		tree[Cannon].AddNode(Catapult_Tower);
		// Lvl 2
		TowerData Trebuchet_Tower = new TowerData(Towers.Trebuchet);
		tree[Catapult_Tower].AddNode(Trebuchet_Tower);
		// Lvl 3
		TowerData Cannonball_Tower = new TowerData(Towers.Cannonball);
		tree[Trebuchet_Tower].AddNode(Cannonball_Tower);
		// Lvl 4 Split Cannonball_Tower to Bomb_Tower path and Anvil_Tower path
		TowerData Bomb_Tower = new TowerData(Towers.Bomb);
		tree[Cannonball_Tower].AddNode(Bomb_Tower);


		// Root Tower Magic Type
		TowerData Magic = new TowerData(Towers.MagicCatagory);
		tree.AddNode(Magic);
		// Lvl 1
		TowerData Magic_Tower = new TowerData(Towers.Magic);
		tree[Magic].AddNode(Magic_Tower);
		// Lvl 2
		TowerData Mystic_Tower = new TowerData(Towers.Mystic);
		tree[Magic_Tower].AddNode(Mystic_Tower);
		// Lvl 3
		TowerData Arcane_Tower = new TowerData(Towers.Arcane);
		tree[Mystic_Tower].AddNode(Arcane_Tower);
		// Lvl 4 Split Arcane_Tower to Psionics_Towerpath and Mana_Tower path
		TowerData Psionics_Tower = new TowerData(Towers.Psionics);
		tree[Arcane_Tower].AddNode(Psionics_Tower);
		TowerData Mana_Tower = new TowerData(Towers.Mana);
		tree[Arcane_Tower].AddNode(Mana_Tower);
		// Lvl 5
		TowerData Brain_Tower = new TowerData(Towers.Brain);
		tree[Psionics_Tower].AddNode(Brain_Tower);
		TowerData Aether_Tower = new TowerData(Towers.Aether);
		tree[Mana_Tower].AddNode(Aether_Tower);
		// Lvl 6
		TowerData Enlightenment_Tower = new TowerData(Towers.Enlightenment);
		tree[Brain_Tower].AddNode(Enlightenment_Tower);
		TowerData Wizard_Tower = new TowerData(Towers.Wizard);
		tree[Aether_Tower].AddNode(Wizard_Tower);
    }

    /// <summary>
    /// Returns a list of base towers available
    /// </summary>
    /// <returns></returns>
    public List<Towers> GetBaseTowers()
    {
        if (TowerUpgradeTree == null)
            BuildTowerUpgradeTree(out TowerUpgradeTree);
        List<Towers> bases = new List<Towers>();
        // Go through all catagories
        foreach (TowerNode<TowerData> catagory in TowerUpgradeTree.children)
        {
            // Go through all base towers in a catagory
            // should only be one tower for now but just in case I added a loop
            foreach (TowerNode<TowerData> baseTower in catagory.children)
            {
                bases.Add(baseTower.data.tower);
            }
        }
        return bases;
    }

    /// <summary>
    /// Given a tower in the upgrade tree, returns a list of towers which is its possible upgrades
    /// </summary>
    /// <param name="tower"></param>
    /// <returns></returns>
    public List<Towers> GetTowerUpgrades(Towers tower)
    {
        if (TowerUpgradeTree == null)
            BuildTowerUpgradeTree(out TowerUpgradeTree);
        // if the upgrade tree does not contain the passed in tower, then return null
        if (TowerUpgradeTree[tower] == null)
            return null;
        
        List<Towers> upgrades = new List<Towers>();
        // go through the children of tower
        foreach (TowerNode<TowerData> child in TowerUpgradeTree[tower].children)
        {
            upgrades.Add(child.data.tower);
        }
        return upgrades;
    }

    /// <summary>
    /// Gets a list of creatures that can be afforded with a given price budget
    /// </summary>
    /// <param name="price"></param>
    /// <returns></returns>
    public List<Creatures> GetAffordableCreatures(float price)
    {
        List<Creatures> affordables = new List<Creatures>();
        foreach (KeyValuePair<Creatures, CreatureData> item in creatures)
        {
            if (item.Value.creatureScript.price <= price)
            {
                affordables.Add(item.Key);
            }
        }
        affordables.Sort();
        return affordables;
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
