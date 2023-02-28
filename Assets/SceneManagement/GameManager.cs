using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int menuIndex = 0;
    public int tacoMakingIndex = 1;
    public int drivingIndex = 2;
    public int cutscene = 3;

    TacoMakingGameManager tacoGameManager;
    // DrivingGameManager drivingGameManager;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Update()
    {
        // << TACO GAME MANAGER >>
        if (tacoGameManager == null)
        {
            tacoGameManager = GameObject.FindGameObjectWithTag("TacoGameManager").GetComponent<TacoMakingGameManager>();
        }
        else
        {
            // check if all customers submitted , if so move to driving with gas amount
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuIndex);
    }

    public void LoadTacoMakingScene()
    {
        SceneManager.LoadScene(tacoMakingIndex);
    }

    public void LoadDrivingScene()
    {
        SceneManager.LoadScene(drivingIndex);
    }

    public void LoadCutscene()
    {
        SceneManager.LoadScene(cutscene);
    }


}
