using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureQueuer : MonoBehaviour
{

    public GameObject contentUIElement;

    public GameObject creatureButtonPrefab;

    public RectTransform queueProgressBar;

    public int playerID = 0;


    private TaylorTimer progressTimer;
    private float maxProgressBarLength;

    private double currentWaitTime = 0f;
    private double currentPercentageComplete;

    private LinkedList<CreatureSpawnButton> buttons = new LinkedList<CreatureSpawnButton>();

    private GameLoader gameLoader;

    // Use this for initialization
    void Start()
    {
        maxProgressBarLength = queueProgressBar.sizeDelta.x;
        queueProgressBar.sizeDelta = new Vector2(0, queueProgressBar.sizeDelta.y);

        progressTimer = new TaylorTimer(50);
        progressTimer.Elapsed += ProgressTimer_Elapsed;

        gameLoader = Help.GetGameLoader();
    }

    private void ProgressTimer_Elapsed(object sender, System.EventArgs e)
    {
        if (buttons.Count == 0)
            return;

        currentWaitTime += progressTimer.TargetTimeMilliseconds;
        currentPercentageComplete = currentWaitTime / PlayerManager.GetCreatureSpawnTime(playerID);

        //Updating the progress bar:
        if (queueProgressBar != null)
        {
            queueProgressBar.sizeDelta = new Vector2((float)(maxProgressBarLength * currentPercentageComplete), queueProgressBar.sizeDelta.y);
        }

        //Spawn a creature:
        if (currentPercentageComplete > 1)
        {
            currentWaitTime = 0;

            queueProgressBar.sizeDelta = new Vector2(0, queueProgressBar.sizeDelta.y);

            PlayerManager.CreateCreature(playerID, buttons.First.Value.CreatureBeingSpawned, false);
            DequeueTop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buttons.Count != 0)
        {
            progressTimer.Update();
        }
    }

    /// <summary>
    /// Dequeues to top (next) button in the queue.
    /// </summary>
    public void DequeueTop()
    {
        Destroy(buttons.First.Value.gameObject);
        buttons.RemoveFirst();
    }

    /// <summary>
    /// Deletes a button from the creature queue.
    /// </summary>
    /// <param name="instanceID"></param>
    public void DeleteButtonByInstanceID(int instanceID, bool refund)
    {
        LinkedListNode<CreatureSpawnButton> it;

        for (it = buttons.First; it != buttons.Last; it = it.Next)
        {
            if (it.Value.GetInstanceID() == instanceID)
            {
                Destroy(it.Value.gameObject);
                buttons.Remove(it);

                if (refund)
                {
                    PlayerManager.AddBalance(playerID, gameLoader.GetCreatureScript(it.Value.CreatureBeingSpawned).price);
                }
            }
        }
    }

    /// <summary>
    /// Adds a button for a creature to eventually be spawned to the queue.
    /// </summary>
    /// <param name="creatureToSpawn">The creature that will be created and spawned.</param>
    public void AddButton(Creatures creatureToSpawn)
    {
        GameObject newButton = Instantiate(creatureButtonPrefab, contentUIElement.transform);
        CreatureSpawnButton button = newButton.GetComponent<CreatureSpawnButton>();
        button.CreatureBeingSpawned = creatureToSpawn;
        button.Queuer = this;

        Image image = newButton.GetComponent<Image>();
        image.sprite = gameLoader.GetCreatureSprite(creatureToSpawn);

        buttons.AddLast(button);
    }
}
