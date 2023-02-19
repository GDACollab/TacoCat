using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : PointOfInterest
{
    public LocationTracker LocTracker;
    public EnvironmentGenerator EnvGen;

    [Header("Number Of Landmarks")]
    public int LandmarkNum;

    public List<Vector3> Landmarks = new List<Vector3>();
    public void AddLandmarkCoordToList()
    {
        return;
    }

    //EX For GetPercentagesNeeded: if 5 landmarks wanted then youll get 0,20,40,60,80,100 added to list PecentagesNeeded
    public List<int> GetPercentagesNeeded(int LandmarkNum)
    {
        List<int> PercentagesNeeded = new List<int>();
        LandmarkNum = LandmarkNum-1;
        int percent = 0;
        PercentagesNeeded.Add(percent); // add start to the list 
        for (int i = 1; i <= LandmarkNum; i++)
        {
            percent = Mathf.FloorToInt( i/ LandmarkNum);
            PercentagesNeeded.Add(percent);
        }
        return PercentagesNeeded;
    }


    public Rigidbody2D rb_landmark;//dunno if we need this cause idk about Rigibody2D that much
}
