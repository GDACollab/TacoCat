using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomerManager : MonoBehaviour
{
    [HideInInspector]
    public TacoMakingGameManager tacoGameManager;
    public GameObject customerPrefab;
    public GameObject currCustomer;

    public Transform startPos, middlePos, endPos;      //Used as the points the customer transitions to/from
    [HideInInspector] public bool movingIn, movingOut; //Used for keeping track of if the current customer is transitioning in or out
    public float transitionTime;       //How long it takes in seconds for the customer to transition in/out of frame
    private float currTransitionTime;  //Used for keeping track of time during transitions


    //before calling check if customers left to generate == 0
    private void Start()
    {
        tacoGameManager = GetComponentInParent<TacoMakingGameManager>();
        movingIn  = false;
        movingOut = false;
        //DebugSpawnCustomers(10);
    }

    private void FixedUpdate()
    {
        //Moves the customer into frame
        if (movingIn)
        {
            if (currTransitionTime >= transitionTime)
            {
                movingIn = false;
            }
            else
            {
                //Uses Sin to have the Lerp slow down near the end
                float interpolater = currTransitionTime / transitionTime;
                interpolater = Mathf.Sin(interpolater * Mathf.PI * 0.5f);

                currCustomer.transform.position = Vector3.Lerp(startPos.position, middlePos.position, interpolater);
                currTransitionTime += Time.deltaTime;
                
            }           
        }

        //Moves the customer out of frame and then destroys them
        if (movingOut)
        {
            if (currTransitionTime >= transitionTime)
            {                
                movingOut = false;
                Destroy(currCustomer);
            }
            else
            {
                //Uses Cos to have the Lerp start slow in  the beginning
                float interpolater = currTransitionTime / transitionTime;
                interpolater = 1 - Mathf.Cos(interpolater * Mathf.PI * 0.5f);

                currCustomer.transform.position = Vector3.Lerp(middlePos.position, endPos.position, interpolater);
                currTransitionTime += Time.deltaTime;
            }           
        }
    }

    public GameObject CreateNewCustomer(int customer_number)
    {
        //member that generates a customer

        GameObject newCustomer = Instantiate(customerPrefab, startPos);
        currCustomer = newCustomer;

        movingIn = true;
        currTransitionTime = 0;
        return newCustomer;
    }

    //Member used to remove the current customer
    public void RemoveCurrentCustomer()
    {
        //If there is a current customer, then starts its transition out of frame
        if (currCustomer != null)
        {
            movingOut = true;
            currTransitionTime = 0;
        }
    }

    // spawn a bunch of customers at once to debug order generation
    public void DebugSpawnCustomers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newCustomer = CreateNewCustomer(i);
            newCustomer.transform.position += new Vector3(i, 0, 0);
        }
    }

}
