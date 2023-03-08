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
    public void AddLandmarkCoordToList( List<float> PercentList)
    {
        for (int i = 0; i < PercentList.Count-1; i++)
        {
            Landmarks.Add(LocTracker.GetPointAtPercentage(PercentList[i]));
        }
        return;
    }

    //EX For GetPercentagesNeeded: if 5 landmarks wanted then youll get 0,20,40,60,80,100 added to list PecentagesNeeded
    public List<float> GetPercentagesNeeded(int LandmarkNum)
    {
        List<float> PercentagesNeeded = new List<float>();
        LandmarkNum = LandmarkNum-1;
        float percent = 0.0F;
        PercentagesNeeded.Add(percent); // add start to the list 
        for (int i = 1; i <= LandmarkNum; i++)
        {
            percent = Mathf.FloorToInt( (i/ LandmarkNum)*100);
            PercentagesNeeded.Add(percent);
        }
        return PercentagesNeeded;
    }


    public Rigidbody2D rb_landmark;//dunno if we need this cause idk about Rigibody2D that much
}
