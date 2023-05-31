using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelIndicator : MonoBehaviour
{
    Vehicle vehicle;
    public Animator anim;
    public GameObject flipCountFx;


    // Start is called before the first frame update
    void Start()
    {
        vehicle = GetComponentInParent<Vehicle>();
    }

    // Update is called once per frame
    void Update()
    {

        // NO GAS UI
        if (vehicle.fuelAmount <= 0)
        {
            anim.Play("NoGasIndicator");
        }


        // NO NITRO UI
        if (vehicle.nitroCharges <= 0)
        {
            anim.Play("NoNitro");
        }

    }
}
