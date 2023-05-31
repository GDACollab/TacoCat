using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockTimer : MonoBehaviour
{
    public TMP_Text clockTime;

    private void Start()
    {
        GameManager gameManager = GameManager.instance;
        string clockHour = gameManager.curClockHour.ToString();
        string clockMinute = gameManager.curClockMinute.ToString();

        if (gameManager.curClockHour > 12) { clockHour = (gameManager.curClockHour - 12).ToString(); }

        if (clockMinute.Length == 1)
        {
            clockMinute = "0" + clockMinute;
        }
        if (gameManager.isAM == true)
        {
            clockTime.text = clockHour + ":" + clockMinute + " AM";
        }
        else
        {
            clockTime.text = clockHour + ":" + clockMinute + " PM";
        }

    }

    void Update()
    {
        GameManager gameManager = GameManager.instance;

        string clockHour = gameManager.curClockHour.ToString();
        string clockMinute = gameManager.curClockMinute.ToString();
        if (clockMinute.Length == 1)
        {
            clockMinute = "0" + clockMinute;
        }

        if (gameManager.curClockMinute % gameManager.updateClockEvery_Minutes == 0)
        {
            if (gameManager.isAM == true)
            {
                clockTime.text = clockHour + ":" + clockMinute + " AM";
            }
            else
            {
                clockTime.text = clockHour + ":" + clockMinute + " PM";
            }
        }
    }
}
