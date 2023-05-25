using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkipButton : MonoBehaviour
{
    public GameObject skipText;

    void Start()
    {
        skipText.SetActive(false);

        CutsceneManager manager = GameObject.FindGameObjectWithTag("CutsceneManager").GetComponent<CutsceneManager>();

        if (manager.chosenDialogue != manager.GoodEndingDialogue && manager.chosenDialogue != manager.BadEndingDialogue)
        {
            Invoke("ShowAsset", 3f);
        }   
    }

    void ShowAsset()
    {
        skipText.SetActive(true);
    }

}