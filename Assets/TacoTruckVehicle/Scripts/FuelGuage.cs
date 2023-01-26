using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelGuage : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text fuelGuage;
    private GameObject vehicle;
    Vehicle vehicle_script;

    void Start()
    {
        fuelGuage = GetComponent<TMP_Text>();
        vehicle = GameObject.Find("Vehicle");
        vehicle_script = vehicle.GetComponent<Vehicle>();
    }

    // Update is called once per frame
    void Update()
    {
        fuelGuage.text = "Fuel: " + vehicle_script.GetFuel() + "\n";
        fuelGuage.text += "xVelocity: " + vehicle_script.GetVelocity().x;
    }
}
