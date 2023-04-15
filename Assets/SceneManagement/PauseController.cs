using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.P; // Set the key that should be used to pause the game
    public GameObject pauseCanvas; // Reference to the pause menu canvas
    public string excludedSceneName; // The name of the scene to exclude from pausing

    private bool isPaused = false;

    void Start()
    {
        // Disable the pause menu canvas at the start of the game
        pauseCanvas.SetActive(false);

        // Make the pause menu persistent across scenes
        DontDestroyOnLoad(pauseCanvas);

        // Make the pause menu controller persistent across scenes
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
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

        // Check if the game is paused
        if (isPaused)
        {
            // Check if the pause key is pressed again to resume the game
            if (Input.GetKeyDown(pauseKey))
            {
                return;
            }
            else
            {
                // Disable all input other than the pause key
                DisableInput();
            }
        }
    }

    private void DisableInput()
    {
        // Disable all input except for the pause key
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            // Skip the pause key
            if (keyCode == pauseKey) continue;

            // Disable the key if it is currently being pressed
            if (Input.GetKey(keyCode))
            {
                Input.ResetInputAxes();
                return;
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

        // Enable the pause menu canvas
        pauseCanvas.SetActive(true);

        Debug.Log("Game paused.");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // Set time scale back to one to resume the game
        isPaused = false;

        // Disable the pause menu canvas
        pauseCanvas.SetActive(false);

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
