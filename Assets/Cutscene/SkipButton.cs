using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkipButton : MonoBehaviour, IPointerClickHandler
{
    public CutsceneManager CutsceneAsset;

    void Start()
    {
        gameObject.SetActive(false);
        if (CutsceneAsset.chosenDialogue != CutsceneAsset.GoodEndingDialogue)
        {
            Invoke("ShowAsset", 3f);
        }
    }

    void ShowAsset()
    {
        gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CutsceneAsset.endOfCutscene = true;
    }
}