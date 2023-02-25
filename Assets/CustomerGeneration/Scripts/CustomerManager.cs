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

    public Transform startPos, thirdLinePos, secondLinePos, firstLinePos, orderingPos, endPos; //Used as the points the customer transitions to/from
    [HideInInspector] public bool movingIn, movingOut; //Used for keeping track of if the current customer is transitioning in or out
    public float transitionTime;       //How long it takes in seconds for the customer to transition in/out of frame
    private float currTransitionTime;  //Used for keeping track of time during transitions
    public List<GameObject> customerList = new List<GameObject>();


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
        UpdateCustomers();
        /*

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

                currCustomer.transform.position = Vector3.Lerp(startPos.position, orderingPos.position, interpolater);
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

                currCustomer.transform.position = Vector3.Lerp(orderingPos.position, endPos.position, interpolater);
                currTransitionTime += Time.deltaTime;
            }           
        }
        */

    }

    public GameObject CreateNewCustomer(int customer_number)
    {
        //member that generates a customer

        GameObject newCustomer = Instantiate(customerPrefab, transform);
        newCustomer.transform.position = startPos.position;
        customerList.Add(newCustomer);
        UpdateCustomers();

        return currCustomer;
    }

    //Member used to remove the current customer
    public void RemoveCurrentCustomer()
    {
        //If there is a current customer, then starts its transition out of frame
        if (currCustomer != null)
        {
            movingOut = true;
            currTransitionTime = 0;
            Destroy(currCustomer);
        }
    }

    //Member to update the positions of all the customers in line
    private void UpdateCustomers()
    {
        //If there is no current customer, moves the customer first in line into currCustomer
        if (currCustomer == null && customerList.Count > 0)
        {
            currCustomer = customerList[0];
            customerList.RemoveAt(0);
            currTransitionTime = 0;
            movingIn = true;
        }
        customerList.TrimExcess();

        if (customerList.Count == 3)
        {
            customerList[2].transform.position = thirdLinePos.position;
        }
        if (customerList.Count == 2)
        {
            customerList[1].transform.position = secondLinePos.position;
        }
        if (customerList.Count == 1)
        {
            customerList[0].transform.position = firstLinePos.position;
        }
        if (currCustomer != null)
        {
            currCustomer.transform.position = orderingPos.position;
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
