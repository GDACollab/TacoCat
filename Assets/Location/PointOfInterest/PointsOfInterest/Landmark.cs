using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : PointOfInterest
{
    public Rigidbody2D rb_landmark;//dunno if we need this cause idk about Rigibody2D that much
    public Vector3 location;
    // Start is called before the first frame update
    void Start()
    {
        location = _location;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
