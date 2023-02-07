using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    // Start is called before the first frame update
    int vehicle_posx;
    int vehicle_posy;
    public Transform target;
    public Vector3 offset;
    public float camSpeed = 0.2f;
   
    // Update is called once per frame
    void FixedUpdate()
    {
        
        //Camera shares x,y,z + offset(x,y,z)
        if(target) {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, camSpeed * Time.deltaTime);
            //transform.position = target.position + offset;

        }
    }
}
