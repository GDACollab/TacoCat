using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taco
{
    public enum shellType { FLOUR, CORN, HARD_SHELL }
    public enum fillingType { BEEF, CHICKEN, BEANS, NONE }
    public enum toppingType { SAUCE_CHEESE, SHREDDED_CHEESE, SOUR_CREAM, NONE }
    // Note on NONE topping type: If making a random taco recipe,
    // if NONE is rolled, stop checking for toppings and don't add amymore toppings

    private shellType s_shell;
    private List<fillingType> s_filling;
    private List<toppingType> s_toppings;

    public Taco(shellType shell, List<fillingType> fillings, List<toppingType> toppings)
    {
        s_shell = shell;
        s_filling = fillings;
        s_toppings = toppings;
    }

    public void AddShell(shellType shell)
    {
        // Maybe don't replace the shell if there are already ingredients
        s_shell = shell;
    }

    public void AddFilling(fillingType filling)
    {
        s_filling.Add(filling);
    }

    public void AddTopping(toppingType topping)
    {
        s_toppings.Add(topping);
    }

    /// <summary>
    /// Returns a value between 0 and 1 based on how many ingredients match
    /// </summary>
    /// <param name="other"></param>
    /// <returns>A value between 0 and 1, the percentage of taco ingredient match</returns>
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