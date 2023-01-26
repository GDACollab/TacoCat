using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    // Start is called before the first frame update
    int vehicle_posx;
    int vehicle_posy;
    public Transform vehicle;
    public Vector3 offset;
    public float camSpeed = 0.2f;
   
    // Update is called once per frame
    void LateUpdate()
    {
        //Camera shares x,y,z + offset(x,y,z)
        if(vehicle) {
            transform.position = Vector3.Lerp(transform.position, vehicle.position + offset, camSpeed * Time.deltaTime);
        }
    }
}
