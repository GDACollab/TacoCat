using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckAnimationHandler : MonoBehaviour
{
    Vehicle vehicle;
    CameraHandler cameraHandler;
    
    [Space(10)]
    public GameObject crashEffectPrefab;
    private GameObject spawnedCrashEffect;

    public GameObject nitroEffect;
    public ParticleSystem nitroParticles;
    public ParticleSystem nitroParticles2;


    public GameObject perfectLandingEffect;
    public ParticleSystem perfectParticles;
    public ParticleSystem perfectParticles2;


    public void Start()
    {
        vehicle = GetComponentInParent<Vehicle>();
        cameraHandler = Camera.main.GetComponent<CameraHandler>();


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

        if (enabled)
        {
            if (nitroParticles.isStopped)
            {
                nitroParticles.Play();
            }

            if (nitroParticles2.isStopped)
            {
                nitroParticles2.Play();
            }
        }
        else
        {
            if (nitroParticles.isPlaying)
            {
                nitroParticles.Stop();
            }

            if (nitroParticles2.isPlaying)
            {
                nitroParticles2.Stop();
            }
        }
    }

    public void EnablePerfectBoostEffect(bool enabled)
    {
        perfectLandingEffect.SetActive(enabled);

        if (enabled)
        {
            if (perfectParticles.isStopped)
            {
                perfectParticles.Play();
            }

            if (perfectParticles2.isStopped)
            {
                perfectParticles2.Play();
            }
        }
        else
        {
            if (perfectParticles.isPlaying)
            {
                perfectParticles.Stop();
            }

            if (perfectParticles2.isPlaying)
            {
                perfectParticles2.Stop();
            }
        }
    }
}
