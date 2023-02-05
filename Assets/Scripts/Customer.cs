using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ingredientType=CustomerManager.ingredientType;

using scoreType=Taco.scoreType;
public class Customer: MonoBehaviour
{   

    static public IngredientList s_order; //ingredients in the order

    public scoreType tacoGrading(Taco currTaco){
        //compares the list of ingredients in the taco submitted and the list of ingredients in the customer's order.
        if(currTaco.s_ingredients.Count==s_order.Count){ //if taco lengths are equal
            if((currTaco.s_ingredients & s_order) == currTaco.s_ingredients.Count){ //if # of matching in order ingredients == length of either list
                currTaco.s_score=scoreType.PERFECT; //perfect score 
                CustomerManager.s_perfectCounter++; //increment perfect counter
                if (CustomerManager.s_perfectCounter % 3 ==0) CustomerManager.s_comboCounter++; //display combo stuff
                goto exit;
            }
        }
        IngredientList currTacoSorted= new IngredientList(currTaco.s_ingredients);
        IngredientList s_orderSorted= new IngredientList(s_order);
        currTacoSorted.Sort();
        s_orderSorted.Sort();
        int numMatching=currTacoSorted & s_orderSorted; //number of matching ingredients (ignoring order)
        //this will need to change depending on how we see duplicates
        int longerLength= (currTaco.s_ingredients.Count>s_order.Count)? currTaco.s_ingredients.Count: s_order.Count;
        //definitely a better way to do this next section lmao
        if(numMatching-longerLength==0){
            currTaco.s_score=scoreType.GOOD;
        }
        else if(numMatching-longerLength==1){
            currTaco.s_score=scoreType.OKAY;
        }
        else if(numMatching-longerLength>=2){
            currTaco.s_score=scoreType.FAILED;
        }
        CustomerManager.s_perfectCounter=0;
        exit:{}
        return currTaco.s_score;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
