using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashCorrection : MonoBehaviour
{
    Vehicle vehicle;
    Rigidbody2D rb;

    public Vector3 resetRotation;
    public GameObject collisionEffect;

    private void Awake()
    {
        vehicle = GetComponentInParent<Vehicle>();
        rb = GetComponentInParent<Rigidbody2D>();
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
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        vehicle.transform.rotation = Quaternion.Euler(resetRotation);

        yield return null;
    }

}
