using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public List<INGREDIENT_TYPE> menu;

    public GameObject customerPrefab;
    public List<GameObject> customerList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateNewCustomer();

        //newCustomerScript.CreateRandomTaco();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // << CREATE A NEW CUSTOMER >>
    public GameObject CreateNewCustomer()
    {
        // spawn new prefab 
        GameObject newCustomer = Instantiate(customerPrefab, Vector3.zero, Quaternion.identity);
        newCustomer.transform.parent = this.transform;

        Customer newCustomerScript = newCustomer.GetComponent<Customer>();

        customerList.Add(newCustomer);

        return newCustomer;
    } 
}
