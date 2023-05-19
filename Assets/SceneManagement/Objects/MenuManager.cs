using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // UI Elements
    [Header("Game")]
    //public GameObject gameManager;
    [Header("Signs")]
    public GameObject continueSign;
    public GameObject endlessSign;
    public GameObject pole;

    void Awake()
    {
        // Check level progression
        int level = GameManager.instance.currLevel;
        if (level > 1)
        {
            continueSign.SetActive(true); // Enable continue sign
            // Adjust pole to fit continue sign
        }
        if (level > 3)
        {
            endlessSign.SetActive(true); // Enable endless sign
        }
    }
}
