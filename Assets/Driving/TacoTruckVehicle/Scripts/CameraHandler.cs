using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    private Rigidbody2D vehicleRb;

    public GameObject vehicle;
    public GameObject ground;

    [Space(10)]
    public float camSpeed = 0.2f;

    [Header("Camera Shake")]
    [Range(0, 1)]
    public float perfect_camShakeMagnitude = 0.5f;
    [Range(0, 1)]
    public float nitro_camShakeMagnitude = 0.5f;


    [Header("Parameters")]
    public Vector2 velocityRange = new Vector2(300, 1000); //the range of velocity the camera should adjust for
    public Vector2 heightRange = new Vector2(300, 1000);
    
    [Space(30)]
    [Header("Adjustment Ranges")]
    public Vector2 xPosRange = new Vector2(0, -100); // the range of z positions the camera should adjust between
    public Vector2 yPosRange = new Vector2(0, -200);
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

            // Linearly interpolate the camera's y-position based on the height
            float heightPercent = Mathf.InverseLerp(heightRange.x, heightRange.y, vehicle.transform.position.y - ground.transform.position.y);

            float yPos = Mathf.Lerp(yPosRange.x, yPosRange.y, heightPercent);

            // Update the camera's position with the new z-position
            currOffset = new Vector3(xPos, yPos, zPos);
            transform.position = Vector3.Lerp(transform.position, vehicle.transform.position + currOffset, camSpeed * Time.deltaTime);
            
        }
    }


    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position += new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return 0;
        }
    }






    //Camera Zero stuff

    //List of variables used to determine points
    // x/y values of current points
    public float a_x_pos;
    public float a_y_pos;
    public float b_x_pos;
    public float b_y_pos;
    // slope/constant values used in equation
    public float slope;
    public float constant;

    public List<Vector3> bezierPoints = new List<Vector3>(); //List of points used to determine where the 'zero' point should be
    public int bezierPointsListTracker = 0; //Current positioning in the list of points

    //Called in the bezier generator, adds the P_1 and P_2 points to the bezierPoints list
    //(To do so, whenever a P_1/2 point is made, run this function with that point before moving onto the next point)
    public void addToBezierPoints(Vector3 point)
    {
        bezierPoints.Add(point);
    }

    //Determines the numbers used for making the equation
    public void MakeEquation(Vector3 A_pos, Vector3 B_pos)
    {
        a_x_pos = A_pos.x;
        a_y_pos = A_pos.y;
        b_x_pos = B_pos.x;
        b_y_pos = B_pos.y;
        slope = (b_y_pos - a_y_pos) / (b_x_pos - a_x_pos);
        constant = (b_y_pos - (slope * (-1))); //-1 = cos(pi)
    }

    //Runs the equation to determine that determines the current 'zero' point
    float CalculateZero(Vector3 car_pos) 
    {
        return slope * Mathf.Cos(car_pos.x * Mathf.PI / b_x_pos) + constant;
    }

    float GetCurrentZero()
    {
        //If the vehicle was able to be grabbed earlier in the script
        if (vehicle)
        {
            //We check if the car is NOT between the current 2 points
            if (!(a_x_pos <= vehicle.transform.position.x && vehicle.transform.position.x <= b_x_pos)) 
            {
                //If so, we either increment or decrement the tracker...
                if (b_x_pos > vehicle.transform.position.x)
                {
                    bezierPointsListTracker -= 1;
                }
                else
                {
                    bezierPointsListTracker += 1;
                }
                //And then form a new equation for 0!
                MakeEquation(bezierPoints[bezierPointsListTracker], bezierPoints[bezierPointsListTracker + 1]);
            }
            //Then we calculate and return a 'zero' point using the current (whether new or old) equation
            return CalculateZero(vehicle.transform.position);
        }
        //If no vehicle was grabbed, defaults to 0
        return 0.0f;
    }
    
}
