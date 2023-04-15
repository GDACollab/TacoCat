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
        pauseCanvas.SetActive(false);

        // Make the pause menu canvas persistent across scenes
        DontDestroyOnLoad(pauseCanvas);

        // Make the pause menu controller persistent across scenes
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Check if the game is paused
        if (isPaused)
        {
            // Check if the P key is pressed to resume the game
            if (Input.GetKeyDown(pauseKey))
            {
                ResumeGame();
            }
            else
            {
                // Disable all other input when the game is paused
                Input.ResetInputAxes();
            }
        }
        else
        {
            // Pause or resume the game when the pause key is pressed
            if (Input.GetKeyDown(pauseKey))
            {
                PauseGame();
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
