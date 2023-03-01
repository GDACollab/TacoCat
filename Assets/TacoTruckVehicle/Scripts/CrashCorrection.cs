using System.Collections;
using UnityEngine;

public class CrashCorrection : MonoBehaviour
{
    Vehicle vehicle;
    Rigidbody2D rb;
    public BoxCollider2D collisionTrigger;

    public float resetAngle = 0;
    public float rotationSpeed = 2;

    public bool isCorrecting;

    private void Awake()
    {
        vehicle = GetComponentInParent<Vehicle>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void Update()
    {
        if (isCorrecting) { RotateToResetAngle(); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CollisionTerrain"))
        {
            Debug.Log("Truck Crash with " + collision.name);
            StartCoroutine(CorrectCrash());
        }
    }

    // correct crash coroutine
    public IEnumerator CorrectCrash()
    {
        vehicle.state = driveState.CRASH; // set state

        // stop velocity ,, disable trigger
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        collisionTrigger.enabled = false;

        // move truck up from ground to flip
        vehicle.transform.position += Vector3.up * 10;
        isCorrecting = true;

        // wait until truck is flipped
        yield return new WaitUntil(() => IsTruckUpright(0.2f));

        // reset constraints,  trigger and bool
        rb.constraints = RigidbodyConstraints2D.None;
        collisionTrigger.enabled = true;
        isCorrecting = false;

        vehicle.state = driveState.GROUNDED;
    }

    // constantly rotate the truck to reset angle
    public void RotateToResetAngle()
    {
        // Get the current rotation of the object
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Set the z rotation to the target rotation
        currentRotation.z = Mathf.MoveTowardsAngle(currentRotation.z, resetAngle, rotationSpeed * Time.deltaTime);

        // Apply the new rotation to the object
        transform.rotation = Quaternion.Euler(currentRotation);
    }

    // check to see is truck is within reset angle buffer
    public bool IsTruckUpright(float buffer)
    {
        // Get the current rotation of the object
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float difference = Mathf.Abs(currentRotation.z - resetAngle);

        Debug.Log("difference " + difference);

        if (difference < buffer)
        {
            return true;
        }

        Debug.Log("Rotation Difference " + currentRotation.z);

        return false;
    }

}
