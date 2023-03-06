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
}
