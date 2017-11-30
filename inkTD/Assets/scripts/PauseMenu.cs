using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Button pauseButton;
    public Button closeButton;
    public GameObject pauseMenu;
    public Slider soundEffectsSlider;
    public static bool isPaused;

    // Use this for initialization
    void Start ()
    {
        soundEffectsSlider.value = AudioListener.volume;
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public void Pause()
    {
        isPaused = !isPaused;
        Debug.Log(isPaused);
        if (isPaused)
        {
            Time.timeScale = 0.0f;
            this.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1.0f;
            this.gameObject.SetActive(false);
        }
    }

    public static void CheckIfPaused()
    {
        if (isPaused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = soundEffectsSlider.value;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}
