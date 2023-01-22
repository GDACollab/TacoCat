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
    public float gravityBoost;
    public float stopTollerance;

    [HideInInspector]
    public bool vehicleCollision;
    Rigidbody2D rb_vehicle;
    
    private Vector3 startPosition;
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

        if(gas && fuel > 0) { //Increase Gravity
            print("right key pressed: Gravity Increase");
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
