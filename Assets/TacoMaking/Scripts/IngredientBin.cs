using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBin : MonoBehaviour
{
    // current ingredient object && type in bin
    public GameObject currIngredient;
    public CustomerManager.ingredientType ingredientType;

    // << SET INGREDIENT >>
    public void SetIngredient(GameObject ingr)
    {
        // instantiate version of object
        currIngredient = Instantiate(ingr, transform.position, Quaternion.identity);
        currIngredient.transform.parent = transform;

        ingredientType = currIngredient.GetComponent<Ingredient>().type;
    }
}