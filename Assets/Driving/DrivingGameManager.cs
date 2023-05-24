using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class DrivingGameManager : MonoBehaviour
{
    [HideInInspector]
    public GameManager gameManager;

    public StageManager foregroundStageManager;
    public StageManager playAreaStageManager; // manages the generation stages
    public StageManager backgroundStageManager; // manages the generation stages


    public LightingManager lightingManager;
    public DrivingUIManager uiManager;
    public Vehicle vehicle;
    public CameraHandler camHandler;


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
    
    [Header("Transition")]
    public string successText = "You made it to the next city. One step closer to Jamie!";
    public string failText = "You ran out of gas. A tow truck took you back to the prevous city";
    
    [Header("Nitro Carry")]
    public int nitroCharges = 3;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.FreezeAll;

        endOfGame = false;
        stuckTime = 0;

        StartCoroutine(Initialize());
        
    }

    public IEnumerator Initialize()
    {
        playAreaStageManager.BeginStageGeneration();
        yield return new WaitUntil(() => playAreaStageManager.allStagesGenerated);
        yield return new WaitUntil(() => playAreaStageManager.environmentGenerator.environmentSpawned);

        // move vehicle
        vehicle.transform.position = playAreaStageManager.environmentGenerator.playerSpawnPoint.position;
        camHandler.transform.position = playAreaStageManager.environmentGenerator.playerSpawnPoint.position;


        foregroundStageManager.BeginStageGeneration();
        yield return new WaitUntil(() => foregroundStageManager.allStagesGenerated);

        backgroundStageManager.BeginStageGeneration();
        yield return new WaitUntil(() => backgroundStageManager.allStagesGenerated);

        //playAreaStageManager.environmentGenerator.
        vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.None;
        vehicle.nitroCharges = nitroCharges;
        uiManager.updateNitro();
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
                    Debug.Log(failText);
                    uiManager.transitionStop(failText, false);
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
            Debug.Log(successText);
            uiManager.transitionStop(successText, true);
        }

        // << UPDATE DISTANCE TRACKER >>
        vehicleDistance = Vector2.Distance(playAreaStageManager.main_begPos, vehicle.transform.position);
        totalDistance = playAreaStageManager.mainGenerationLength;
        percentageTraveled = vehicleDistance / totalDistance;
        if (percentageTraveled <= 0) { percentageTraveled = 0; }

        lightingManager.timeOfDay = gameManager.main_gameTimer;
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
