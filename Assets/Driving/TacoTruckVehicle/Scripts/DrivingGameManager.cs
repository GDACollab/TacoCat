using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    [Header("Stuck")]
    public int stuckMaxVelocity;
    public int stuckTimeoutDuration;
    public float stuckTime;


    // Start is called before the first frame update
    void Start()
    {
        vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.FreezeAll;
        endOfGame = false;
        stuckTime = 0;
        
    }

    // Update is called once per frame
    void Update()
    {

        // << INIT SCENE >>
        if (groundGeneration.generationFinished)
        {
            vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.None;
        }

        // Check for stuck
        if (vehicle.GetFuel() == 0 && vehicle.GetNitro() == 0 && !endOfGame) // Out of fuel & Nitro
        {

            if (vehicle.GetVelocity().x < stuckMaxVelocity) // Truck is stuck
            {
                if (stuckTime >= stuckTimeoutDuration && !endOfGame) // Timer is up
                {
                    Debug.Log("You ran out of gas. A tow truck took you back to the prevous city");
                    endOfGame = true;
                }
                else 
                {
                    stuckTime += Time.deltaTime; // Increment the timer
                }
            }
            else // Truck is still moving
            {
                stuckTime = 0; // Reset the clock
            }
        }
        
        // Check for end of level
        if (percentageTraveled >= 1 && !endOfGame)
        {
            Debug.Log("You made it to the next city. One step closer to Jamie!");
            endOfGame = true;
        }

        // << UPDATE DISTANCE TRACKER >>
        totalDistance = Vector2.Distance(beginningPoint.position, endPoint.position);
        vehicleDistance = Vector2.Distance(beginningPoint.position, vehicle.transform.position);
        percentageTraveled = vehicleDistance / totalDistance;

    }
}
