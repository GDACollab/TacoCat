using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    public DrivingGameManager drivingGameManager;

    [Space(10)]
    public bool start;
    public bool end;

    public void Start()
    {
        drivingGameManager = GameObject.FindGameObjectWithTag("DrivingGameManager").GetComponent<DrivingGameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Vehicle"))
        {
            if (end)
            {
                Debug.Log("Player hit end driving trigger");


                //drivingGameManager.endOfGame = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Vehicle"))
        {
            if (start)
            {
                drivingGameManager.camHandler.overrideCam = false;
            }
        }
    }

}
