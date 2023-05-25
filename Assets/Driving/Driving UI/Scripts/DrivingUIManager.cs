using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrivingUIManager : MonoBehaviour
{
    [HideInInspector]
    public DrivingGameManager drivingGameManager;
    public GameObject vehicle;
    Vehicle vehicle_script;
    FlipTracker flipTracker;

    [HideInInspector]
    public CameraEffectManager cameraEffectManager;

    private float fuelAmount = 0; // Initial fuel
    private int numNitro = 0; // Initial nitro
    private int miles = 0;
    public List<int> signDistances;
    private int signNum = 0;

    

    // Public variables used to define the UI objects
    // Need these for Circle-style fuel guage. They need the "FuelUIGauge" tag.
    [Header("Circle-style UI objects")]
    public Transform fuelSlider;

    public List<Image> nitroCharges;

    [Header("Debugging UI objects")]
    public TMP_Text velocityText;
    public TMP_Text flipText;
    public TMP_Text signText;
    public int numSigns = 4;
    public int totalMiles = 1000;

    [Header("Distance")]
    public Transform pointer;
    public Transform pointerStart;
    public Transform pointerEnd;

    [Header("Toggle Progress Bar")]
    private GameObject progressBarSlider;

    [Header("Tutorial Canvas")]
    public GameObject tutorialCanvas;

    [Header("Transition UI")]
    public GameObject transitionParent;
    public TMP_Text transitionMessage;
    private bool endOfGame = false;
    
    // Start is called before the first frame update
    void Start()
    {
        drivingGameManager.GetComponentInParent<DrivingGameManager>();
        cameraEffectManager = drivingGameManager.camHandler.GetComponent<CameraEffectManager>();
        vehicle_script = vehicle.GetComponent<Vehicle>();
        flipTracker = vehicle.GetComponent<FlipTracker>();

        initFuelNitro(vehicle_script.GetFuel(),vehicle_script.GetNitro());
        miles = totalMiles;

        signDistances = drivingGameManager.getSignDistances(numSigns, totalMiles);
        signDistances.Add(0);
    }

    // Update is called once per frame
    void Update()
    {
        updateFuel();
        //updateNitro();
        
        velocityText.text = "Velocity: " + vehicle.GetComponent<Rigidbody2D>().velocity.x;

        flipText.text = "Flip Count: " + flipTracker.flipCount;
        
        // update the distance travelled
        if (drivingGameManager.percentageTraveled < 0) { pointer.position = pointerStart.position; }
        else if (drivingGameManager.percentageTraveled > 1) { pointer.position = pointerEnd.position; }
        else
        {
            pointer.position = Vector3.Lerp(pointerStart.position, pointerEnd.position, drivingGameManager.percentageTraveled);
        }
    }

    // Function to initialize the fuel and nitro. Called outside this class.
    void initFuelNitro(float fuel, int nitro){
        fuelAmount = fuel;
        numNitro = (nitro>3) ? 3: nitro;
        updateFuel();
        updateNitro();
    }
    
    // Function to update the fuel gauge
    void updateFuel(){
        //Vector3 nRotation = new Vector3(0f, 0f, 50f - 140 * Mathf.Clamp(vehicle_script.GetFuel(), 0, 1f));
        //adjust first value's float multiplier to change end pos, second to change start pos. Unsure what the math is, guessing should work well enough hopefully
        Vector3 nRotation = Vector3.Lerp(Vector3.forward * -175f, Vector3.forward * 50f, vehicle_script.GetFuel());

        fuelSlider.transform.eulerAngles = nRotation;
    }
    
    // Function to update the nitro charges

    public void updateNitro() // Called in Vehicle.cs when nitro is updated
    {
        // Gather nitro count
        numNitro = (vehicle_script.GetNitro() > 3) ? 3 : vehicle_script.GetNitro();
        
        for (int i = 2; i >= 0; i--)
        {
            if (i >= numNitro)
            {
                nitroCharges[i].gameObject.SetActive(false);
            }
            else
            {
                nitroCharges[i].gameObject.SetActive(true);
            }
        }
    }

    public void decrementNitro(){ //called in Vehicle.cs when nitro is decremented
    //removes the proper nitro fill 

        numNitro = (vehicle_script.GetNitro()>3) ? 3: vehicle_script.GetNitro();
        //Debug.Log(numNitro);
        for(int i = 2; i>=0;i--){
            if(nitroCharges[i].gameObject.activeInHierarchy){
                //Debug.Log(i);
                nitroCharges[i].gameObject.SetActive(false);
                break;
            }
        }
    }
    
    public void GameEndCanvas(string message){
        transitionParent.SetActive(true);
        transitionMessage.text = message;
    }

}
 