using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTracker : MonoBehaviour
{
    public EnvironmentGenerator EnvGen;
    public Landmark Landmark;

    [Header("Player Vehicle")]
    public GameObject playerVehicle;
    public PointOfInterest startingLocation;
    public PointOfInterest destination;
    // LocationTracker has all the PointsOfInterests of the current scene
    [Header("List of Landmarks")]
    public List<PointOfInterest> allPointsOfInterest = new List<PointOfInterest>();

    [Header("Vehicle Location")]
    private Vehicle s_vehicle = null;
    public Vector2 vehiclePosition;
    private float totalDistance = 0;
    private float remainingDistance = 0;
    // Start is called before the first frame update
    void Start()
    {
        allPointsOfInterest.Add(startingLocation);
        allPointsOfInterest.Add(destination);
        s_vehicle = playerVehicle.GetComponent<Vehicle>();//makes LocationTracker able to call Vehicle.GetPosition()
        Vector3 vehiclePositon3D = s_vehicle.GetPosition();
        vehiclePosition = new Vector2(vehiclePositon3D.x, vehiclePositon3D.y);
        startingLocation.SetLocation(vehiclePosition);
        Debug.Log(vehiclePosition);
    }

    // Update is called once per frame
    void Update()
    {
        vehiclePosition = s_vehicle.GetPosition();
        totalDistance = CalculateDistanceFromStart(startingLocation, destination);
        remainingDistance = CalculateDistanceToDestination(s_vehicle, destination);
        //Debug.Log( "Distance to Destination from start:"+ totalDistance);
        /*Debug.Log("Distance to Destination:" + remainingDistance);
        Debug.Log("Distance in Percent:" + ConvertToPercent(remainingDistance, totalDistance));*/
    }

    private float CalculateDistanceToDestination(Vehicle vehicle, PointOfInterest end)
    {
        Vector3 vehiclePosition3D= vehicle.GetPosition();
        Vector2 vehiclePosition2D = new Vector2(vehiclePosition3D.x,vehiclePosition3D.y );

        return Vector2.Distance(vehiclePosition2D, end.GetLocation());  
    }
    private float CalculateDistanceFromStart(PointOfInterest start, PointOfInterest end)
    {
        return Vector2.Distance(start.GetLocation(), end.GetLocation());
    }

    private float ConvertToPercent(float numerator, float denominator) 
    {
        return (numerator/denominator)*100;
    }

    public Vector3 GetPointAtPercentage(float Percentage) {
        List<Vector3> Points = EnvGen.groundPoints; //Gets groundpoints from EnvironmentGenerator.cs
        int PointCount = EnvGen.groundPoints.Count; //Get the amount of groundpoints

        int GroundPointIndex = Mathf.FloorToInt(PointCount * Percentage);

        return Points[GroundPointIndex];
    }
}
