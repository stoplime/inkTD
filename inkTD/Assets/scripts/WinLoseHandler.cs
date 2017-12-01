using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseHandler : MonoBehaviour
{

    /// <summary>
    /// Gets or sets the title text of the win/lose menu.
    /// </summary>
    public string TitleText
    {
        get { return titleText.text; }
        set
        {
            FindTitleText();
            titleText.text = value;
        }
    }

    /// <summary>
    /// Gets or sets the color of the title text.
    /// </summary>
    public Color TitleColor
    {
        get { return titleText.color; }
        set
        {
            FindTitleText();
            titleText.color = value;
        }
    }

    public Text titleText;

    public Text player1StatText;

    public Text player2StatText;

    // Use this for initialization
    void Start()
    {
        FindTitleText();

        StringBuilder builder = new StringBuilder();

        //Player 1 stats:
        builder.AppendLine("You made " + StatManager.GetStat(PlayerManager.CurrentPlayer, Stats.TowersCreated) + " towers!");
        builder.AppendLine("You upgraded " + StatManager.GetStat(PlayerManager.CurrentPlayer, Stats.TowersUpgraded) + " towers!");
        builder.AppendLine("You spent a total of " + StatManager.GetStat(PlayerManager.CurrentPlayer, Stats.InkSpent) + " ink!");
        //builder.AppendLine("You killed " + StatManager.GetStat(PlayerManager.CurrentPlayer, Stats.CreaturesKilled) + " creatures!");
        builder.AppendLine("You created " + StatManager.GetStat(PlayerManager.CurrentPlayer, Stats.CreaturesSpawned) + " creatures!");
        player1StatText.text = builder.ToString();

        builder = new StringBuilder();

        //player 2 stats:
        builder.AppendLine("Opponent made " + StatManager.GetStat(1, Stats.TowersCreated) + " towers!");
        builder.AppendLine("Opponent upgraded " + StatManager.GetStat(1, Stats.TowersUpgraded) + " towers!");
        builder.AppendLine("Opponent spent a total of " + StatManager.GetStat(1, Stats.InkSpent) + " ink!");
        //builder.AppendLine("Opponent killed " + StatManager.GetStat(1, Stats.CreaturesKilled) + " creatures!");
        builder.AppendLine("Opponent created " + StatManager.GetStat(1, Stats.CreaturesSpawned) + " creatures!");
        player2StatText.text = builder.ToString();
    }

    private void FindTitleText()
    {
        if (titleText == null)
        {
            titleText = transform.Find("Title").GetComponent<Text>();
        }
    }

    public void EnableMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
