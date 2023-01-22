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
    bool gas; //Increase Gravity
    [Header("Initial Settings")]
    public Vector2 startingVelocity;
    public float gravity;
    public int fuel;
    public float gravityBoost; //Gravity increase while in air
    public float horizontalBoost; //Relative forward boost while grounded
    public float stopTollerance;

    [HideInInspector]
    public bool grounded;
    Rigidbody2D rb_vehicle;

    private Vector3 startPosition;    

    /* Jump Test */
    bool jumpPress;
    private float thurst = 500.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb_vehicle = GetComponent<Rigidbody2D>();
        rb_vehicle.velocity += startingVelocity;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        gas = Input.GetKey("right");
        
        /* Jump Test */
        jumpPress = Input.GetKeyDown("space");
        if(jumpPress) {
            print("space key pressed: JUMP");
            rb_vehicle.AddForce(Vector2.up * thurst);
        }

        if(gas && grounded) {
            /* Gas pressed while grounded - Move horizontal */
            print("Vroom");
            rb_vehicle.velocity = transform.right * horizontalBoost;
            fuel--;
        } else if(gas && fuel > 0 && !grounded) {
            /* Gas press while air - Increase Gravity */
            print("Gravity Increase");
            rb_vehicle.AddForce(Vector2.down * gravityBoost * rb_vehicle.mass);
            fuel--;
        } else if (GetVelocity().magnitude < stopTollerance && fuel == 0) {
            /* If the vehicle "stops" moving then reset position */
            transform.position = startPosition;
        }

    }

    void FixedUpdate() {
        rb_vehicle.AddForce(Vector2.down * gravity * rb_vehicle.mass);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("funny");
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
        return fuel;
    }
    public Vector2 GetVelocity() {
        return rb_vehicle.velocity;
    }
    public Vector3 GetPosition() {
        return transform.position;
    }
}
