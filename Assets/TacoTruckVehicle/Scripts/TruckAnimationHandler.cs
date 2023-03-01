using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckAnimationHandler : MonoBehaviour
{
    public Vehicle vehicle;

    public GameObject crashEffectPrefab;
    private GameObject spawnedCrashEffect;

    public GameObject nitroEffect;

    public void Start()
    {
        vehicle = GetComponentInParent<Vehicle>();

    }

    public void Update()
    {
        if (vehicle.state == driveState.NITRO)
        {
            EnableNitroEffect(true);
        } 
        else { EnableNitroEffect(false); }

        if (vehicle.state == driveState.CRASH)
        {
            TriggerCrashEffect(2);
        }
    }

    public void TriggerCrashEffect(float destroyTimer)
    {
        if (spawnedCrashEffect != null) { return; } // don't spawn if spawned already 

        spawnedCrashEffect = Instantiate(crashEffectPrefab, vehicle.transform.position, Quaternion.identity);

        Destroy(spawnedCrashEffect, destroyTimer);
    }

    public void EnableNitroEffect(bool enabled)
    {
        nitroEffect.SetActive(enabled);
    }
}
