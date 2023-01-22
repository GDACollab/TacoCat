using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 
    GetFuel()
*/
public class Vehicle : MonoBehaviour
{
    bool jumpPress;
    bool gas;
    bool reverse;
    public float acceleration = 10.0f;
    public int fuel = 500;
    private float thurst = 500.0f;
    private float gravity = 50.0f;
    Rigidbody2D rb_vehicle;
    
    // Start is called before the first frame update
    void Start()
    {
        rb_vehicle = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        jumpPress = Input.GetKeyDown("space");
        gas = Input.GetKey("right");
        reverse = Input.GetKey("left");
        if(jumpPress) { //There is no Jump
            print("space key pressed: JUMP");
            rb_vehicle.AddForce(Vector2.up * thurst);
        }
        if(gas) {
            print("right key pressed: VRoom");
            rb_vehicle.velocity += new Vector2(acceleration,0);
            fuel--;
        }
        if(reverse) {
            print("left key pressed: un-VRoom");
            rb_vehicle.velocity += new Vector2(-acceleration,0);
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
