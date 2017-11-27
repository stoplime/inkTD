using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSpawnButton : MonoBehaviour
{

    /// <summary>
    /// Gets or sets the creature that will be spawned.
    /// </summary>
    public Creatures CreatureBeingSpawned { get; set; }

    /// <summary>
    /// Gets or sets the creature queuer.
    /// </summary>
    public CreatureQueuer Queuer { get; set; }

    // Use this for initialization
    void Start()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        Queuer.DeleteButtonByInstanceID(GetInstanceID(), true);
    }
}
