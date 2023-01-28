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

    [Header("States")]
    public bool isGrounded;
    public bool inAir;
    public bool gasPressed; // increase gravity force on truck
    public bool jumpState;

    [Header("Forces")]
    public Vector2 startingVelocity; // initial velocity
    public Vector2 gasForce; // input based force on truck
    public Vector2 jumpForce; // jump force

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
        // constant gravity
        rb_vehicle.AddForce(Vector2.down * gravity * rb_vehicle.mass);

        // << GAS STATE >>
        if (gasPressed && fuelAmount > 0)
        {
            rb_vehicle.AddForce(gasForce * rb_vehicle.mass);

            fuelAmount--;
        }
    }

    public void Inputs()
    {
        gasPressed = Input.GetKey(gas);

        jumpState = Input.GetKeyUp(KeyCode.Space);

        // << JUMP STATE >>
        // needs to be in update because input is single-frame based
        if (jumpState)
        {
            Debug.Log("Jump");
            rb_vehicle.AddForce(jumpForce * rb_vehicle.mass, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Terrain") {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.tag == "Terrain") {
            isGrounded = false;
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
