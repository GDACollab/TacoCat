using System.Collections;
using UnityEngine;

public class CrashCorrection : MonoBehaviour
{
    Vehicle vehicle;
    Rigidbody2D rb;
    public BoxCollider2D collisionTrigger;

    public float resetAngle = 0;
    public float rotationSpeed = 2;
    public GameObject collisionEffect;

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

    public IEnumerator CorrectCrash()
    {
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        collisionTrigger.enabled = false;

        vehicle.transform.position += Vector3.up * 10;

        isCorrecting = true;

        yield return new WaitUntil(() => IsTruckUpright(0.2f));


        rb.constraints = RigidbodyConstraints2D.None;

        collisionTrigger.enabled = true;

        isCorrecting = false;

    }

    public void RotateToResetAngle()
    {
        // Get the current rotation of the object
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Set the z rotation to the target rotation
        currentRotation.z = Mathf.MoveTowardsAngle(currentRotation.z, resetAngle, rotationSpeed * Time.deltaTime);

        // Apply the new rotation to the object
        transform.rotation = Quaternion.Euler(currentRotation);
    }

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
