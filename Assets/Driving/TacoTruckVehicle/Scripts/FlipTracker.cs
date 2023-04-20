using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTracker : MonoBehaviour
{

    Vehicle vehicle;
    TruckAnimationHandler animHandler;
    RaycastHit2D hit;
    public AudioManager audioManager;
    GroundGeneration groundGeneration;
    int hitPointIndex;
    float initTruckRotation;

    public bool jumpStarted = false;

    [Space(10)]
    public int flipCount;
    private bool flipCounted;
    public float currAirTime;

    [Space(10)]
    public float perfectLandingRotationBound = 1;
    public float perfectLandingMinAirTime = 0.75f;

    [Space(10)]
    public float currRot;
    public float startJumpRot;
    public float endJumpRot;

    [Space(10)]
    public float landPointRotation;


    void Start()
    {
        vehicle = GetComponent<Vehicle>();
        animHandler = GetComponent<TruckAnimationHandler>();
        initTruckRotation = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        currRot = transform.rotation.eulerAngles.z - initTruckRotation;

        // get point underneath truck
        hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, vehicle.groundLayer);
        if (hit.collider == null) { return; }

        // get point underneath truck
        groundGeneration = hit.collider.gameObject.GetComponentInParent<GroundGeneration>();
        hitPointIndex = groundGeneration.GetClosestGroundPointIndexToPos(hit.point);
        if (groundGeneration == null) { return; }

        // << TRIGGER WHEN IN AIR >>
        if (vehicle.state == driveState.IN_AIR && !jumpStarted)
        {
            // reset
            startJumpRot = 0;
            endJumpRot = 0;

            // set values
            jumpStarted = true;
            startJumpRot = currRot;
        }

        // << TRIGGER WHEN GROUNDED >>
        if (vehicle.state == driveState.GROUNDED && jumpStarted)
        {
            // set values
            jumpStarted = false;
            endJumpRot = currRot;

            landPointRotation = groundGeneration.allGroundRotations[hitPointIndex];

            if (IsPerfectLanding(endJumpRot, landPointRotation) && flipCount > 0) 
            {
                StartCoroutine(vehicle.PerfectLandingBoost());
                audioManager.Play(audioManager.flipBoostSFX);
            }
        }

        // track in air time
        if (jumpStarted && vehicle.state == driveState.IN_AIR)
        {
            currAirTime += Time.deltaTime;

            ActiveFlipCounter();
        }
        else 
        { 
            currAirTime = 0;
            flipCount = 0;
        }
    }

    public bool IsPerfectLanding(float landPointRot, float groundPointRot)
    {
        if (vehicle.state == driveState.CRASH) { return false; }

        // if rotation is within bound and enough time has passed and landing downhill
        if ((groundPointRot - landPointRot) < perfectLandingRotationBound && currAirTime > perfectLandingMinAirTime && landPointRot < -5) 
        { 
            return true;
        }
        return false;
    }

    public void ActiveFlipCounter()
    {
        if ((currRot < -250) && (currRot > -270) && !flipCounted)
        {
            flipCount++;
            flipCounted = true;
        }
        else if ((currRot < 10) && (currRot > -10))
        {
            flipCounted = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundGeneration != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundGeneration.allGroundPoints[hitPointIndex], 4);
        }
    }
}
