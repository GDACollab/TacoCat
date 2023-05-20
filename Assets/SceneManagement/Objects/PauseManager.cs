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
    public GameObject settingsMenu;
    public GameObject settingsBackButton;
    public GameObject switchMenu;
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
            }
        }

        pauseCanvas.SetActive(false);
    }

    public void Pause() {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);
        audioManager.isPaused = isPaused;
        // Reset pause menu
        if (!isPaused)
        {
            Debug.Log("PauseManager: Resetting Pause Menu");
            baseMenu.SetActive(true);
            settingsMenu.SetActive(false);
            settingsBackButton.SetActive(true);
            switchMenu.SetActive(false);
        }
        Time.timeScale = isPaused ? 0 : 1;
    }

    // Quick access settings menu
    public void Settings()
    {
        // Pause Game
        Pause();
        // Open settings menu
        Debug.Log("PauseManager: Opening Settings Menu");
        baseMenu.SetActive(false);      // Disable normal options
        settingsMenu.SetActive(true);   // Enable Settings
        settingsBackButton.SetActive(false);    // Disable access to normal options (main menu only)
    }

    private void Update() {
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
