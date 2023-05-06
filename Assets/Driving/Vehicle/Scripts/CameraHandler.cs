using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    private Camera cam;
    private Rigidbody2D vehicleRb;

    public GameObject vehicle;
    public GameObject ground;
    public float vehicleHeight;

    [Space(10)]
    [Range(-1000, 1000)]
    public float groundLineY = 0;
    public float smoothTime = 0.3f;

    [Space(10)]
    public float camSpeed = 0.2f;

    [Header("Camera Shake")]
    [Range(0, 1)]
    public float perfect_camShakeMagnitude = 0.5f;
    [Range(0, 1)]
    public float nitro_camShakeMagnitude = 0.5f;


    [Header("Parameters")]
    public Vector2 velocityRange = new Vector2(300, 1000); //the range of velocity the camera should adjust for
    
    [Space(30)]
    [Header("Adjustment Ranges")]
    public Vector2 xPosRange = new Vector2(0, -100); // the range of z positions the camera should adjust between
    public Vector2 zPosRange = new Vector2(-200, -500); // the range of z positions the camera should adjust between
    public Vector3 currOffset;

    private void Start()
    {
        cam = GetComponent<Camera>();
        vehicleRb = vehicle.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        // if vehicle  is found
        if(vehicle) 
        {
            BoundingTracker();

            /*
            // Calculate the current velocity as a percentage of the velocityRange
            float velocityPercent = Mathf.InverseLerp(velocityRange.x, velocityRange.y, vehicleRb.velocity.magnitude);
            var emitter = vehicle.GetComponent<FMODUnity.StudioEventEmitter>();
            emitter.SetParameter("RPM", velocityPercent);
            // Linearly interpolate the camera's z-position based on the velocityPercent
            float zPos = Mathf.Lerp(zPosRange.x, zPosRange.y, velocityPercent);
            float xPos = Mathf.Lerp(xPosRange.x, xPosRange.y, velocityPercent);

            // Linearly interpolate the camera's y-position based on the height
            float heightPercent = Mathf.InverseLerp(heightRange.x, heightRange.y, vehicle.transform.position.y - ground.transform.position.y);

            float yPos = Mathf.Lerp(yPosRange.x, yPosRange.y, heightPercent);

            // Update the camera's position with the new z-position
            currOffset = new Vector3(xPos, yPos, zPos);
            transform.position = Vector3.Lerp(transform.position, vehicle.transform.position + currOffset, camSpeed * Time.deltaTime);
            */    
        }
        
    }

    private void BoundingTracker()
    {

        // Calculate the desired camera position
        Vector3 desiredPos = vehicleRb.position;

        float velocityPercent = Mathf.InverseLerp(velocityRange.x, velocityRange.y, vehicleRb.velocity.magnitude);
        desiredPos.x = desiredPos.x + Mathf.Lerp(xPosRange.x, xPosRange.y, velocityPercent);
        desiredPos.y = groundLineY + ((vehicleRb.position.y - groundLineY) / 2);

        vehicleHeight = Mathf.Abs(vehicleRb.position.y - groundLineY);
        desiredPos.z = desiredPos.z + Mathf.Lerp(zPosRange.x, zPosRange.y, velocityPercent);



        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothTime * Time.deltaTime);
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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(-10000, groundLineY), new Vector2(10000, groundLineY));
    }
}
