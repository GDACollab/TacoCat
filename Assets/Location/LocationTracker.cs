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
        Debug.Log( "Distance to Destination from start:"+ CalculateDistanceFromStart(StartingLocation, Destination));
        Debug.Log("Distance to Destination:" + CalculateDistanceToDestination(s_vehicle, Destination));
    }

    private Vector2 CalculateDistanceToDestination(Vehicle vehicle, PointOfInterest end)
    {
        Vector3 vehiclePosition3D= vehicle.GetPosition();
        Vector2 vehiclePosition2D = new Vector2(vehiclePosition3D.x,vehiclePosition3D.y );

        return end.GetLocation() - vehiclePosition2D;  
    }
    private Vector2 CalculateDistanceFromStart(PointOfInterest start, PointOfInterest end)
    {
        return end.GetLocation()- start.GetLocation();
    }
}
