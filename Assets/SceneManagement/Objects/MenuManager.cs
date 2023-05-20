using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // UI Elements
    [Header("Game States")]
    GameManager gameManager;
    PauseManager pauseManager;
    int level;
    bool win;
    [Header("Signs")]
    public GameObject continueSign;
    public GameObject hangingSign;
    public GameObject pole;

    void Start()
    {
        // Get GameManager
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        pauseManager = GameObject.FindGameObjectWithTag("PauseManager").GetComponent<PauseManager>();
        level = gameManager.currLevel;
        win = gameManager.trueEnding;

        if ((level > 1 && level < 4) || (gameManager.lastGame == currGame.DRIVING))
        {
            continueSign.SetActive(true); // Enable continue sign
            // Adjust pole to fit continue sign
            RectTransform poleBox = pole.GetComponent<RectTransform>();
            poleBox.sizeDelta = new Vector2(100, 1500);
        }
        if (win)
        {
            hangingSign.SetActive(true); // Enable hanging sign
        }
    }

    public void Continue()
    {
        // Retrieve last scene
        if (gameManager.lastGame == currGame.TACO_MAKING)
        {
            gameManager.LoadTacoMakingScene();
        }
        else if (gameManager.lastGame == currGame.DRIVING)
        {
            gameManager.LoadDrivingScene(level);
        }
        else
        {
            Debug.Log("MenuManager ERROR: Expected TACO_MAKING or DRIVING state. Got: " + gameManager.lastGame);
            gameManager.LoadTacoMakingScene(); // Default to taco making scene
        }
    }

    public void NewGame()
    {
        gameManager.LoadCutscene();
        // TBD: First cutscene audio
    }

    public void Settings()
    {
        pauseManager.Settings();
    }

    public void Credits()
    {

    }

    public void Exit()
    {
        gameManager.Quit();
    }
}
