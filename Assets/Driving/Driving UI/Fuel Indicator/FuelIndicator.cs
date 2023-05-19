using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelIndicator : MonoBehaviour
{
    Vehicle vehicle;
    public GameObject noGasPopup;
    public GameObject noNitroPopup;

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
            noGasPopup.SetActive(true);
        }
        else
        {
            noGasPopup.SetActive(false);
        }


        // NO NITRO UI
        if (vehicle.nitroCharges <= 0)
        {
            noNitroPopup.SetActive(true);
        }
        else
        {
            noNitroPopup.SetActive(false);
        }

    }
}
