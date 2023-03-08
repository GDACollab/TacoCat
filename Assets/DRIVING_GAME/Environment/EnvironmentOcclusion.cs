using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentOcclusion : MonoBehaviour
{
    public List<EnvironmentGenerator> envGenerators;

    public Transform targetTransform; // the center of the range
    public float range = 2.0f; // the range around the center

    private void Update()
    {

        foreach (EnvironmentGenerator envGenerator in envGenerators)
        {
            foreach (GameObject envObject in envGenerator.allSpawnedObjects)
            {
                Transform transformToCheck = envObject.transform;

                // calculate the distance between the target and the transform to check
                float distance = Vector3.Distance(targetTransform.position, transformToCheck.position);

                // check if the distance is within the specified range
                if (distance <= range)
                {
                    envObject.SetActive(true);
                    // Debug.Log(transformToCheck.name + " is within range of " + targetTransform.name);
                }
                else
                {
                    envObject.SetActive(false);
                }
            }
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(targetTransform.position, range);
    }
}
