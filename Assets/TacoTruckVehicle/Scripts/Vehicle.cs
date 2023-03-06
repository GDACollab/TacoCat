using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum driveState { NONE, START_DRIVE, GROUNDED, IN_AIR, NITRO, CRASH, END_DRIVE }

public class Vehicle : MonoBehaviour
{
    public Rigidbody2D rb_vehicle;

    [Space(10)]
    public LayerMask groundLayer;
    public List<Collider2D> groundColliderList;
    public float groundColliderSize = 15;

    [Header("States")]
    public driveState state = driveState.START_DRIVE;
    public bool gasPressed; // increase gravity force on truck
    public int rotationDir;

    [Header("General Driving")]
    public float gravity;

    [Space(10)]
    public int fuelAmount;
    public Vector2 startingVelocity; // initial velocity
    public Vector2 inAirForce; // input based force on truck
    public Vector2 groundedForce; // input based force on truck

    [Space(20)]
    public float velocityClamp = 500;

    [Space(10)]
    public float rotationSpeed = 50f;

    [Header("Nitro")]
    public int nitroCharges = 3;
    public Vector2 nitroForce;
    public float activeNitroTime = 5; // how long each charge lasts

    [Header("Inputs")]
    public KeyCode gasInputKey;
    public KeyCode nitroInputKey;
    public KeyCode rotateRight;
    public KeyCode rotateLeft;

    [Header("Debug Settings")]
    [Range(0.1f, 10)]
    public float gizmoSize = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        rb_vehicle.velocity = startingVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();

        StateMachine();
    }

    void FixedUpdate() {

        // << CONSTANT GRAVITY >>
        rb_vehicle.AddForce(Vector2.down * gravity * rb_vehicle.mass * Time.deltaTime);

        // << CHECK FOR GROUND COLLIDERS >>
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(transform.position, groundColliderSize, groundLayer);
        groundColliderList = new List<Collider2D>(groundColliders);

        // << GAS STATE >>
        if (gasPressed && fuelAmount > 0)
        {
            // in air force
            if (state == driveState.IN_AIR)
            {
                //Debug.Log("airForce");
                rb_vehicle.AddForce(inAirForce * rb_vehicle.mass);
            }
            // on ground force
            else if (state == driveState.GROUNDED)
            {
                //Debug.Log("groundForce");
                rb_vehicle.AddForce(groundedForce * rb_vehicle.mass);

            }

            // subtract fuel amount
            fuelAmount--;
        }

        // << NITRO STATE >>
        if (state == driveState.NITRO)
        {
            rb_vehicle.AddForce(nitroForce * rb_vehicle.mass);
        }

        // << ROTATE CAR >>
        rb_vehicle.angularVelocity = Mathf.Lerp(rb_vehicle.angularVelocity, rotationDir * rotationSpeed, Time.deltaTime);

        // << CLAMP VELOCITY >>
       rb_vehicle.velocity = Vector2.ClampMagnitude(rb_vehicle.velocity, velocityClamp);
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
        if (state != driveState.NITRO && Input.GetKeyDown(nitroInputKey) && nitroCharges > 0)
        {
            StartCoroutine(NitroBoost());
        }
    }

    public void StateMachine()
    {
        // if not in nitro mode
        if (state != driveState.NITRO && state != driveState.CRASH)
        {
            // set in air / ground drive
            if (groundColliderList.Count > 0) { state = driveState.GROUNDED; }
            else { state = driveState.IN_AIR; }
        }
    }

    // override all states and 
    public IEnumerator NitroBoost()
    {
        state = driveState.NITRO;
        nitroCharges--;

        yield return new WaitForSeconds(activeNitroTime);

        state = driveState.IN_AIR;
    }

    public int GetFuel() {
        return fuelAmount;
    }
    public int GetNitro() {
        return nitroCharges;
    }
    public Vector2 GetVelocity() {
        return rb_vehicle.velocity;
    }
    public Vector3 GetPosition() {
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        //Gizmos.DrawSphere(transform.position, groundColliderSize);

        // draw ray to show current velocity of rigidbody
        if (rb_vehicle != null)
        {
            Vector3 velocity = rb_vehicle.velocity;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, velocity.normalized * velocity.magnitude * gizmoSize);
        }


    }
}
