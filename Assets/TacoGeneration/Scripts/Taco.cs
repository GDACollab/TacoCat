using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ingredientType=CustomerManager.ingredientType;
<<<<<<< Updated upstream

public class IngredientList: List<CustomerManager.ingredientType>{
=======
/*
public class IngredientList: List<ingredientType>{
>>>>>>> Stashed changes
    public IngredientList(){
        new List<CustomerManager.ingredientType>();
    }
    public IngredientList(IngredientList clone){
        for(int i=0;i<clone.Count;i++){
            this.Add(clone[i]);
        }
    }

}
<<<<<<< Updated upstream
    
public class Taco : CustomerManager
{
    public enum scoreType{PERFECT, GOOD, OKAY, FAILED}
    
    public IngredientList s_ingredients;

    public Taco(IngredientList ingredients)
    {
        s_ingredients = ingredients;
    }
=======
*/

public class Taco : MonoBehaviour
{
    public enum scoreType{PERFECT, GOOD, OKAY, FAILED}
    
    public List<ingredientType> s_ingredients;//= new IngredientList();
>>>>>>> Stashed changes
    public void addIngredient(ingredientType newIngredient){
        //called when key has been inputed for ingredient in taco making
        s_ingredients.Add(newIngredient);
    }
<<<<<<< Updated upstream
=======
    public void addingredientObject(GameObject obj)
    {
        GameObject ingr = Instantiate(obj, transform);
    }

    public static int ingredientCompare(List<ingredientType> a, List<ingredientType> b){
        int ingredientMatch=0;
        int smallerLength=0;
        smallerLength=(a.Count>b.Count)? b.Count : a.Count;
        //^setting the # of times we'll iterate through the list to be the length of the shorter list
        for(int i=0; i<smallerLength;i++){ //iterate through the smaller list
            if(a[i]==b[i]){
                ingredientMatch++;
            }
        }
        return ingredientMatch;
    }

>>>>>>> Stashed changes
}

