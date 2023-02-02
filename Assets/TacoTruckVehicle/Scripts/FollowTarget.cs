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
            transform.position = Vector3.Lerp(transform.position, Round(target.position + offset, 0), camSpeed * Time.deltaTime);
            //transform.position = target.position + offset;

        }
    }

    public static Vector3 Round(Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
}
