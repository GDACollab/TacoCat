using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum scoreType { PERFECT, GOOD, OKAY, FAILED }
public enum ingredientType { FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS, LETTUCE, TOMATO }
public enum customerType { CAT, DOG, FISH }

public class Taco : MonoBehaviour
{
    // list of curr ingredients
    public List<ingredientType> s_ingredients;

    public List<GameObject> currIngredients = new List<GameObject>();
}


