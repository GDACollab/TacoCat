using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomerManager : MonoBehaviour
{
    public enum ingredientType {FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS}

    public enum scoreType{PERFECT, GOOD, OKAY, FAILED} // possible scores a taco can get.
    public List<ingredientType> s_menu;//what items are on the menu
    public int s_perfectCounter; //counts the number of perfect tacos in a row, resets when a !perfect taco is submitted
    public int s_comboCounter;   //counts the number for 3 combos in total throughout the whole minigame
    public int customersLeftToGenerate; //the number of customers left to generate in the scene

    //before calling check if customers left to generate == 0

    void customerUpdate(){
        //is called once a taco is turned in (collision in a different game object/button key press) or when the previous taco is done being graded
        //checks if a new customer needs to be generated
        //runs all things that come along with a new customer (i.e. taco order generation, moving the customer etc.)
        customerGenerator();//generates the customer prefab??
        //Customer.s_order=orderGenerator();//generates the customer's order
        //^gets the customer just generated and generates their order (the line of code above is probably wrong)                           
    }
    void customerGenerator(){//member that generates a customer
    }
    
    public List<ingredientType> orderGenerator(){
        List<ingredientType> order = new List<ingredientType>();
        //PUT ORDER GENERATION ALGORITHM HERE
        return order;
    }
    // Update is called once per frame
    void Update()
    {
    }
}
