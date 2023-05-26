using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PopUp_Card : MonoBehaviour
{

    [Header("Header")]
    public TextMeshProUGUI headerTMP;

    [Header("Body")]
    public TextMeshProUGUI bodyTMP;

    public void SetHeader(string text)
    {
        headerTMP.text = text;
    }

    public void SetBody(string text)
    {
        bodyTMP.text = text;
    }
}
