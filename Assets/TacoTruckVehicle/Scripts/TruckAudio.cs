using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class TruckAudio : MonoBehaviour
{

    public float minRPM = 0;
    public float maxRPM = 5000;
    Vehicle vehicle;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        vehicle = GetComponentInParent<Vehicle>();
    }
    // Update is called once per frame
    void Update()
    {
        float truckSpeed = vehicle != null ? vehicle.GetVelocity().x : 0;
        truckSpeed = truckSpeed / 1000 * 2;
        // set RPM value for the FMOD event
        float effectiveRPM = Mathf.Lerp(minRPM, maxRPM, truckSpeed);
        var emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.SetParameter("RPM", effectiveRPM);
        Debug.Log("RPM: "+effectiveRPM);
        Debug.Log("truckSpeed: " + truckSpeed);
    }
}
