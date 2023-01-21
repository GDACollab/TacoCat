using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    bool jumpPress;
    bool gas;
    bool reverse;
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
            rb_vehicle.velocity = new Vector2(10,0);
        }
        if(reverse) {
            print("left key pressed: un-VRoom");
            rb_vehicle.velocity = new Vector2(-10,0);
        }
    }
    void FixedUpdate() {
        //rb_vehicle.velocity = new Vector2(10,0);
        rb_vehicle.AddForce(Vector2.down * gravity * rb_vehicle.mass);
    }
}
