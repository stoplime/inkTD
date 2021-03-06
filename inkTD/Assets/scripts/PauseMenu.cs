﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using helper;

public class PauseMenu : MonoBehaviour
{
    public Button pauseButton;
    public Button closeButton;
    public GameObject pauseMenu;
    public Slider musicSlider;
    public Slider soundEffectsSlider;
    public Toggle fullScreenToggle;
    public Dropdown resolutionsDropdown;
    public GameObject musicPlayer;
    public static bool isPaused;
    public static bool isFullScreen;
    public Resolution[] resolutions;
    public int selectedResoultion;

    void Awake()
    {
        
        musicPlayer.GetComponent<AudioSource>().volume = Help.MusicVolume;
    }

    // Use this for initialization
    void Start ()
    {
        if (musicSlider != null)
        {
            if (musicPlayer != null)
            {
                musicSlider.value = Help.MusicVolume;
            }
            else
            {
                musicSlider.enabled = false;
            }
        }
        if(soundEffectsSlider != null)
        {
            soundEffectsSlider.value = Help.TowerSoundEffectVolume;
        }
        isFullScreen = Screen.fullScreen;
        if (fullScreenToggle != null)
        {
            fullScreenToggle.isOn = isFullScreen;
        }
        resolutions = Screen.resolutions;
        // print("Run the start");
        if (resolutionsDropdown != null)
        {
            resolutionsDropdown.ClearOptions();
            List<string> options = new List<string>();
            for (int i = 0; i < resolutions.Length; i++)
            {
                options.Add(resolutions[i].width.ToString() + "x" + resolutions[i].height.ToString());
                if (resolutions[i].Equals(Screen.currentResolution))
                {
                    selectedResoultion = i;
                }
            }
            resolutionsDropdown.AddOptions(options);
            resolutionsDropdown.value = selectedResoultion;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public void Pause()
    {
        isPaused = !isPaused;
        // Debug.Log(isPaused);
        if (isPaused)
        {
            Time.timeScale = 0.0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1.0f;
            pauseMenu.SetActive(false);
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

    public void ChangeMusicVolume()
    {
        musicPlayer.GetComponent<AudioSource>().volume = musicSlider.value;
        Help.MusicVolume = musicSlider.value;
    }

    public void ChangeSoundEffectVolume()
    {
        Help.TowerSoundEffectVolume = soundEffectsSlider.value;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    public void OnChangeResolution()
    {
        if (resolutions == null)
        {
            Start();
        }
        if (resolutionsDropdown != null)
        {
            selectedResoultion = resolutionsDropdown.value;
        }
        Screen.SetResolution(resolutions[selectedResoultion].width, resolutions[selectedResoultion].height, isFullScreen);
        // print("Resolution Change");
    }

    public void OnAntiAlias(bool value)
    {
        if (value)
        {
            QualitySettings.antiAliasing = 2;
        }
        else
        {
            QualitySettings.antiAliasing = 0;
        }
    }
}
