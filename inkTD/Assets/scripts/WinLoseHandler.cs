using System.Collections;
using System.Collections.Generic;
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

    private Text titleText;

	// Use this for initialization
	void Start ()
    {
        FindTitleText();
	}

    private void FindTitleText()
    {
        if (titleText == null)
        {
            titleText = transform.Find("Title").GetComponent<Text>();
        }
    }

	
	// Update is called once per frame
	//void Update ()
 //   {
		
	//}
}
