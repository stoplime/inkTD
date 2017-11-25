using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;
using UnityEngine.EventSystems;

public class Tower : InkObject
{
    [Header("General Settings")]

    [Tooltip("The towers type that this tower represents.")]
    public Towers towerType;

    [Tooltip("The distance in all directions around the tower in world coordinates that can be fired upon. Example: 5")]
    public float range;

    /// <summary>
    /// This variable is no longer used after the tower/creature has been created. 
    /// </summary>
    [Tooltip("The position along the playing grid's x axis.")]
    public int initialGridPositionX = 0;

    /// <summary>
    /// This variable is no longer used after the tower/creature has been created.
    /// </summary>
    [Tooltip("The position along the playing grid's y axis.")]
    public int initialGridPositionY = 0;

    [Tooltip("The target the tower will attempt to fire at.")]
    public TargetTypes priorityTarget = TargetTypes.First;

    [Tooltip("The area of effect radius of the project when it collides with a target.")]
    public float projectileAreaRadius = 5;

    // might not want to use this, I'd rather have the projectiles themselves calculate whether they have collided or not. <- Too expensive if there's a bunch of projectiles (>1000)
    [Tooltip("The total lifetime in milliseconds the projectile will live.")]
    public float projectileLife = 1000;

    [Tooltip("The object that is shot when this tower attacks.")]
    public GameObject projectileObject;

    [Tooltip("The height at which projectiles are spawned.")]
    public float projectileSpawnHeight;

    [Tooltip("The sound effect that plays whenever a projectile is fired from this tower.")]
    public AudioClip shootSoundEffect;

    [Tooltip("If true the bezier curve representing the firing arc of the tower will be shown. This requires a BezierVisualizer script attached.")]
    public bool visualizeBezier = false;

    [Tooltip("If true the cicle of the tower will be visualized and rendered.")]
    public bool visualizeRadius = false;

    public CircleMeshCreator radiusVisualizer;

    [Header("Special Settings")]
    [Tooltip("If true, the tower can change available positions in the grid. If false it will have no affect to the grid it is placed within.")]
    public bool existsInGrid = true;

    public static bool isSelected;

    public Camera towerCam;

    /// <summary>
    /// Gets or sets the target this tower is aiming at.
    /// </summary>
    public GameObject Target
    {
        get { return target; }
        set { SetTarget(value); }
    }

    /// <summary>
    /// Sets this tower's position along the playing grid x axis.
    /// </summary>
    public int GridPositionX
    {
        get { return gridPositionX; }
        set { SetTowerPosition(value, gridPositionY); }
    }

    /// <summary>
    /// Sets this tower's position along the playing grid's y axis.
    /// </summary>
    public int GridPositionY
    {
        get { return gridPositionY; }
        set { SetTowerPosition(gridPositionX, value); }
    }

    /// <summary>
    /// Gets the height of the tower.
    /// </summary>
    public float Height
    {
        get { return towerBounds.max.y; }
    }

    /// <summary>
    /// Gets the bounding box of the tower.
    /// </summary>
    public Bounds Bounds
    {
        get { return towerBounds; }
    }

    private GameObject target = null;
    private Creature targetCreature = null;
    private MeshRenderer targetRenderer = null;
    //TODO: Cache the creature script component of the target object, to check when health is gone the target is returned to null.

    private List<Creature> creatures;

    private TaylorTimer timer;
    private AudioSource audioSource;
    private Bounds towerBounds;

    /// <summary>
    /// The area/radius collider the tower uses to gather targets.
    /// </summary>
    private SphereCollider towerTargetArea;

    private List<Creature> nearbyCreatures = new List<Creature>(8);

    private BezierVisualizer visualizer;

    private Vector3 spawnPos;
    private Vector3 projectileSize;

    private int gridPositionX;
    private int gridPositionY;

    private float height;

    private float creatureDist = 0f;
    private int rangeRounded;

