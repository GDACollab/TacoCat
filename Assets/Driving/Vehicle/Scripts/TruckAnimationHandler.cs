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
    public ParticleSystem nitroParty;
    public ParticleSystem flipParty;

    public GameObject nitroEffect;

    public GameObject perfectLandingEffect;

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
        // else { EnableNitroEffect(false); }

        if (vehicle.state == driveState.CRASH)
        {
            TriggerCrashEffect(2);
        }

        if (vehicle.state == driveState.PERFECT_LANDING)
        {
            EnablePerfectBoostEffect(true);
        }
        // else { EnablePerfectBoostEffect(false); }
    }

    public void TriggerCrashEffect(float destroyTimer = 2)
    {
        // if (spawnedCrashEffect != null) { return; } // don't spawn if spawned already 

        // spawnedCrashEffect = Instantiate(crashEffectPrefab, vehicle.transform.position, Quaternion.identity);

        // Destroy(spawnedCrashEffect, destroyTimer);
        ParticleSystem crashParty = vehicle.GetComponent<ParticleSystem>();
        crashParty.Play(false);
    }

    public void EnableNitroEffect(bool enabled)
    {

        // nitroEffect.SetActive(enabled);
        // If you can get the nitro particle system in velocity truck's children, then please do.
        // ParticleSystem nitroParty = vehicle.gameObject.GetComponentInChildren<ParticleSystem>();
        nitroParty.Play(false);
    }

    public void EnablePerfectBoostEffect(bool enabled)
    {
        // perfectLandingEffect.SetActive(enabled);
        // If you can get the flip particle system in velocity truck's children, then please do.
        // ParticleSystem flipParty = vehicle.gameObject.GetComponentInChildren<ParticleSystem>();
        flipParty.Play(false);
    }
}
