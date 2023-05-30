using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.Escape;
    public GameObject pauseCanvas;
    public GameObject volumeSliderParent;

    [Header("Groups")]
    public GameObject baseMenu;
    public GameObject volumeSettings;
    public GameObject levelSwap;

    AudioManager audioManager;
    protected bool isPaused = false;

    protected List<Slider> volumeSliders = new List<Slider>();

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        var sliderTransform = volumeSliderParent.transform;
        for (var i = 0; i < sliderTransform.childCount; i++) {
            var child = sliderTransform.GetChild(i);
            if (child.TryGetComponent<Slider>(out var slider)) {
                volumeSliders.Add(slider);
                slider.value = 0.5f;
            }
        }

        pauseCanvas.SetActive(false);
    }

    public void Pause() {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);

        // set sliders to current values
        for (int i = 0; i < volumeSliders.Count; i++)
        {
            var slider = volumeSliders[i];
            switch (i)
            {
                case 0:
                    slider.value = audioManager.masterVolume;
                    break;
                case 1:
                    slider.value = audioManager.musicVolume;
                    break;
                case 2:
                    slider.value = audioManager.ambianceVolume;
                    break;
                case 3:
                    slider.value = audioManager.sfxVolume;
                    break;
                case 4:
                    slider.value = audioManager.dialogueVolume;
                    break;
            }
        }

        // Reset pause menu on close
        if (!isPaused)
        {
            Debug.Log("PauseManager: Resetting Pause Menu");
            baseMenu.SetActive(false);
            volumeSettings.SetActive(false);
            levelSwap.SetActive(false);

        }

        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ShowMainPauseMenu()
    {
        // Pause Game
        isPaused = true;
        pauseCanvas.SetActive(true);

        // Open settings menu
        Debug.Log("PauseManager: Opening Pause Menu");
        baseMenu.SetActive(true);
        volumeSettings.SetActive(false);
        levelSwap.SetActive(false);
    }

    // Quick access settings menu
    public void ShowVolumeSettings()
    {
        // Pause Game
        isPaused = true;
        pauseCanvas.SetActive(true);

        // Open settings menu
        Debug.Log("PauseManager: Opening Settings Menu");
        baseMenu.SetActive(false);
        volumeSettings.SetActive(true);
        levelSwap.SetActive(false);
    }

    private void Update() {

        audioManager.isPaused = isPaused;


        if (Input.GetKeyDown(pauseKey)) {
            Pause();
        }
        if (isPaused) {

            // ... Switch statements. Please change AudioManager.cs to a list or something. I hate switch statements.
            for (var i = 0; i < volumeSliders.Count; i++) {
                var slider = volumeSliders[i];
                switch (i) {
                    case 0:
                        audioManager.masterVolume = slider.value;
                        break;
                    case 1:
                        audioManager.musicVolume = slider.value;
                        break;
                    case 2:
                        audioManager.ambianceVolume = slider.value;
                        break;
                    case 3:
                        audioManager.sfxVolume = slider.value;
                        break;
                    case 4:
                        audioManager.dialogueVolume = slider.value;
                        break;
                }
            }
        }
    }
}