    /// <summary>
    /// A list of targets a tower can target.
    /// </summary>
    public enum TargetTypes
    {
        First = 0,
        Last = 1,
        LowestLife = 2,
        HighestLife = 3,
    }

    void Awake()
    {
        gridPositionX = initialGridPositionX;
        gridPositionY = initialGridPositionY;
    }
    
	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        // TODO: Add a random offset so the towers wont fire in sync cause it's loud as hell.
        timer = new TaylorTimer((60000 + Random.Range(0, 80)) / speed);
        timer.Elapsed += Timer_Elapsed;
        
        audioSource = GetComponent<AudioSource>();

        CalculateBounds();

        SetTowerPosition(gridPositionX, gridPositionY);

        visualizer = gameObject.GetComponent<BezierVisualizer>();

        if (projectileObject != null)
        {
            projectileSize = projectileObject.transform.Find("Projectile_Front").gameObject.GetComponent<MeshRenderer>().bounds.size;//projectileObject.GetComponentInChildren<MeshRenderer>().bounds.size;
        }

        SetSpawnPos();

        if (visualizeBezier)
        {
            VisualizeBezier();
        }

        height = towerBounds.max.y;

        creatures = PlayerManager.GetCreatures(ownerID);

        if (radiusVisualizer != null)
        {
            radiusVisualizer.Range = range;
        }

        towerTargetArea = GetComponent<SphereCollider>();

        rangeRounded = (int)(range + 0.5f);

