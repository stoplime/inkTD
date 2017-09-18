using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class Tower : InkObject
{

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

    // might not want to use this, I'd rather have the projectiles themselves calculate whether they have collided or not.
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

    private GameObject target = null;
    private Creature targetCreature = null;
    private MeshRenderer targetRenderer = null;
    //TODO: Cache the creature script component of the target object, to check when health is gone the target is returned to null.

    private List<Creature> creatures;

    private TaylorTimer timer;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

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
	void Start ()
    {
        timer = new TaylorTimer(60000 / speed);
        timer.Elapsed += Timer_Elapsed;

        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();

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

        height = meshRenderer.bounds.max.y;

        creatures = PlayerManager.GetCreatures(ownerID);

        if (radiusVisualizer != null)
        {
            radiusVisualizer.range = range;
        }

        rangeRounded = (int)(range + 0.5f);

        //TEST ONLY:
        Modifiers.Add(new Modifier(ModiferTypes.Fire, 1));
        Modifiers.Add(new Modifier(ModiferTypes.Ice, 1));
        Modifiers.Add(new Modifier(ModiferTypes.Acid, 1));

    }

    private void SetSpawnPos()
    {
        spawnPos = meshRenderer.bounds.center;
        spawnPos.y += projectileSpawnHeight;
    }

    private void ValidateUpdate()
    {
        if (Application.isPlaying && timer != null && (speed < timer.TargetTime * 60000 + 0.0001 || speed > timer.TargetTime * 60000 - 0.0001))
            timer.TargetTime = 60000 / speed;

        SetTowerPosition(initialGridPositionX, initialGridPositionY);

        if (meshRenderer != null)
        {
            SetSpawnPos();
        }

        if (radiusVisualizer != null)
        {
            radiusVisualizer.range = range;
            radiusVisualizer.OnValidate();

            if (radiusVisualizer.isActiveAndEnabled != visualizeRadius)
                radiusVisualizer.gameObject.SetActive(visualizeRadius);
        }

        rangeRounded = (int)(range + 0.5f);

        VisualizeBezier();
    }

    public override void OnValidate()
    {
        base.OnValidate();

        ValidateUpdate();
    }

    void OnDrawGizmos()
    { 
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

        targetRenderer = target.GetComponent<MeshRenderer>();
    }

    private void FindTarget()
    {
        if (creatures.Count > 0)
        {
            target = null;

            foreach (Creature c in creatures)
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

            if (targetRenderer == null)
            {
                controller.TargetPosition = target.transform.position;
            }
            else
            {
                controller.TargetPosition = targetRenderer.bounds.center;
            }

            //Computing the 'rough' estimate for the projectile's arc using the spawn position
            Vector3 curveEnd = target.transform.position;
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
                //audioSource.PlayOneShot(shootSoundEffect, Help.TowerSoundEffectVolume);
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
        //empty the grid position at gridPositionX, gridPositionY
        Vector3 realPos = Grid.gridToPos(new IntVector2(x, y));
        transform.position = new Vector3(realPos.x, transform.position.y, realPos.z);
        gridPositionX = x;
        gridPositionY = y;
        initialGridPositionX = x;
        initialGridPositionY = y;
        //fill the grid position at x, y
        PlayerManager.SetGameObject(ownerID, gameObject, x, y);
    }

    /// <summary>
    /// Aligns the tower to the given x and y within the playing grid.
    /// </summary>
    /// <param name="xy">The IntVector2 grid position.</param>
    public void SetTowerPosition(IntVector2 xy)
    {
        //empty the grid position at gridPositionX, gridPositionY
        Vector3 realPos = Grid.gridToPos(xy);
        transform.position = new Vector3(realPos.x, transform.position.y, realPos.z);
        gridPositionX = xy.x;
        gridPositionY = xy.y;
        initialGridPositionX = xy.x;
        initialGridPositionY = xy.y;
        //fill the grid position at x, y
        PlayerManager.SetGameObject(ownerID, gameObject, xy.x, xy.y);
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
