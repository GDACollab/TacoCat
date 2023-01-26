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
    WheelJoint2D frontMotor;
    WheelJoint2D backMotor;

    [Header("Inputs")]
    public bool gasPressed; // increase gravity force on truck

    [Header("Movement Settings")]
    public Vector2 startingVelocity;

    public Vector2 playerGasForce; // input based force on truck

    [Header("Values")]
    public int fuelAmount = 100000;
    public float horizontalBoost; //Relative forward boost while grounded

    [Header("Debug Settings")]
    [Range(0.1f, 10)]
    public float gizmoSize = 1;

    [HideInInspector]
    public bool grounded;

    private Vector3 startPosition;    

    /* Jump Test */
    bool jumpPress;
    private float thurst = 500.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb_vehicle.velocity = startingVelocity;
        startPosition = transform.position;

        frontMotor = frontWheel.GetComponent<WheelJoint2D>();
        frontMotor = frontWheel.GetComponent<WheelJoint2D>();

    }

    // Update is called once per frame
    void Update()
    {
        gasPressed = Input.GetKey("right");
        
        /* Jump Test */
        jumpPress = Input.GetKeyDown("space");
        if(jumpPress) {
            print("space key pressed: JUMP");
            rb_vehicle.AddForce(Vector2.up * thurst);
        }
       
        // << PLAYER GAS INPUT >>
        if( gasPressed && fuelAmount > 0)
        {
            rb_vehicle.AddForce(playerGasForce * rb_vehicle.mass);

            fuelAmount--;
        }
    }

    void FixedUpdate() {
        // constant gravity
        rb_vehicle.AddForce(Vector2.down * Physics2D.gravity * rb_vehicle.mass);
    }



    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Terrain") {
            print("Grounded");
            grounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.tag == "Terrain") {
            print("Un-Grounded");
            grounded = false;
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

        // draw ray to show current velocity of rigidbody
        if (rb_vehicle != null)
        {
            Vector3 velocity = rb_vehicle.velocity;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, velocity.normalized * velocity.magnitude * gizmoSize);
        }


    }
}