        //TEST ONLY:
        Modifiers.Add(new Modifier(ModiferTypes.Fire, 1));
        Modifiers.Add(new Modifier(ModiferTypes.Ice, 1));
        Modifiers.Add(new Modifier(ModiferTypes.Acid, 1));
    }

    private void CalculateBounds()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            towerBounds = renderer.bounds;
        }
        else
        {
            towerBounds = new Bounds(transform.position, Vector3.zero);
            GameObject child;
            for (int i = 0; i < transform.childCount; i++)
            {
                child = transform.GetChild(i).gameObject;
                if (child.tag == "Tower")
                {
                    towerBounds.Encapsulate(child.GetComponent<MeshRenderer>().bounds);
                }
            }
        }
    }

    private void SetSpawnPos()
    {
        spawnPos = towerBounds.center;
        spawnPos.y += projectileSpawnHeight;
    }

    private void ValidateUpdate()
    {
        if (Application.isPlaying && timer != null && (speed < timer.TargetTime * 60000 + 0.0001 || speed > timer.TargetTime * 60000 - 0.0001))
            timer.TargetTime = 60000 / speed;

        SetTowerPosition(initialGridPositionX, initialGridPositionY);

        SetSpawnPos();

        if (radiusVisualizer != null)
        {
            radiusVisualizer.Range = range / transform.lossyScale.x;
            if (radiusVisualizer.isActiveAndEnabled != visualizeRadius)
                radiusVisualizer.gameObject.SetActive(visualizeRadius);
        }

        rangeRounded = (int)(range + 0.5f);

        if (towerTargetArea == null)
        {
            towerTargetArea = GetComponent<SphereCollider>();
        }
        towerTargetArea.radius = range / Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.z, transform.lossyScale.y));//((transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f); //range * 0.015f;//(range / transform.lossyScale.x) * 0.70f; //A+ code

        VisualizeBezier();
    }

    void OnTriggerEnter(Collider col)
    {
        Creature creature = col.gameObject.GetComponent<Creature>();
        if (creature != null && creature.gridID == ownerID) //Ensure this tower only ever attacks creatures with a gridID that matches its owner id.
        {
            nearbyCreatures.Add(creature);
        }
    }

    void OnTriggerExit(Collider col)
    {
        Creature creature = col.gameObject.GetComponent<Creature>();
        if (creature != null)
        {
            for (int i = nearbyCreatures.Count - 1; i >= 0; i--)
            {
                if (nearbyCreatures[i].UniqueID == creature.UniqueID)
                {
                    nearbyCreatures.RemoveAt(i);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Event that runs when the tower is selected.
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        GameLoader info = Help.GetGameLoader(); //Could cache this.

        GameObject leftRightMenu = GameObject.FindGameObjectWithTag("TowerSelectMenuHor");
        GameObject upDownMenu = GameObject.FindGameObjectWithTag("TowerSelectMenuVer");

        GameObject selectedMenu = leftRightMenu; //TODO: Determine whether the tower menu button is on left/right or up/down.

        TowerInfoController currentController = info.towerControllerCurrentLeftRight;

        currentController.SetTower(towerType);

        Tab_Handler towerButtonHandler = info.towerTabButton.GetComponent<Tab_Handler>();
        towerButtonHandler.menuScript.AlternativeMenuActive = true;
        if (!towerButtonHandler.menuScript.IsVisible)
            towerButtonHandler.menuScript.ToggleMenuRollout();
    }

    public override void OnValidate()
    {
        base.OnValidate();

        ValidateUpdate();
    }

    void OnDestroy()
    {
        if (PlayerManager.GetGrid(ownerID).getGridObject(gridPositionX, gridPositionY) == gameObject)
        {
            PlayerManager.SetGameObject(ownerID, null, gridPositionX, gridPositionY);
        }
    }

    protected override void OnOwnerChange()
    {
        base.OnOwnerChange();

        creatures = PlayerManager.GetCreatures(ownerID);
    }

    private void VisualizeBezier()
    {
        if (target != null)
        {
            Vector3 curveEnd = target.transform.position;
            Vector3 curveMid = new Vector3((spawnPos.x + curveEnd.x) / 2, gameObject.GetComponent<MeshRenderer>().bounds.max.y, (spawnPos.z + curveEnd.z) / 2);
            VisualizeBezier(spawnPos, curveMid, curveEnd);
        }
    }

    private void VisualizeBezier(Vector3 curveStart, Vector3 curveMid, Vector3 curveEnd)
    {
        if (visualizeBezier && target != null && visualizer != null)
        {
            visualizer.points = new Vector3[3];
            visualizer.points[0] = curveStart;
            visualizer.points[1] = curveMid;
            visualizer.points[2] = curveEnd;
        }
    }

    private void SetTarget(GameObject target)
    {
        this.target = target;
        targetCreature = target.GetComponent<Creature>();
        targetRenderer = target.GetComponent<MeshRenderer>();
    }

    private void FindTarget()
    {
        if (creatures.Count > 0)
        {
            target = null;

            for (int i = nearbyCreatures.Count - 1; i >= 0; i--)
            {
                if (nearbyCreatures[i] == null)
                    nearbyCreatures.RemoveAt(i);
            }

            //TODO: Refactor the code below, or at least clean it up to look nicer.
            foreach (Creature c in nearbyCreatures)
            {
                if (c.GridPosition.x > gridPositionX - rangeRounded
                    && c.GridPosition.x < gridPositionX + rangeRounded)
                {
                    creatureDist = Vector3.Distance(c.transform.position, transform.position);
                    if (creatureDist <= range)
                    {
                        if (target == null)
                        {
                            target = c.gameObject;
                        }
                        else
                        {
                            switch (priorityTarget)
                            {
                                case TargetTypes.First:
                                    if (creatureDist < Vector3.Distance(target.transform.position, transform.position))
                                    {
                                        SetTarget(c);
                                    }
                                    break;
                                case TargetTypes.HighestLife:
                                    if (targetCreature.health < c.health)
                                    {
                                        SetTarget(c);
                                    }
                                    break;
                                case TargetTypes.Last:
                                    if (creatureDist > Vector3.Distance(target.transform.position, transform.position))
                                    {
                                        SetTarget(c);
                                    }
                                    break;
                                case TargetTypes.LowestLife:
                                    if (targetCreature.health > c.health)
                                    {
                                        SetTarget(c);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void Timer_Elapsed(object sender, System.EventArgs e)
    {
        //Finding the target
        FindTarget();

        //Firing the projectile
        if (target != null)
        {
            Vector3 curveStart;
            Quaternion rotation = Quaternion.identity;
            GameObject projectile = Instantiate(projectileObject, spawnPos, rotation) as GameObject;
            Projectile_Controller controller = projectile.GetComponent<Projectile_Controller>();
            controller.SetCreator(this);
            controller.Life = projectileLife;
            controller.Target = target;
            controller.Damage = damage;
            controller.AOERadius = projectileAreaRadius;
            // may want to apply effects like fire here to the controller

            //Computing the 'rough' estimate for the projectile's arc using the spawn position
            Vector3 curveEnd = target.GetComponent<Creature>().PredictPos(projectileLife/1000); //target.transform.position;
            Vector3 curveMid = new Vector3((spawnPos.x + curveEnd.x) / 2, height, (spawnPos.z + curveEnd.z) / 2);
            controller.SetCurvePoints(spawnPos, curveMid, curveEnd);

            //Computing the actual projectile arc.
            curveStart = spawnPos + (projectile.transform.forward * (projectileSize.y * 0.75f));
            curveMid = new Vector3((curveStart.x + curveEnd.x) / 2, height, (curveStart.z + curveEnd.z) / 2);
            controller.SetCurvePoints(curveStart, curveMid, curveEnd);

            VisualizeBezier(curveStart, curveMid, curveEnd);

            //Applying the particles based on the modifiers present:
            string particleName = string.Empty;
            GameObject particle;
            foreach (Modifier m in Modifiers)
            {
                particleName = Help.GetModifierParticlePrefab(m.type);
                if (particleName != string.Empty)
                {
                    particle = Instantiate(Resources.Load("Particles/" + particleName), projectile.transform) as GameObject;
                    particle.transform.localRotation = Quaternion.Euler(-180, 0, 0);
                }
            }

            if (audioSource != null && shootSoundEffect != null)
            {
                audioSource.PlayOneShot(shootSoundEffect);
            }
        }
    }

    /// <summary>
    /// Aligns the tower to the given x and y within the playing grid.
    /// </summary>
    /// <param name="x">The x axis grid block number.</param>
    /// <param name="y">The y axis grid block number.</param>
    public void SetTowerPosition(int x, int y)
    {
        //TODO: empty the grid position at gridPositionX, gridPositionY
        // Vector3 realPos = Grid.gridToPos(new IntVector2(x, y));
        transform.position = Grid.gridToPos(new IntVector2(x, y));
        gridPositionX = x;
        gridPositionY = y;
        initialGridPositionX = x;
        initialGridPositionY = y;
        //fill the grid position at x, y
        if (existsInGrid)
            PlayerManager.SetGameObject(ownerID, gameObject, x, y);

        CalculateBounds();
    }

    /// <summary>
    /// Aligns the tower to the given x and y within the playing grid.
    /// </summary>
    /// <param name="xy">The IntVector2 grid position.</param>
    public void SetTowerPosition(IntVector2 xy)
    {
        //TODO: empty the grid position at gridPositionX, gridPositionY
        // Vector3 realPos = Grid.gridToPos(xy);
        transform.position = Grid.gridToPos(xy);
        gridPositionX = xy.x;
        gridPositionY = xy.y;
        initialGridPositionX = xy.x;
        initialGridPositionY = xy.y;
        //fill the grid position at x, y
        if (existsInGrid)
            PlayerManager.SetGameObject(ownerID, gameObject, xy.x, xy.y);

        CalculateBounds();
    }

    private void SetTarget(Creature target)
    {
        this.targetCreature = target;

        if (target == null)
            this.target = null;
        else
            this.target = target.gameObject;
    }

    // Update is called once per frame
    void Update ()
    {
        timer.Update();
    }
}
