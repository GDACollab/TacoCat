using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockTimer : MonoBehaviour
{
    public TMP_Text clockTime;
    void Update()
    {
        string clockHour = GameManager.instance.curClockHour.ToString();
        string clockMinute = GameManager.instance.curClockMinute.ToString();
        if (clockMinute.Length == 1)
        {
            clockMinute = "0" + clockMinute;
        }
        if (GameManager.instance.curClockMinute % GameManager.instance.updateClockEvery_Minutes == 0)
        {
            if (GameManager.instance.isAM == true)
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
