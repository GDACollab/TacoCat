using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameManager gameManager;
    public AudioManager audioManager;
    void Start()
    {
        gameManager = GameManager.instance;
        audioManager = gameManager.audioManager;
    }

    // Update is called once per frame
    void Update()
    {

        if (gameManager == null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        }

    }


    public void StartGame()
    {
        gameManager.LoadCutscene();
    }
}
