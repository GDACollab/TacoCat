using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    // Set the key that should be used to pause the game
    public KeyCode pauseKey = KeyCode.P;

    // Reference to the pause menu canvas
    public GameObject pauseCanvas;

    // The name of the scene to exclude from pausing
    public string excludedSceneName;

    private bool isPaused = false;

    void Start()
    {
        // Disable the pause menu canvas at the start of the game
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);

            // Make the pause menu canvas persistent across scenes
            DontDestroyOnLoad(pauseCanvas);
        }

        // Make the pause menu controller persistent across scenes
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Pause or resume the game when the pause key is pressed
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // Disable all input except for the pause key when the game is paused
        if (isPaused)
        {
            DisableInput();
        }
    }

    // Disable all input except for the pause key
    private void DisableInput()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            switch (keyCode)
            {
                // Skip the pause key
                case KeyCode.P:
                    continue;

                // Disable the key if it is currently being pressed
                default:
                    if (Input.GetKey(keyCode))
                    {
                        Input.ResetInputAxes();
                    }
                    break;
            }
        }
    }

    private void PauseGame()
    {
        if (IsExcludedScene())
        {
            return;
        }

        Time.timeScale = 0f; // Set time scale to zero to pause the game
        isPaused = true;

        if (pauseCanvas != null)
        {
            // Enable the pause menu canvas
            pauseCanvas.SetActive(true);
        }

        Debug.Log("Game paused.");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // Set time scale back to one to resume the game
        isPaused = false;

        if (pauseCanvas != null)
        {
            // Disable the pause menu canvas
            pauseCanvas.SetActive(false);
        }

        Debug.Log("Game resumed.");
    }

    private bool IsExcludedScene()
    {
        // Get the name of the current scene
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if the current scene name matches the excluded scene name
        return currentSceneName == excludedSceneName;
    }
}
