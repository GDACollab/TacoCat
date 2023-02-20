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

    public Transform startPos, middlePos, endPos;
    public bool movingIn, movingOut;
    private float interpolater;
    public float transitionSpeed;


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
            if (interpolater >= 1)
            {
                movingIn = false;
            }
            else
            {
                currCustomer.transform.position = Vector3.Lerp(startPos.position, middlePos.position, interpolater);
                interpolater += transitionSpeed / 100;
            }           
        }

        //Moves the customer out of frame and then destroys them
        if (movingOut)
        {
            if (interpolater >= 1)
            {                
                movingOut = false;
                Destroy(currCustomer);
            }
            else
            {
                currCustomer.transform.position = Vector3.Lerp(middlePos.position, endPos.position, interpolater);
                interpolater += transitionSpeed / 100;
            }           
        }
    }

    public GameObject CreateNewCustomer(int customer_number)
    {
        //member that generates a customer

        GameObject newCustomer = Instantiate(customerPrefab, startPos);

        currCustomer = newCustomer;
        movingIn = true;
        interpolater = 0;
        return newCustomer;
    }

    public void RemoveCurrentCustomer()
    {
        //If there is a current customer, then starts its transition out of frame
        if (currCustomer != null)
        {
            movingOut = true;
            interpolater = 0;
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
