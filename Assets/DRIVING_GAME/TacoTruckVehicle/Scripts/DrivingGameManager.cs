using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingGameManager : MonoBehaviour
{
    public Vehicle vehicle;
    public GroundGeneration groundGeneration;

    [Space(10)]
    public bool endOfGame;

    [Header("Distance")]
    public Transform beginningPoint;
    public Transform endPoint;
    public float totalDistance;
    public float vehicleDistance;
    public float percentageTraveled;


    // Start is called before the first frame update
    void Start()
    {
        vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.FreezeAll;
        // initFuelNitro(0.5f, 2);
    }

    // Update is called once per frame
    void Update()
    {
        // << INIT SCENE >>
        if (groundGeneration.generationFinished)
        {
            vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.None;
        }

        // << UPDATE DISTANCE TRACKER >>
        totalDistance = Vector2.Distance(beginningPoint.position, endPoint.position);
        vehicleDistance = Vector2.Distance(beginningPoint.position, vehicle.transform.position);
        percentageTraveled = vehicleDistance / totalDistance;

    }
    
    // Function to initialize the fuel and nitro. Called outside this class.
    public void initFuelNitro(float fuel, int nitro){
        vehicle.fuelAmount = (int)(fuel*vehicle.maxFuel);
        vehicle.nitroCharges = (nitro>3) ? 3: nitro;
    }
}
