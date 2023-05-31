using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    // UI Elements
    [Header("Game States")]
    public GameManager gameManager;
    public PauseManager pauseManager;
    int level;
    bool fullGameComplete;
    [Header("Signs")]
    public GameObject newGameSign;
    public GameObject continueSign;
    public GameObject hangingSign;
    public GameObject pole;

    [Header("Secret")]
    public string citiesVisitedPreface = "City Highscore: ";
    public TextMeshProUGUI citiesHighscoreText;

    void Start()
    {
        // Get GameManager
        gameManager = GameManager.instance;
        pauseManager = gameManager.pauseManager;

        level = gameManager.currLevel;
        fullGameComplete = gameManager.fullGameComplete;

        if ((level > 1 && level < 4))
        {
            continueSign.SetActive(true); // Enable continue sign
            newGameSign.SetActive(false);
            // Adjust pole to fit continue sign
            RectTransform poleBox = pole.GetComponent<RectTransform>();
            poleBox.sizeDelta = new Vector2(100, 1500);
        }
        else
        {
            newGameSign.SetActive(true);
        }

        if (fullGameComplete)
        {
            citiesHighscoreText.text = citiesVisitedPreface + gameManager.endlessCitiesHighscore;

            hangingSign.SetActive(true); // Enable hanging sign
            // Redundant; On active sign already create deploy animation
            Animator signDrop = hangingSign.GetComponent<Animator>();
            Debug.Log("MenuManager: Deloying Hanging Sign");
            signDrop.Play("Deploy"); //<<SECRET>>
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
        gameManager.endlessModeActive = false;

        gameManager.currLevel = 1;
        gameManager.LoadCutscene();
        gameManager.GameTimerStart();
        // TBD: First cutscene audio
    }

    public void Settings()
    {
        pauseManager = GameManager.instance.pauseManager;
        pauseManager.ShowVolumeSettings(true);
    }

    public void Credits()
    {
        gameManager.LoadCredits();
    }

    public void Exit()
    {
        gameManager.Quit();
    }

    public void StartEndlessMode()
    {
        gameManager.LoadEndlessTacoMaking(true);
    }
}
