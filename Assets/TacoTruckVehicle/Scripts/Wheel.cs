using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    private GameObject vehicle;
    Vehicle vehicle_script;
    private float angle_rotation;
    float vel_y;
    float vel_x;
    void Start()
    {
        vehicle = GameObject.Find("Vehicle");
        vehicle_script = vehicle.GetComponent<Vehicle>();
    }

    // Update is called once per frame
    void Update()
    {
        angle_rotation = findRotation();
        transform.Rotate(0,0,angle_rotation, Space.World);

    }

    float findRotation() {
        /*
        Rotation Based on Velocity
        Problem: Wheel shouldn't spin faster if the velocty
        is grained when not in contact with a surface.
        Solution: Probably doesn't matter if we use y velocity 
        or not
        */
        //vel_y = vehicle_script.GetVelocity().y;
        vel_x = vehicle_script.GetVelocity().x;
        //return (Mathf.Sqrt(vel_x * vel_x + vel_y * vel_y))/10;
        return vel_x;
    }
}
