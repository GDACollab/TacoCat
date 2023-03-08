using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
	void Start()
	{
		
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Space) == true) {
			SceneManager.LoadScene("Main");
		}
	}
}
