using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBin : MonoBehaviour
{
    public GameObject currIngredient;

    public void SetIngredient(GameObject ingr)
    {
        // instantiate version of object
        currIngredient = Instantiate(ingr, transform.position, Quaternion.identity);
        currIngredient.transform.parent = transform;
    }
}
