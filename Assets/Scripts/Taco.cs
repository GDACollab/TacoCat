using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum SCORE_TYPE { PERFECT, GOOD, OKAY, FAILED }
public enum INGREDIENT_TYPE { NONE, FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS, LETTUCE, TOMATO }
public enum CUSTOMER_TYPE { CAT, DOG, FISH }

public class Taco : MonoBehaviour
{
    // max amount of ingredients
    public int maxIngredients = 3;

    // list of curr ingredients
    public List<INGREDIENT_TYPE> currIngredients;

    // list of gameobject ingredients
    public List<GameObject> ingredientObjects = new List<GameObject>();



    


}


