using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(StageManager))]
[RequireComponent(typeof(EnvironmentGenerator))]
[RequireComponent(typeof(EnvironmentOcclusion))]
public class DrivingGameManager : MonoBehaviour
{
    GameManager gameManager;
    StageManager stageManager; // manages the generation stages
    public LightingManager lightingManager;
    public DrivingUIManager uiManager;
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
    private bool endRun = false;


    // Start is called before the first frame update
    void Awake()
    {

        stageManager = GetComponent<StageManager>();
        vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.FreezeAll;


        endOfGame = false;
        stuckTime = 0;

        StartCoroutine(Initialize());
        
    }

    public IEnumerator Initialize()
    {
        yield return new WaitUntil(() => stageManager.allStagesGenerated);

        vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.None;

    }

    // Update is called once per frame
    void Update()
    {
        // Check for stuck
        if (vehicle.GetFuel() == 0 && vehicle.GetNitro() == 0 && !endOfGame) // Out of fuel & Nitro
        {

            if (vehicle.GetVelocity().x < stuckMaxVelocity) // Truck is stuck
            {
                if (stuckTime >= stuckTimeoutDuration && !endOfGame && !endRun) // Timer is up
                {
                    Debug.Log("You ran out of gas. A tow truck took you back to the prevous city");
                    uiManager.transitionStop("You ran out of gas. A tow truck took you back to the previous city", false);
                    endRun = true;
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
            uiManager.transitionStop("You made it to the next city. One step closer to Jamie!", true);
        }

        // << UPDATE DISTANCE TRACKER >>
        vehicleDistance = Vector2.Distance(stageManager.main_begPos, vehicle.transform.position);
        totalDistance = stageManager.mainGenerationLength;
        percentageTraveled = vehicleDistance / totalDistance;
        if (percentageTraveled <= 0) { percentageTraveled = 0; }

        if (lightingManager != null)
        {
            lightingManager.timeOfDay = percentageTraveled;
        }

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
