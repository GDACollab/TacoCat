using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCorrection : MonoBehaviour
{
    Rigidbody2D rb;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGGER ENTERED BY :: " + collision.name);


        // if triggered, then flip car


    }

    public void FlipCar()
    {
        // get the transform component of the truck
        // get the rigidbody component ( already made the variable above )

        // << ADJUST VELOCITY >>
        // set rb.velocity to 0

        // (( OPTIONAL : FREEZE X POS ))

        // << CHANGE TRANSFORM VALUES >>
        // move it up (y position) a given amount of height specified in the global variables above ^^

        // and rotate the car back to z rotation 0


    }


}
