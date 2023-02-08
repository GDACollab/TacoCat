using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ingredientType=CustomerManager.ingredientType;

using scoreType=Taco.scoreType;
public class Customer: MonoBehaviour
{   
    static public List<ingredientType> s_order; //ingredients in the order

    public scoreType tacoGrading(Taco currTaco){
        //compares the list of ingredients in the taco submitted and the list of ingredients in the customer's order.
        if(currTaco.s_ingredients.Count==s_order.Count){ //if taco lengths are equal
            if(Taco.ingredientCompare(currTaco.s_ingredients, s_order) == currTaco.s_ingredients.Count){ //if # of matching in order ingredients == length of either list
                _CustomerManager.s_perfectCounter++; //increment perfect counter
                if (_CustomerManager.s_perfectCounter % 3 ==0) _CustomerManager.s_comboCounter++; //display combo stuff //maybe move elsewhere
                return scoreType.PERFECT; //perfect score 
            }
        }
        List<ingredientType> currTacoSorted= new List<ingredientType> (currTaco.s_ingredients);
        List<ingredientType> s_orderSorted= new List<ingredientType>(s_order);
        currTacoSorted.Sort();
        s_orderSorted.Sort();
        int numMatching=Taco.ingredientCompare(currTacoSorted, s_orderSorted); //number of matching ingredients (ignoring order)
        //this will need to change depending on how we see duplicates
        int longerLength= (currTaco.s_ingredients.Count>s_order.Count)? currTaco.s_ingredients.Count: s_order.Count;
        //definitely a better way to do this next section lmao
        _CustomerManager.s_perfectCounter=0;
        if(numMatching-longerLength==0){
            return scoreType.GOOD;
        }
        else if(numMatching-longerLength==1){
            return scoreType.OKAY;
        }
        else return scoreType.FAILED;
    }

    // Start is called before the first frame update
    private CustomerManager _CustomerManager;
    void Start()
    {
        _CustomerManager=GetComponentInParent<CustomerManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
