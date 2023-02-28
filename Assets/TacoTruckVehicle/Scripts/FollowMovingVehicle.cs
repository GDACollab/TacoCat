using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMovingVehicle : MonoBehaviour
{

    public GameObject vehicle;
    private Rigidbody2D vehicleRb;
    public float camSpeed = 0.2f;
    public Vector2 velocityRange = new Vector2(300, 1000); //the range of velocity the camera should adjust for
    public Vector2 xPosRange = new Vector2(0, -100); // the range of z positions the camera should adjust between
    public Vector2 zPosRange = new Vector2(-200, -500); // the range of z positions the camera should adjust between

    public Vector3 currOffset;

    private void Start()
    {
        vehicleRb = vehicle.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if vehicle  is found
        if(vehicle) {

            // Calculate the current velocity as a percentage of the velocityRange
            float velocityPercent = Mathf.InverseLerp(velocityRange.x, velocityRange.y, vehicleRb.velocity.magnitude);

            // Linearly interpolate the camera's z-position based on the velocityPercent
            float zPos = Mathf.Lerp(zPosRange.x, zPosRange.y, velocityPercent);
            float xPos = Mathf.Lerp(xPosRange.x, xPosRange.y, velocityPercent);

            // Update the camera's position with the new z-position
            currOffset = new Vector3(xPos, currOffset.y, zPos);
            transform.position = Vector3.Lerp(transform.position, vehicle.transform.position + currOffset, camSpeed * Time.deltaTime);
            
        }



    }
}