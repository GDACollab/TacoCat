using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // UI Elements
    [Header("Signs")]
    public GameObject continueSign;
    public GameObject hangingSign;
    public GameObject pole;

    void Start()
    {
        // Check level progression
        int level = GameManager.instance.currLevel;
        bool win = GameManager.instance.trueEnding;

        if (level > 1 && level < 4)
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
}
