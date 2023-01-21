using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taco
{
    enum shellType { FLOUR, CORN, HARD_SHELL }
    enum fillingType { BEEF, CHICKEN, BEANS }
    enum toppingType { SAUCE_CHEESE, SHREDDED_CHEESE, SOUR_CREAM, NONE }

    private shellType shell;
    private List<fillingType> filling;
    private List<toppingType> toppings;

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

        int shorterList = other.filling.Count < filling.Count ?  other.filling.Count : filling.Count;
        //for (int i = 0; i <   )
    }
}