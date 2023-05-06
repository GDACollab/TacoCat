using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckAnimationHandler : MonoBehaviour
{
    Vehicle vehicle;
    public CameraHandler cameraHandler;
    
    [Space(10)]
    public GameObject crashEffectPrefab;
    private GameObject spawnedCrashEffect;

    public GameObject nitroEffect;

    public GameObject perfectLandingEffect;

    public void Start()
    {
        vehicle = GetComponentInParent<Vehicle>();

    }

    public void Update()
    {
        if (vehicle.state == DRIVE_STATE.NITRO)
        {
            EnableNitroEffect(true);
        } 
        else { EnableNitroEffect(false); }

        if (vehicle.state == DRIVE_STATE.CRASH)
        {
            TriggerCrashEffect();
        }

        if (vehicle.state == DRIVE_STATE.PERFECT_LANDING)
        {
            EnablePerfectBoostEffect(true);
        }
        else { EnablePerfectBoostEffect(false); }
    }

    public void TriggerCrashEffect()
    {
        if (spawnedCrashEffect != null) { return; } // don't spawn if spawned already 

        spawnedCrashEffect = Instantiate(crashEffectPrefab, vehicle.transform.position, Quaternion.identity);

        Destroy(spawnedCrashEffect, 5);
    }

    public void EnableNitroEffect(bool enabled)
    {
        nitroEffect.SetActive(enabled);
    }

    public void EnablePerfectBoostEffect(bool enabled)
    {
        perfectLandingEffect.SetActive(enabled);
    }
}
