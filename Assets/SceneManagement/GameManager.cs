using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum currScene { MENU , INTRO_CUTSCENE , TACO_MAKING , DRIVING }

public class GameManager : MonoBehaviour
{
    public int menuIndex = 0;
    public int tacoMakingIndex = 1;
    public int drivingIndex = 2;
    public int cutscene = 3;
    public int loadingSceneIndex = 4;

    TacoMakingGameManager tacoGameManager;
    DrivingGameManager drivingGameManager;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
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
            if (tacoGameManager.endOfGame)
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

            if (drivingGameManager.endOfGame)
            {
                LoadTacoMakingScene();
            }
        }
        
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuIndex);
    }

    public void LoadTacoMakingScene()
    {

        StartCoroutine(LoadingCoroutine(tacoMakingIndex));
    }

    public void LoadDrivingScene()
    {
        StartCoroutine(LoadingCoroutine(drivingIndex));
    }

    IEnumerator LoadingCoroutine(int sceneIndex)
    {
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
    }

    public void LoadCutscene()
    {
        SceneManager.LoadScene(cutscene);
    }


}
