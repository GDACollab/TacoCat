using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelGuageUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text fuelText;
    public Image fuelGuage;
    public TMP_Text nitroText;
    public Image nitroGuage;
    private GameObject vehicle;
    Vehicle vehicle_script;
    private float initFuel = 0;
    private float initNitro = 0;

    void Start()
    {
        //fuelText = GetComponent<TMP_Text>();
        //fuelGuage = GetComponent<Image>();
        vehicle = GameObject.FindGameObjectWithTag("Vehicle");
        vehicle_script = vehicle.GetComponent<Vehicle>();
        initFuel = vehicle_script.GetFuel();
        initNitro = vehicle_script.GetNitro();
    }

    // Update is called once per frame
    void Update()
    {
        fuelText.text = "Fuel: " + vehicle_script.GetFuel() + "\n";
        fuelGuage.fillAmount = Mathf.Clamp(vehicle_script.GetFuel()/initFuel, 0, 1f);
        fuelGuage.color = Color.HSVToRGB(122f*vehicle_script.GetFuel()/initFuel/360, 0.71f, 0.96f, true);
        nitroText.text = "Nitro: " + vehicle_script.GetNitro() + "\n";
        nitroGuage.fillAmount = Mathf.Clamp(vehicle_script.GetNitro()/initNitro, 0, 1f);
        nitroGuage.color = Color.HSVToRGB((360-125*vehicle_script.GetNitro()/initNitro)/360, 0.71f, 0.96f, true);
    }
}
