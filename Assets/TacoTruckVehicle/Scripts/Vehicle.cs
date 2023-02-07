using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Functions
    public int GetFuel()
    public Vector2 GetVelocity()
    public Vector3 GetPosition()
*/
public class Vehicle : MonoBehaviour
{
    public Rigidbody2D rb_vehicle;
    public GameObject frontWheel;
    public GameObject backWheel;


    [Space(10)]
    public LayerMask groundLayer;
    public List<Collider2D> groundColliderList;
    public float groundColliderSize = 15;

    [Header("States")]
    public bool isGrounded;
    public bool inAir;
    public bool gasPressed; // increase gravity force on truck
    public bool jumpState;

    [Header("Forces")]
    public Vector2 startingVelocity; // initial velocity
    public Vector2 inAirForce; // input based force on truck
    public Vector2 groundedForce; // input based force on truck
    public float rotationSpeed = 50f;

    [Header("Inputs")]
    public KeyCode gas;
    public KeyCode rotateRight;
    public KeyCode rotateLeft;

    [Header("Values")]
    public int fuelAmount = 100000;
    [Range(0, 100)]
    public float gravity = 9.81f;
    public float horizontalBoost; //Relative forward boost while grounded

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
    }

    void FixedUpdate() {


        // << CONSTANT GRAVITY >>
        rb_vehicle.AddForce(Vector2.down * gravity * rb_vehicle.mass);

        // << CHECK GROUND >>
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(transform.position, groundColliderSize, groundLayer);
        groundColliderList = new List<Collider2D>(groundColliders);

        // set bool
        if (groundColliderList.Count > 0) { isGrounded = true; inAir = false; }
        else { isGrounded = false; inAir = true; }

        // << GAS STATE >>
        if (gasPressed && fuelAmount > 0)
        {
            // in air force
            if (!isGrounded)
            {
                Debug.Log("airForce");
                rb_vehicle.AddForce(inAirForce * rb_vehicle.mass);
            }
            // on ground force
            else
            {
                Debug.Log("groundForce");
                rb_vehicle.AddForce(groundedForce * rb_vehicle.mass);

            }

            fuelAmount--;
        }

    }

    public void Inputs()
    {
        gasPressed = Input.GetKey(gas);

        if (Input.GetKey(rotateLeft))
        {
            rb_vehicle.angularVelocity = Mathf.Lerp(rb_vehicle.angularVelocity, -rotationSpeed, Time.deltaTime);
        }
        else if (Input.GetKey(rotateRight))
        {
            rb_vehicle.angularVelocity = Mathf.Lerp(rb_vehicle.angularVelocity, rotationSpeed, Time.deltaTime);
        }

    }



    public int GetFuel() {
        return fuelAmount;
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
