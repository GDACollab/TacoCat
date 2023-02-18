using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taco : MonoBehaviour
{
    public List<ingredientType> s_ingredients; //ingredients currently on the taco
    public void addIngredient(ingredientType newIngredient){ 
        //called when key has been inputed for ingredient in taco making
        s_ingredients.Add(newIngredient);
    }
    public void addingredientObject(GameObject obj)
    {
        GameObject ingr = Instantiate(obj, transform);
    }

    public static int ingredientCompare(List<ingredientType> a, List<ingredientType> b){ //returns the number of matching ingredients in a row from the [0]th element
        int ingredientMatch=0;
        int smallerLength=0;
        smallerLength=(a.Count>b.Count)? b.Count : a.Count;
        //^setting the # of times we'll iterate through the list to be the length of the shorter list
        for(int i=0; i<smallerLength;i++){ //iterate through the smaller list
            if(a[i]==b[i]){
                ingredientMatch++;
            }
            else{
                break;
            }
        }
        return ingredientMatch;
    }

}

