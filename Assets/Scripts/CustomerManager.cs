using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomerManager : MonoBehaviour
{
    public enum ingredientType {FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS}
    public IngredientList s_menu;//what items are on the menu
    public int s_perfectCounter;
    public int s_comboCounter;    
    public int customersLeftToGenerate;
    //private Customer Customer; //just some unity stuff testing ignore.
    //public GameObject prefab; 
   
    // Start is called before the first frame update 

    //before calling check if customers left to generate == 0

    void customerUpdate(){
        //is called once a taco is turned in (collision in a different game object/button key press) or when the previous taco is done being graded
        //checks if a new customer needs to be generated
        //runs all things that come along with a new customer (i.e. taco order generation, moving the customer etc.)
        customerGenerator();//generates the customer prefab??
        Customer.s_order=orderGenerator();//generates the customer's order
        //^gets the customer just generated and generates their order (the line of code above is probably wrong)                           
    }
    void customerGenerator(){//member that generates a customer
    }
    public IngredientList orderGenerator(){ //member that generates a list of the ingredientType and sends it to Customer
        IngredientList order = new IngredientList();
        return order;
    }
    void Start()
    {
        //Customer=GetComponentInChildren<Customer>();
        //GameObject newCustomer= Instantiate(prefab,this.transform);
        //Customer.tacoGrading();
        //call generateACustomer or function that calls that
    }

    // Update is called once per frame
    void Update()
    {
    }
}
