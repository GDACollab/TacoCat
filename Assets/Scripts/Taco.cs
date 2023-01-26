using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taco
{
    public enum ingredientType {FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS}
    private List<ingredientType> s_ingredients;


    public Taco(List<ingredientType> ingredients)
    {
        s_ingredients = ingredients;
    }

    /// <summary>
    /// Returns a value between 0 and 1 based on how many ingredients match
    /// </summary>
    /// <param name="other"></param>
    /// <returns>A value between 0 and 1, the percentage of taco ingredient match</returns>

    //CHECKING TACO PSUEDO CODE
    /*
        for e/a taco
        go through list subtracting from example taco
        if curr ingredient doesn't match example ingredient 
            if topping matches customer type then add dialogue or something
            denote to "Good Taco" (reset perfect counter)
            check to make sure ingredients match even if order is wrong
        if ingredient is in taco, that doesn't match example taco, 
            denote to "Okay Taco" 
            add one to ingredientMisMatchCounter
        if ingredientMisMatchCounter gets increases to 2, denote to "Failed Taco" STOP CHECKING
        if done iterating through taco, check if exampleTaco has any elements, if it has 1 extra,
            denote taco to "Okay Taco"
        if more extra
            denote taco to "Failed Taco"
    */
    public float CheckTaco(Taco other)
    {
        int totalIngredients = 0;
        int matchingIngredients = 0;
        if (s_shell == other.s_shell)
        {
            matchingIngredients++;
        }
        totalIngredients++;

        int shorterFilling;
        int longerFilling;
        if (other.s_filling.Count < s_filling.Count)
        {
            shorterFilling = other.s_filling.Count;
            longerFilling = s_filling.Count;
        }
        else
        {
            shorterFilling = s_filling.Count;
            longerFilling = other.s_filling.Count;
        }

        for (int i = 0; i < shorterFilling; i++)
        {
            if (s_filling[i] == other.s_filling[i])
            {
                matchingIngredients++;
            }
            totalIngredients++;
        }
        // Catch any ingredient mismatch if one list is shorter than the other
        totalIngredients += longerFilling - shorterFilling;

        int shorterTopping;
        int longerTopping;
        if (other.s_toppings.Count < s_toppings.Count)
        {
            shorterTopping = other.s_toppings.Count;
            longerTopping = s_toppings.Count;
        }
        else
        {
            shorterTopping = s_toppings.Count;
            longerTopping = other.s_toppings.Count;
        }

        for (int i = 0; i < shorterTopping; i++)
        {
            if (s_toppings[i] == other.s_toppings[i])
            {
                matchingIngredients++;
            }
            totalIngredients++;
        }
        // Catch any ingredient mismatch if one list is shorter than the other
        totalIngredients += longerTopping - shorterTopping;

        return (float) matchingIngredients / (float) totalIngredients;
    }
}