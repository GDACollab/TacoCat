using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentOcclusion : MonoBehaviour
{

    public Transform followTarget;
    public Vector2 activeObjectRange = new Vector2(200,100);


    public Transform targetTransform; // the center of the range
    public float range = 2.0f; // the range around the center

    public List<GameObject> gameObjectsToCheck; // the list of game objects to check

    private void Start()
    {
        foreach (GameObject gameObjectToCheck in gameObjectsToCheck)
        {
            Transform transformToCheck = gameObjectToCheck.transform;

            // calculate the distance between the target and the transform to check
            float distance = Vector3.Distance(targetTransform.position, transformToCheck.position);

            // check if the distance is within the specified range
            if (distance <= range)
            {
                Debug.Log(transformToCheck.name + " is within range of " + targetTransform.name);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(followTarget.position, activeObjectRange);
    }
}
