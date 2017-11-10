using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public static int levelNum;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Quit()
    {
        Application.Quit();
    }

    public void SetLevelNum(int levelNum)
    {
        MainMenuScript.levelNum = levelNum;
    }

    public void LoadLevel()
    {
        switch(levelNum)
        {
            case -1:
                SceneManager.LoadScene(0);
                break;
            case 1:
                SceneManager.LoadScene(1);
                break;
            default:
                Debug.Log("No Level Selected!");
                break;
        }
    }

    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}
