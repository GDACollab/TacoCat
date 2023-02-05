using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Funcitions: 
 * GetLocation()
 * 
 */
public class PointOfInterest : MonoBehaviour
{
    //Point of Interests are either 
    //A) A Landmark
    //B) A Gas Station
    //this is the location in the scene of a given Point of Interest
    public Vector3 _location;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 GetLocation() { return _location; }
}
