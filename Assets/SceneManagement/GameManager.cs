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

    // TacoMakingGameManager tacoGameManager;
    // DrivingGameManager drivingGameManager;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Update()
    {
        /*
        // << TACO GAME MANAGER >>
        if (tacoGameManager == null)
        {
            tacoGameManager = GameObject.FindGameObjectWithTag("TacoGameManager").GetComponent<TacoMakingGameManager>();
        }
        else
        {
            // check if all customers submitted , if so move to driving with gas amount
        }
        */
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuIndex);
    }

    public void LoadTacoMakingScene()
    {

        StartCoroutine(TacoMakingLoadCoroutine());
    }

    IEnumerator TacoMakingLoadCoroutine()
    {
        SceneManager.LoadSceneAsync(loadingSceneIndex);
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(tacoMakingIndex);

        yield return new WaitForSeconds(2);

        SceneManager.UnloadSceneAsync(loadingSceneIndex);
    }



    public void LoadDrivingScene()
    {
        SceneManager.LoadScene(drivingIndex);
    }

    IEnumerator DrivingLoadCoroutine()
    {
        SceneManager.LoadSceneAsync(loadingSceneIndex);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(drivingIndex);

        yield return new WaitForSeconds(2);

        SceneManager.UnloadSceneAsync(loadingSceneIndex);
    }

    public void LoadCutscene()
    {
        SceneManager.LoadScene(cutscene);
    }


}
