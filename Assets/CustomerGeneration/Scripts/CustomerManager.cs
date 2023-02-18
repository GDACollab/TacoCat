using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomerManager : MonoBehaviour
{
    public enum ingredientType {FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS}
    public enum scoreType{PERFECT, GOOD, OKAY, FAILED} // possible scores a taco can get.

    public int s_perfectCounter; //counts the number of perfect tacos in a row, resets when a !perfect taco is submitted
    public int s_comboCounter;   //counts the number for 3 combos in total throughout the whole minigame
    public int customersLeftToGenerate; //the number of customers left to generate in the scene

    [HideInInspector]
    public TacoMakingGameManager tacoGameManager;
    public int customerCount = 10;
    public GameObject customerPrefab;
    public List<GameObject> currentCustomers;


    //before calling check if customers left to generate == 0
    private void Start()
    {
        tacoGameManager = GetComponentInParent<TacoMakingGameManager>();

        for(int i = 0; i < customerCount; i++)
        {
            CreateNewCustomer(i);
        }
    }

    public GameObject CreateNewCustomer(int customer_number)
    {
        //member that generates a customer

        GameObject newCustomer = Instantiate(customerPrefab, transform);

        newCustomer.transform.position += new Vector3(customer_number, 0, 0);

        currentCustomers.Add(newCustomer);

        return newCustomer;
    }


    // Update is called once per frame
    void Update()
    {
    }
}
