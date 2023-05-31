using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum DRIVINGGAME_STATE { LOADING, TUTORIAL, PLAY, COMPLETE, FAIL, END_TRANSITION }

public class DrivingGameManager : MonoBehaviour
{
    [HideInInspector]
    public GameManager gameManager;
    public LightingManager lightingManager;
    public DrivingUIManager uiManager;
    public Vehicle vehicle;
    public CameraHandler camHandler;

    [Header("States")]
    public DRIVINGGAME_STATE state = DRIVINGGAME_STATE.LOADING;
    public bool completedLevel;

    [Header("Stages")]
    public StageManager foregroundStageManager;
    public StageManager playAreaStageManager; // manages the generation stages
    public StageManager backgroundStageManager; // manages the generation stages

    [Header("Distance")]
    public float totalDistance;
    public float vehicleDistance;
    public float percentageTraveled;

    [Header("Stuck")]
    public int stuckMaxVelocity;
    public int stuckTimeoutDuration;
    public float stuckTime;

    [Header("Nitro Carry")]
    public int nitroCharges = 3;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameManager.instance;
        gameManager.drivingGameManager = GetComponent<DrivingGameManager>();
        gameManager.currGame = currGame.DRIVING;

        uiManager = GetComponentInChildren<DrivingUIManager>();
        vehicle.rb_vehicle.constraints = RigidbodyConstraints2D.FreezeAll;
        vehicle.disableInputs = true;

        // << UPDATE LIGHTING MANAGER >>
        lightingManager.timeOfDay = gameManager.main_gameTimer;

        stuckTime = 0;

        StartCoroutine(Initialize());
        
    }

    public IEnumerator Initialize()
    {
        playAreaStageManager.BeginStageGeneration();
        yield return new WaitUntil(() => playAreaStageManager.allStagesGenerated);
        yield return new WaitUntil(() => playAreaStageManager.environmentGenerator.environmentSpawned);

        // init vehicle pos     
        yield return new WaitUntil(() => playAreaStageManager.environmentGenerator.playerSpawnPoint != null);
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


        yield return new WaitForSeconds(1);
        state = DRIVINGGAME_STATE.TUTORIAL;
    }

    Coroutine endStateRoutine; // Variable to store the coroutine
    void Update()
    {

        switch (state)
        {
            case DRIVINGGAME_STATE.LOADING:

                vehicle.disableInputs = true;

                break;
            case DRIVINGGAME_STATE.TUTORIAL:

                uiManager.cameraEffectManager.StartFadeIn(1.5f);

                if (!uiManager.cameraEffectManager.isFading)
                {
                    state = DRIVINGGAME_STATE.PLAY;
                    /*uiManager.ShowBegLevelCanvas();
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        uiManager.beginningCanvas.SetActive(false);
                        state = DRIVINGGAME_STATE.PLAY;
                    }*/
                }

                break;
            case DRIVINGGAME_STATE.PLAY:
                vehicle.disableInputs = false;

                UpdatePlay();

                break;

            case DRIVINGGAME_STATE.COMPLETE:
                if (endStateRoutine == null)
                {
                    endStateRoutine = StartCoroutine(EndStateRoutine(true));
                }
                break;
            case DRIVINGGAME_STATE.FAIL:
                if (endStateRoutine == null)
                {
                    endStateRoutine = StartCoroutine(EndStateRoutine(false));
                }
                break;

            case DRIVINGGAME_STATE.END_TRANSITION:
                break;

            default:
                break;

        }
    }

    public void UpdatePlay()
    {
        // << STUCK CHECK >>
        if (vehicle.GetFuel() == 0 && vehicle.GetNitro() == 0) // Out of fuel & Nitro
        {

            state = DRIVINGGAME_STATE.FAIL;


            /*
            if (vehicle.GetVelocity().x < stuckMaxVelocity) // Truck is stuck
            {
                if (stuckTime >= stuckTimeoutDuration) // Timer is up
                {
                    state = DRIVINGGAME_STATE.FAIL;
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
            */
        }

        // << UPDATE DISTANCE TRACKER >>
        vehicleDistance = Vector2.Distance(playAreaStageManager.main_begPos, vehicle.transform.position);
        totalDistance = playAreaStageManager.mainGenerationLength;
        percentageTraveled = vehicleDistance / totalDistance;
        if (percentageTraveled <= 0) { percentageTraveled = 0; }

        // << UPDATE LIGHTING MANAGER >>
        lightingManager.timeOfDay = gameManager.main_gameTimer;
    }

    IEnumerator EndStateRoutine(bool level_complete)
    {
        StartCoroutine(vehicle.NegateVelocity(2));
        vehicle.disableInputs = true;

        uiManager.ShowEndLevelCanvas(level_complete);

        completedLevel = level_complete; // for gamemanager reference


        yield return new WaitForSeconds(1f);

        while (state != DRIVINGGAME_STATE.END_TRANSITION)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                uiManager.beginningCanvas.SetActive(false);
                uiManager.cameraEffectManager.StartFadeOut();
                state = DRIVINGGAME_STATE.END_TRANSITION;
            }
            yield return null;
        }

        endStateRoutine = null;
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
