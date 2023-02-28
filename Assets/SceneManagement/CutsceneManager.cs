using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    GameManager gameManager;

    public void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public void LoadGame()
    {
        Debug.Log("Load Game");
        gameManager.LoadTacoMakingScene();
    }
}
