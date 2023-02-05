using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taco : MonoBehaviour
{
    public enum scoreType{PERFECT, GOOD, OKAY, FAILED}
    public enum ingredientType {FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS}
    //public enum customerType{CAT, DOG, FISH} 
    private List<ingredientType> s_ingredients;
    private scoreType s_score;
    //private customerType s_customer;
    

    public Taco(List<ingredientType> ingredients)
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
    public class IngredientList: List<ingredientType>{
        public static IngredientList operator&(IngredientList a, IngredientList b){
            int ingredientMatch=0;
            int smallerLength=0;
            smallerLength=(a.length()>b.length())? b.length() : a.length();
            //^setting the # of times we'll iterate through the list to be the length of the shorter list
            for(unsigned i=0; i<smallerLength();i++){ //iterate through the smaller list
                if(a[i]==b[i]){
                    ingredientMatch++;
                }
            }
            return ingredientMatch;
        }

    }
    public scoreType tacoGrading(Taco orderTaco, Taco currTaco){
        if(currTaco.length()==orderTaco.length()){ //if taco lengths are equal
            if(currTaco & orderTaco == currTaco.length()){ //if # of matching in order ingredients == length of either list
                currTaco.s_score=PERFECT; //perfect score 
                perfectCounter++; //increment perfect counter
                if (perfectCounter % 3) comboCounter++; //display combo stuff
                goto exit;
            }
        }
        int numMatching=currTaco.sort() & orderTaco.sort(); //number of matching ingredients (ignoring order)
        //this will need to change depending on how we see duplicates
        int longerLength= (currTaco.length()>orderTaco.length())? currTaco.length(): orderTaco.length();
        //definitely a better way to do this next section lmao
        if(numMatching-longerLength==0){
            currTaco.s_score=GOOD;
        }
        else if(numMatching-longerLength==1){
            currTaco.s_score=OKAY;
        }
        else if(numMatching-longerLength>=2){
            currTaco.s_score=FAILED;
        }
        perfectCounter=0;
        exit:{}
    }

    
    /*
    
    
    */
}
