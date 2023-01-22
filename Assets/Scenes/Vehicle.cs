using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 
    GetFuel()
*/
public class Vehicle : MonoBehaviour
{
    bool gas; //Increase Gravity
    public float gravity = 50.0f;
    public float gravity_boost = 10.0f;
    public float starting_velocity = 20.0f;
    public int fuel = 500;
    Rigidbody2D rb_vehicle;
    
    // Start is called before the first frame update
    void Start()
    {
        rb_vehicle = GetComponent<Rigidbody2D>();
        rb_vehicle.velocity += new Vector2(starting_velocity,0);
        
    }

    // Update is called once per frame
    void Update()
    {
        //jumpPress = Input.GetKeyDown("space");
        gas = Input.GetKey("right");

        if(gas) { //Increase Gravity
            print("right key pressed: VRoom");
            //rb_vehicle.velocity += new Vector2(0,-gravity_boost);
            rb_vehicle.AddForce(Vector2.down * gravity_boost * rb_vehicle.mass);
            fuel--;
        }

    }
    void FixedUpdate() {
        rb_vehicle.AddForce(Vector2.down * gravity * rb_vehicle.mass);
    }

    public int GetFuel() {
        return fuel;
    }
}
