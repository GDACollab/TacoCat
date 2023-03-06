using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrivingUIManager : MonoBehaviour
{
    public DrivingGameManager drivingGameManager;
    public GameObject vehicle;

    Vehicle vehicle_script;
    FlipTracker flipTracker;

    private float initFuel = 0; // Initial fuel
    private float initNitro = 0; // Initial nitro


    // Public variables used to define the UI objects
    // Need these for Bar-style fuel gauge. They need the "FuelUIBar" tag.
    [Header("Bar-style UI objects")]
    public TMP_Text fuelText;
    public Image fuelGuage;
    public TMP_Text nitroText;
    public Image nitroGuage;
    // Need these for Circle-style fuel guage. They need the "FuelUIGauge" tag.
    [Header("Circle-style UI objects")]
    public Transform fuelSlider;
    public Image nitroSlider;

    [Header("Debugging UI objects")]
    public TMP_Text velocityText;
    public TMP_Text flipText;

    [Header("UI Types")]
    // Change the type of fuel gauge UI. 0: Bar-style fuel gauge, 1: Circle-style fuel guage
    [Tooltip("The type of fuel gauge UI. \n 0: Bar, 1: Gauge")]
    public int gaugeType = 0; 
    // Change the way the fuel gauge UI bars are colored. 
    // 0: Color goes from green/blue to red smoothly, 1: Color goes from green/blue to yellow/purple at 50% to red at 25%
    [Tooltip("The type of fuel gauge bar coloe. \n 0: Smooth, 1: Gauge")]
    public int colorType = 0;

    [Header("Distance")]
    public Transform pointer;
    public Transform pointerStart;
    public Transform pointerEnd;


    
    // Start is called before the first frame update
    void Start()
    {
        //fuelText = GetComponent<TMP_Text>();
        //fuelGuage = GetComponent<Image>();

        vehicle_script = vehicle.GetComponent<Vehicle>();
        flipTracker = vehicle.GetComponent<FlipTracker>();
        initFuel = vehicle_script.GetFuel();
        initNitro = vehicle_script.GetNitro();



        // Change the UI based on type
        switch (gaugeType) 
        {
            case 1:
                initCircle();
                break;
            default:
                initBar();
                break;
        }




    }

    // Update is called once per frame
    void Update()
    {
        velocityText.text = "Velocity: " + vehicle.GetComponent<Rigidbody2D>().velocity.x;

        flipText.text = "Flip Count: " + flipTracker.flipCount;

        switch (gaugeType) 
        {
            case 1:
                fuelCircle();
                break;
            default:
                fuelBar();
                break;
        }

        // update the distance travelled
        pointer.position = Vector3.Lerp(pointerStart.position, pointerEnd.position, drivingGameManager.percentageTraveled);

    }

    // Functions to initialize the different types of gauges
    void initBar(){
        foreach(GameObject x in GameObject.FindGameObjectsWithTag("FuelUIGauge")){
            x.SetActive(false);
        }
        foreach(GameObject x in GameObject.FindGameObjectsWithTag("FuelUIBar")){
            x.SetActive(true);
        }
    }
    
    void initCircle(){
        foreach(GameObject x in GameObject.FindGameObjectsWithTag("FuelUIBar")){
            x.SetActive(false);
        }
        foreach(GameObject x in GameObject.FindGameObjectsWithTag("FuelUIGauge")){
            x.SetActive(true);
        }
    }
    
    // Functions to update the different types of gauges
    void fuelBar(){
        fuelText.text = "Fuel: " + vehicle_script.GetFuel()/initFuel*100 + "%\n";
        fuelGuage.fillAmount = Mathf.Clamp(vehicle_script.GetFuel()/initFuel, 0, 1f);
        fuelGuage.color = barColor(0);
        nitroText.text = "Nitro: " + vehicle_script.GetNitro()/initNitro*100 + "%\n";
        nitroGuage.fillAmount = Mathf.Clamp(vehicle_script.GetNitro()/initNitro, 0, 1f);
        nitroGuage.color = barColor(1);
    }
    
    void fuelCircle(){
        Vector3 nRotation = new Vector3(0f, 0f, (-90)*Mathf.Clamp(vehicle_script.GetFuel()/initFuel, 0, 1f));
        fuelSlider.transform.eulerAngles = nRotation;
        nitroSlider.fillAmount = Mathf.Clamp(vehicle_script.GetNitro()/initNitro, 0, 1f);
        nitroSlider.color = barColor(1);
    }
    
    // Changes the way the color changes on the bar
    Color barColor(int type){
        if(type==1){
            if(colorType==1){
                if(vehicle_script.GetNitro()/initNitro>0.5){
                    return Color.HSVToRGB(236f/360, 0.71f, 0.96f, true);
                }
                else if(vehicle_script.GetNitro()/initNitro>0.25){
                    return Color.HSVToRGB(260f/360, 0.71f, 0.96f, true);
                }
                else{
                    return Color.HSVToRGB(1f, 0.71f, 0.96f, true);
                }
            }
            else{
                return Color.HSVToRGB((360-125*vehicle_script.GetNitro()/initNitro)/360, 0.71f, 0.96f, true);
            }
        }
        else{
            if(colorType==1){
                if(vehicle_script.GetFuel()/initFuel>0.5){
                    return Color.HSVToRGB(122f/360, 0.71f, 0.96f, true);
                }
                else if(vehicle_script.GetFuel()/initFuel>0.25){
                    return Color.HSVToRGB(54f/360, 0.71f, 0.96f, true);
                }
                else{
                    return Color.HSVToRGB(0f, 0.71f, 0.96f, true);
                }
            }
            else{
                return Color.HSVToRGB(122f*vehicle_script.GetFuel()/initFuel/360, 0.71f, 0.96f, true);
            }
        }
    }
}
 