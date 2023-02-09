using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTracker : MonoBehaviour
{
    [Header("Player Vehicle")]
    public GameObject playerVehicle;
    public PointOfInterest StartingLocation;
    public PointOfInterest Destination;
    // LocationTracker has all the PointsOfInterests of the current scene
    [Header("List of Landmarks")]
    public List<PointOfInterest> allPointsOfInterest = new List<PointOfInterest>();

    [Header("Vehicle Location")]
    private Vehicle s_vehicle = null;
    public Vector3 vehiclePosition;
    // Start is called before the first frame update
    void Start()
    {
        allPointsOfInterest.Add(StartingLocation);
        allPointsOfInterest.Add(Destination);
        s_vehicle = playerVehicle.GetComponent<Vehicle>();//makes LocationTracker able to call Vehicle.GetPosition()
        vehiclePosition = s_vehicle.GetPosition();
        Debug.Log(vehiclePosition);
    }

    // Update is called once per frame
    void Update()
    {
        vehiclePosition = s_vehicle.GetPosition();
        //Debug.Log(vehiclePosition);
    }

    private Vector2 CalculateDistance(PointOfInterest A, PointOfInterest B)
    {

    }
}
