using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    bool gas;
    bool reverse;
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        gas = Input.GetKey("right");
        reverse = Input.GetKey("left");
        if(gas) {
            print("right key pressed: Wheel Spin");
            transform.Rotate(0,0,10, Space.World);
        }
        if(reverse) {
            print("left key pressed: Reverse Wheel Spin");
            transform.Rotate(0,0,-10, Space.World);
        }
    }
}
