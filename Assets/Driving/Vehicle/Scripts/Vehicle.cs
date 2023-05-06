using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DRIVE_STATE { NONE, GROUNDED, UPHILL_GROUNDED, IN_AIR, NITRO, PERFECT_LANDING, CRASH, END_DRIVE }

public class Vehicle : MonoBehaviour
{
    GameManager gameManager;
    AudioManager audioManager;
    StageManager stageManager;
    public CameraHandler cameraHandler;
    DrivingGameManager drivingGameManager;
    DrivingUIManager drivingUIManager;
    public Rigidbody2D rb_vehicle;


    [Space(10)]
    public LayerMask groundLayer;
    public List<Collider2D> groundColliderList;
    public float groundColliderSize = 15;
    public float groundColliderHeightOffset = -2;

    [Header("States")]
    public DRIVE_STATE state;
    public bool gasPressed; // increase gravity force on truck
    public int rotationDir;

    [Header("General Driving")]
    public float gravity;

    [Space(10)]
    public int fuelAmount;
    public int maxFuel;
    public Vector2 startingVelocity; // initial velocity
    public Vector2 inAirForce; // input based force on truck
    public Vector2 groundedForce; // input based force on truck

    [Space(10)]
    public float uphillActivationAngle = 45;
    public float currGroundSlopeAngle;
    public Vector2 uphillForce;


    [Space(20)]
    public float velocityClamp = 500;

    [Space(10)]
    public float rotationSpeed = 50f;

    [Header("Nitro")]
    public static int nitroCharges = 3; // Note: Static variables do not show up in inspector
    public Vector2 nitroForce;
    public float activeNitroTime = 5; // how long each charge lasts

    [Header("Perfect Boost")]
    public Vector2 perfectLandingBoostForce;
    public float activePerfectBoostTime = 0.5f;

    [Header("Inputs")]
    public KeyCode gasInputKey;
    public KeyCode nitroInputKey;
    public KeyCode rotateRight;
    public KeyCode rotateLeft;

    [Header("Debug Settings")]
    [Range(0.1f, 10)]
    public float gizmoSize = 1;

    [Header("RPM (AUDIO)")]
    public float minRPM = 0;
    public float maxRPM = 2000;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioManager = gameManager.audioManager;
        stageManager = GetComponentInParent<StageManager>();
        cameraHandler = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraHandler>();
        drivingGameManager = GameObject.FindGameObjectWithTag("DrivingGameManager").GetComponent<DrivingGameManager>();
        drivingUIManager = drivingGameManager.uiManager;

        rb_vehicle.velocity = startingVelocity;
        state = DRIVE_STATE.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        StateMachine();
    }

    void FixedUpdate()
    {


        rb_vehicle.AddForce(Vector2.down * gravity * rb_vehicle.mass * Time.deltaTime);

        // << CHECK FOR GROUND COLLIDERS >>
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, groundColliderHeightOffset), groundColliderSize, groundLayer);
        groundColliderList = new List<Collider2D>(groundColliders);

        // << GAS STATE >>
        if (gasPressed && fuelAmount > 0)
        {
            // in air force
            if (state == DRIVE_STATE.IN_AIR)
            {
                //Debug.Log("airForce");
                rb_vehicle.AddForce(inAirForce * rb_vehicle.mass);
            }
            // on ground force
            else if (state == DRIVE_STATE.GROUNDED || state == DRIVE_STATE.UPHILL_GROUNDED)
            {

                int curGroundPointIndex = stageManager.PosToGroundPointIndex(rb_vehicle.position);
                currGroundSlopeAngle = stageManager.allLevelGroundRotations[curGroundPointIndex];

                if (currGroundSlopeAngle >= uphillActivationAngle)
                {
                    state = DRIVE_STATE.UPHILL_GROUNDED;
                    rb_vehicle.AddForce(uphillForce * rb_vehicle.mass);
                }
                else
                {
                    //Debug.Log("groundForce");
                    state = DRIVE_STATE.GROUNDED;
                    rb_vehicle.AddForce(groundedForce * rb_vehicle.mass);
                }

            }

            // subtract fuel amount
            fuelAmount--;
        }

        // << NITRO STATE >>
        if (state == DRIVE_STATE.NITRO)
        {
            rb_vehicle.AddForce(nitroForce * rb_vehicle.mass);
        }

        // << PERFECT BOOST STATE >>
        if (state == DRIVE_STATE.PERFECT_LANDING)
        {
            rb_vehicle.AddForce(perfectLandingBoostForce * rb_vehicle.mass);
        }

        // << ROTATE CAR >>
        rb_vehicle.angularVelocity = Mathf.Lerp(rb_vehicle.angularVelocity, rotationDir * rotationSpeed, Time.deltaTime);

        // << CLAMP HORIZONTAL VELOCITY >>
        rb_vehicle.velocity = new Vector2(Vector2.ClampMagnitude(rb_vehicle.velocity, velocityClamp).x, rb_vehicle.velocity.y);

    }

    public void Inputs()
    {
        // << GAS INPUT >>
        gasPressed = Input.GetKey(gasInputKey);

        // << ROTATION INPUT >>
        if (Input.GetKey(rotateLeft)) { rotationDir = -1; }
        else if (Input.GetKey(rotateRight)) { rotationDir = 1; }
        else { rotationDir = 0; }

        // << NITRO INPUT >>
        // if not in nitro ,, key is pressed && nitro charge left
        if (state != DRIVE_STATE.NITRO && Input.GetKeyDown(nitroInputKey) && nitroCharges > 0)
        {
            StartCoroutine(NitroBoost());
            StartCoroutine(cameraHandler.Shake(activeNitroTime, cameraHandler.nitro_camShakeMagnitude));

            try
            {
                audioManager.Play(audioManager.nitroBoostSFX); //NITRO BOOST SOUND EFFECT
            }
            catch { Debug.LogWarning("nitroBoostSFX :: Could not find AudioManager", this.gameObject); }

        }
    }

    public void StateMachine()
    {
        switch (state)
        {
            case DRIVE_STATE.NONE:
            case DRIVE_STATE.IN_AIR:
            case DRIVE_STATE.GROUNDED:

                // update in air / ground drive
                if (groundColliderList.Count > 0) { state = DRIVE_STATE.GROUNDED; }
                else { state = DRIVE_STATE.IN_AIR; }
                break;
            default:
                break;
        }
    }

    // override all states and 
    public IEnumerator NitroBoost()
    {
        state = DRIVE_STATE.NITRO;
        nitroCharges--;
        drivingUIManager.updateNitro();


        yield return new WaitForSeconds(activeNitroTime);

        state = DRIVE_STATE.NONE;
    }

    // override all states and 
    public IEnumerator PerfectLandingBoost()
    {
        state = DRIVE_STATE.PERFECT_LANDING;

        StartCoroutine(cameraHandler.Shake(activePerfectBoostTime, cameraHandler.perfect_camShakeMagnitude));


        yield return new WaitForSeconds(activePerfectBoostTime);

        state = DRIVE_STATE.NONE;
    }

    public float GetFuel()
    {
        return (float)fuelAmount / maxFuel;
    }
    public int GetNitro()
    {
        return nitroCharges;
    }
    public Vector2 GetVelocity()
    {
        return rb_vehicle.velocity;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, groundColliderHeightOffset), groundColliderSize);

        // draw ray to show current velocity of rigidbody
        if (rb_vehicle != null)
        {
            Vector3 velocity = rb_vehicle.velocity;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, velocity.normalized * velocity.magnitude * gizmoSize);
        }


    }
}
