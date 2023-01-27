using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taco
{
    public enum scoreType{PERFECT, GOOD, OKAY, FAILED}
    public enum ingredientType {FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS}
    public enum customerType{CAT, DOG, FISH}
    private List<ingredientType> s_ingredients;
    private scoreType s_score;
    private customerType s_customer;
    

    public Taco(List<ingredientType> ingredients, customerType customer)
    {
        s_ingredients = ingredients;
        s_customer= customer;
        s_score= scoreType.PERFECT;
    }
    public void setScore(scoreType incomingScore){
        s_score=incomingScore;
    }
    /// <summary>
    /// Returns a value between 0 and 1 based on how many ingredients match
    /// </summary>
    /// <param name="currTaco"></param>
    /// <returns>A value between 0 and 1, the percentage of taco ingredient match</returns>

    //CHECKING TACO PSUEDO CODE
    /*
        for e/a taco
        if customer is a fish, if taco contains fish
            denote to "Failed Taco" cool dialogue,
        PERFECT TACO:
            go through lists subtracting from taco and example taco
            if curr ingredient doesn't match example ingredient
                denote to "Good Taco"
        GOOD TACO:
            check to make sure ingredients match even if order is wrong subtract from lists
            if ingredient is in taco, that doesn't match example taco, subtract from taco list
                add one to ingredientMisMatchCounter
        WHEN done iterating through taco, 
            check if exampleTaco has any elements, add # of extra elements to ingredientMisMatchCounter
        if ingredientMisMatchCounter == 1 -> "Okay Taco"
        if ingredientMisMatchCounter > 1 -> "Failed Taco"

        if taco isn't perfect, reset perfectCounter
        else increment perfectCounter
        is perfectCounter % 3 display combo message, increment comboCounter.
    */
    public float CheckTaco(Taco currTaco)
    {
        int ingredientMisMatchCounter = 0; // when == 1 -> "Okay Taco", when >=2 -> "Failed Taco"
        if(s_customer=FISH){
            currTaco.setScore(FAILED);
            //currTaco.angeredFish = true; ???
        }else{
            
        }
        totalIngredients++;

        int shorterFilling;
        int longerFilling;
        if (currTaco.s_filling.Count < s_filling.Count)
        {
            shorterFilling = currTaco.s_filling.Count;
            longerFilling = s_filling.Count;
        }
        else
        {
            shorterFilling = s_filling.Count;
            longerFilling = currTaco.s_filling.Count;
        }

        for (int i = 0; i < shorterFilling; i++)
        {
            if (s_filling[i] == currTaco.s_filling[i])
            {
                matchingIngredients++;
            }
            totalIngredients++;
        }
        // Catch any ingredient mismatch if one list is shorter than the currTaco
        totalIngredients += longerFilling - shorterFilling;

        int shorterTopping;
        int longerTopping;
        if (currTaco.s_toppings.Count < s_toppings.Count)
        {
            shorterTopping = currTaco.s_toppings.Count;
            longerTopping = s_toppings.Count;
        }
        else
        {
            shorterTopping = s_toppings.Count;
            longerTopping = currTaco.s_toppings.Count;
        }

        for (int i = 0; i < shorterTopping; i++)
        {
            if (s_toppings[i] == currTaco.s_toppings[i])
            {
                matchingIngredients++;
            }
            totalIngredients++;
        }
        // Catch any ingredient mismatch if one list is shorter than the currTaco
        totalIngredients += longerTopping - shorterTopping;

        return (float) matchingIngredients / (float) totalIngredients;
    }
}