using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum currScene { MENU , INTRO_CUTSCENE , TACO_MAKING , DRIVING }

public class GameManager : MonoBehaviour
{

    public bool isLoadingScene;

    public int menuIndex = 0;
    public int tacoMakingIndex = 1;
    public int drivingIndex = 2;
    public int cutsceneIndex = 3;
    public int loadingSceneIndex = 4;
    public int randDriveIndex;
    public int nightDriveIndex;

    TacoMakingGameManager tacoGameManager;
    DrivingGameManager drivingGameManager;
    CutsceneManager cutsceneManager;
    public AudioManager audioManager;

    void Start() {
        audioManager.PlaySong(menuIndex);
    }

    public void Update()
    {
        
        // << TACO GAME MANAGER >>
        if (tacoGameManager == null)
        {
            try
            {
                tacoGameManager = GameObject.FindGameObjectWithTag("TacoGameManager").GetComponent<TacoMakingGameManager>();
            }
            catch{ }
        }
        else
        {
            // check if all customers submitted , if so move to driving with gas amount
            if (tacoGameManager.endOfGame && !isLoadingScene)
            {
                LoadDrivingScene();
            }
        }


        // << DRIVING GAME MANAGER >>
        if (drivingGameManager == null)
        {
            try
            {
                drivingGameManager = GameObject.FindGameObjectWithTag("DrivingGameManager").GetComponent<DrivingGameManager>();
            }
            catch { }
        }
        else {

            if (drivingGameManager.endOfGame && !isLoadingScene)
            {
                LoadMenu();
            }
        }

        // << CUTSCENE MANAGER >>
        if (cutsceneManager == null)
        {
            try
            {
                cutsceneManager = GameObject.FindGameObjectWithTag("CutsceneManager").GetComponent<CutsceneManager>();
            }
            catch { }
        }
        else { 

            if (cutsceneManager.endOfCutscene && !isLoadingScene)
            {
                LoadTacoMakingScene();
            }
        }
        // << AUDIO MANAGER >>
        /*
        if (audioManager == null)
        {
            try
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
            }
            catch { }
        }*/
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuIndex);
        audioManager.PlaySong(menuIndex);
    }

    public void LoadTacoMakingScene()
    {

        StartCoroutine(LoadingCoroutine(tacoMakingIndex));
        audioManager.PlaySong(tacoMakingIndex);

    }

    public void LoadDrivingScene()
    {
        StartCoroutine(LoadingCoroutine(drivingIndex));
        audioManager.PlaySong(drivingIndex);
    }

    public void LoadRandDrivingScene()
    {
        StartCoroutine(LoadingCoroutine(randDriveIndex));
        audioManager.PlaySong(drivingIndex);
    }

    public void LoadNightDrivingScene()
    {
        StartCoroutine(LoadingCoroutine(nightDriveIndex));
        audioManager.PlaySong(drivingIndex);
    }

    IEnumerator LoadingCoroutine(int sceneIndex)
    {
        isLoadingScene = true;
        yield return null;

        SceneManager.LoadSceneAsync(loadingSceneIndex); // load loading scene

        AsyncOperation newScene = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive); // async load next scene

        //Don't let the Scene activate until you allow it to
        newScene.allowSceneActivation = false;
        Debug.Log("Pro :" + newScene.progress);

        //When the load is still in progress, output the Text and progress bar
        while (!newScene.isDone)
        {
            //Output the current progress
            Debug.Log("Loading progress: " + (newScene.progress * 100) + "%");

            // Check if the load has finished
            if (newScene.progress >= 0.9f)
            {
                newScene.allowSceneActivation = true;
            }

            yield return null;
        }

        SceneManager.UnloadSceneAsync(loadingSceneIndex);
        isLoadingScene = false;
    }

    public void LoadCutscene()
    {
        SceneManager.LoadScene(cutsceneIndex);
        audioManager.PlaySong(cutsceneIndex);
    }

}
