using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartSceneOnEntry : MonoBehaviour
{
    public KeyCode manualRestartKey = KeyCode.R;

    public void Update()
    {
        if (Input.GetKeyDown(manualRestartKey))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(0);
    }
}
