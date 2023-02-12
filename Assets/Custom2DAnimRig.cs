using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom2DAnimRig : MonoBehaviour
{
    /*
     * Create references to different sprites
     * place those sprites on corresponding bones (empty transforms)
     * animate those bones
     * 
     */

    [Header("Default Sprites")]
    public GameObject headObj;
    public GameObject bodyObj;
    public GameObject rightArmObj;
    public GameObject leftArmObj;
    public GameObject rightLegObj;
    public GameObject leftLegObj;

    [Header("Default Bones")]
    public Transform headBone;
    public Transform bodyBone;
    public Transform rightArmBone;
    public Transform leftArmBone;
    public Transform rightLegBone;
    public Transform leftLegBone;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
