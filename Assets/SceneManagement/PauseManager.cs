using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.Escape;
    public GameObject pauseCanvas;
    AudioManager audioManager;
    protected bool isPaused = false;

    protected List<Slider> volumeSliders = new List<Slider>();

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        var sliderTransform = pauseCanvas.transform.GetChild(0).GetChild(1);
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

        Time.timeScale = isPaused ? 0 : 1;
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
