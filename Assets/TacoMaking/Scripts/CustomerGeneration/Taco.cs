using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taco : MonoBehaviour
{
    public Transform ingredientParent;
    public Animator anim;

    public List<INGREDIENT_TYPE> ingredients; //ingredients currently on the taco

    #region INGREDIENTS ==========================
    public void addIngredient(INGREDIENT_TYPE newIngredient){ 
        //called when key has been inputed for ingredient in taco making
        ingredients.Add(newIngredient);
    }

    public void addingredientObject(GameObject obj)
    {
        GameObject ingr = Instantiate(obj, transform.position + new Vector3(0, 0.2f * ingredients.Count + 0.1f, 0), Quaternion.identity, transform);
        ingr.transform.parent = ingredientParent;

        // ingredient sorting order =  count + taco current sorting order
        ingr.GetComponent<SpriteRenderer>().sortingOrder = ingredients.Count;

        // Check if the ingredient count number is even
        bool isEven = ingredients.Count % 2 == 0;

        // Flip the local scale x based on whether the input is even or odd
        Vector3 localScale = ingr.transform.localScale;
        localScale.x = isEven ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        ingr.transform.localScale = localScale;

    }

    public static int ingredientCompare(List<INGREDIENT_TYPE> a, List<INGREDIENT_TYPE> b){ //returns the number of matching ingredients in a row from the [0]th element
        
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
    #endregion


    #region ANIMATIONS ===========================

    public void PlayEnterAnim()
    {
        anim.Play("Enter");
    }

    public void PlayExitAnim()
    {
        anim.Play("Exit");
    }

    public void PlayPerfectAnim()
    {
        anim.Play("Perfect");
    }

    public void PlayComboAnim()
    {
        anim.Play("Combo");
    }


    #endregion


}

