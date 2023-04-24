using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    public DrivingGameManager drivingGameManager;

    public void Start()
    {
        drivingGameManager = GetComponentInParent<DrivingGameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        drivingGameManager.endOfGame = true;
    }
}
