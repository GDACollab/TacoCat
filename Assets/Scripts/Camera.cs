using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    // Start is called before the first frame update
    int vehicle_posx;
    int vehicle_posy;
    public Transform vehicle;
    public Vector3 offset;
    public float smooth = 0.2f;
   
    // Update is called once per frame
    void LateUpdate()
    {
        //Camera shares x,y,z + offset(x,y,z)
        if(vehicle) {
            transform.position = vehicle.position + offset;
        }
    }
}
