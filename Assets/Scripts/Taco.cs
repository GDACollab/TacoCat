using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taco
{
    public enum shellType { FLOUR, CORN, HARD_SHELL }
    public enum fillingType { BEEF, CHICKEN, BEANS }
    public enum toppingType { SAUCE_CHEESE, SHREDDED_CHEESE, SOUR_CREAM, NONE }

    private shellType s_shell;
    private List<fillingType> s_filling;
    private List<toppingType> s_toppings;

    Taco(shellType shell, List<fillingType> fillings, List<toppingType> toppings)
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
        if (shell == other.shell)
        {
            matchingIngredients++;
        }
        totalIngredients++;

        int shorterFilling;
        int longerFilling;
        if (other.filling.Count < filling.Count)
        {
            shorterFilling = other.filling.Count;
            longerFilling = filling.Count;
        }
        else
        {
            shorterFilling = filling.Count;
            longerFilling = other.filling.Count;
        }

        for (int i = 0; i < shorterFilling; i++)
        {
            if (filling[i] == other.filling[i])
            {
                matchingIngredients++;
            }
            totalIngredients++;
        }
        // Catch any ingredient mismatch if one list is shorter than the other
        totalIngredients += longerFilling - shorterFilling;

        int shorterTopping;
        int longerTopping;
        if (other.toppings.Count < toppings.Count)
        {
            shorterTopping = other.toppings.Count;
            longerTopping = toppings.Count;
        }
        else
        {
            shorterTopping = toppings.Count;
            longerTopping = other.toppings.Count;
        }

        for (int i = 0; i < shorterTopping; i++)
        {
            if (toppings[i] == other.toppings[i])
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