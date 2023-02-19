using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomerManager : MonoBehaviour
{
    [HideInInspector]
    public TacoMakingGameManager tacoGameManager;
    public GameObject customerPrefab;
    public List<GameObject> currentCustomers;


    //before calling check if customers left to generate == 0
    private void Start()
    {
        tacoGameManager = GetComponentInParent<TacoMakingGameManager>();

        //DebugSpawnCustomers(10);
    }

    public GameObject CreateNewCustomer(int customer_number)
    {
        //member that generates a customer

        GameObject newCustomer = Instantiate(customerPrefab, transform);

        currentCustomers.Add(newCustomer);

        return newCustomer;
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
