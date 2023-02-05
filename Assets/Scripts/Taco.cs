using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class IngredientList: List<CustomerManager.ingredientType>{
        public IngredientList(){
            
        }
        public IngredientList(IngredientList clone){
            for(int i=0;i<clone.Count;i++){
                this.Add(clone[i]);
            }
        }
        public static int operator&(IngredientList a, IngredientList b){
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

    }
public class Taco : CustomerManager
{
    public enum scoreType{PERFECT, GOOD, OKAY, FAILED}
    
    //public enum customerType{CAT, DOG, FISH} 
    public IngredientList s_ingredients;
    public scoreType s_score;
    //private customerType s_customer;


    public Taco(IngredientList ingredients)
    {
        s_ingredients = ingredients;
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
    

    
    /*
    
    
    */
}

