using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomerManager : MonoBehaviour
{
    [HideInInspector]
    public TacoMakingGameManager tacoGameManager;
    public GameObject customerPrefab;
    public Customer currCustomer;
  
    public float transitionTime;       //How long it takes in seconds for the customer to transition between positions
    public List<Transform> positionList = new List<Transform>(); //Used as the points the customer transitions to/from 
    public List<Customer> customerList = new List<Customer>();
    [HideInInspector] public int difficulty = 1;


    //before calling check if customers left to generate == 0
    private void Start()
    {
        //DebugSpawnCustomers(10);
    }

    private void FixedUpdate()
    {
        //Updates the customer positions when the current customer gets removed
        if (currCustomer == null && customerList.Count > 0)
        {
            UpdateCustomers();
        }
    }

    public GameObject CreateNewCustomer(int customer_number)
    {
        Debug.Log("Created Customer");

       //Creates a new customer and sets all of its vars
        Customer customerScript  = Instantiate(customerPrefab, transform).GetComponent<Customer>();
        customerScript.transform.position = positionList[5].position;
        customerScript.transitionTime = transitionTime;
        customerScript.currPosition = -1;
        customerScript.difficulty = difficulty;
        customerList.Add(customerScript);
        UpdateCustomers();

        return currCustomer.gameObject;
    }

    //Member used to remove the current customer
    public void RemoveCurrentCustomer()
    {
        //If there is a current customer, then starts its transition out of frame
        if (currCustomer != null)
        {
            //Delays the destruction of the customer so that they have time to move offscreen
            UpdateCustomerPos(currCustomer, positionList[0].position);
            Destroy(currCustomer.gameObject, transitionTime);
            customerList.RemoveAt(0);          
            currCustomer = null;
        }
    }

    //Member to update the positions of all the customers in line
    private void UpdateCustomers()
    {
        //Assign the new current customer when the current one gets removed
        if (currCustomer == null && customerList.Count > 0)
        {
            currCustomer = customerList[0];

            //tacoAudioManager.OrderAudio(); //needs to be edited later
        }

        //Iterate through all the customers and check if their position needs to be updated
        for (int i = 0; i < customerList.Count; i++)
        {
            //If the customers current position has changed, then update its variables
            if (customerList[i].currPosition != i)
            {
                customerList[i].currPosition = i;
                UpdateCustomerPos(customerList[i], positionList[i + 1].position);
            }
        }
    }

    //Helper function for setting the vars for transitioning to a new spot in line
    private void UpdateCustomerPos(Customer customer, Vector3 newPosition)
    {
        customer.prevPos = customer.transform.position;
        customer.targetPos = newPosition;
        customer.interpolater = 0;
        customer.currTransitionTime = 0;
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
