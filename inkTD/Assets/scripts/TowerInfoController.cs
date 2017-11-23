using helper;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoController : MonoBehaviour
{

    public GameObject content;
    public Text statText;
    public Text description;
    public Image towerPreview;


    private GameLoader gameLoader;
    private StringBuilder builder;

    // Use this for initialization
    void Start ()
    {
        gameLoader = Help.GetGameLoader();
	}

    /// <summary>
    /// Sets the tower info to correspond to the given tower.
    /// </summary>
    /// <param name="tower">The given tower whose info will be displayed.</param>
    public void SetTower(Towers tower)
    {
        Tower script = gameLoader.GetTowerScript(tower);
        towerPreview.sprite = gameLoader.GetTowerSprite(tower);

        builder = new StringBuilder();
        builder.Append(script.maxHealth.ToString() + "HP");
        builder.AppendLine(script.range.ToString() + " Range");
        builder.AppendLine(script.speed.ToString() + " Projectiles/Sec");
        builder.AppendLine(script.damage.ToString() + " Damage");
        builder.AppendLine("$" + script.price.ToString());

        statText.text = builder.ToString();

        description.text = script.description;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
