using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(StageManager))]
public class DrivingGameManager : MonoBehaviour
{
    private StageManager stageManager; // manages the generation stages
    public Vehicle vehicle;

    [Space(10)]
    public bool endOfGame;

    [Header("Distance")]
    public float totalDistance;
    public float vehicleDistance;
    public float percentageTraveled;

    [Header("Stuck")]
    public int stuckMaxVelocity;
    public int stuckTimeoutDuration;
    public float stuckTime;


    // Start is called before the first frame update
    void Awake()
    {
        stageManager = GetComponent<StageManager>();

        vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.FreezeAll;

        totalDistance = stageManager.mainGenerationLength;

        endOfGame = false;
        stuckTime = 0;
        
    }

    // Update is called once per frame
    void Update()
    {


        //vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.None;


        // Check for stuck
        if (vehicle.GetFuel() == 0 && vehicle.GetNitro() == 0 && !endOfGame) // Out of fuel & Nitro
        {

            if (vehicle.GetVelocity().x < stuckMaxVelocity) // Truck is stuck
            {
                if (stuckTime >= stuckTimeoutDuration && !endOfGame) // Timer is up
                {
                    Debug.Log("You ran out of gas. A tow truck took you back to the prevous city");
                    GameObject.Find("GameManager").GetComponent<GameManager>().LoadTacoMakingScene();
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

            try
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().LoadTacoMakingScene();
            }
            catch { Debug.LogWarning("GameManager could not be found.", this.gameObject); }
        }

        // << UPDATE DISTANCE TRACKER >>
        /*
        vehicleDistance = Vector2.Distance(beginningPoint.position, vehicle.transform.position);
        percentageTraveled = vehicleDistance / totalDistance;
        */

    }
    
    public List<int> getSignDistances(int numLandmarks, int totalSignDistance){
        List<int> signs = new List<int>(numLandmarks);
        int signDistance = Mathf.FloorToInt(totalSignDistance/(numLandmarks+1));
        int distanceLeft = totalSignDistance;
        
        for(int i = 0; i<numLandmarks; i++){
            distanceLeft -= signDistance;
            signs.Add(distanceLeft);
        }
        return signs;
    }
}
