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

    public Transform startPos, thirdLinePos, secondLinePos, firstLinePos, orderingPos, endPos; //Used as the points the customer transitions to/from
    public float transitionTime;       //How long it takes in seconds for the customer to transition in/out of frame
    public List<Customer> customerList = new List<Customer>();


    //before calling check if customers left to generate == 0
    private void Start()
    {
        tacoGameManager = GetComponentInParent<TacoMakingGameManager>();
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
       //Creates a new customer and sets all of its vars

        Customer customerScript   = Instantiate(customerPrefab, transform).GetComponent<Customer>();
        customerScript.prevPos    = startPos.position;
        customerScript.targetPos  = startPos.position;
        customerScript.transform.position = startPos.position;
        customerScript.transitionTime = transitionTime;
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
            UpdateCustomerPos(currCustomer, endPos.position);           
            Destroy(currCustomer, transitionTime);
            currCustomer = null;
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
        }

        //Update all the customers positions in the line
        if (customerList.Count >= 3)
        {
            UpdateCustomerPos(customerList[2], thirdLinePos.position);
        }
        if (customerList.Count >= 2)
        {
            UpdateCustomerPos(customerList[1], secondLinePos.position);
        }
        if (customerList.Count >= 1)
        {
            UpdateCustomerPos(customerList[0], firstLinePos.position);
        }
        if (currCustomer != null)
        {
            UpdateCustomerPos(currCustomer, orderingPos.position);
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
