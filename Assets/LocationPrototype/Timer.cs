using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
	float time;
	public TMP_Text timertext;
	
	void Start()
	{
		time = 58f;
	}

	void Update()
	{
		time = time - Time.deltaTime;
		timertext.text = time.ToString("0.00");
		if (time <= 0f) {
			SceneManager.LoadScene("End");
		}
	}
}