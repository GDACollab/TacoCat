using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public GameObject tacoPrefab; // clone of taco to use for later
    public GameObject currTaco; // curr gameobject 

    public Transform tacoHolder; // hand position

    public List<GameObject> ingredientsPrefabs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        currTaco = CreateRandomTaco();
        currTaco.transform.parent = tacoHolder;
    }

    public GameObject CreateRandomTaco()
    {
        // create taco
        GameObject newTaco = Instantiate(tacoPrefab);
        Taco tacoScript = newTaco.GetComponent<Taco>();

        // set taco position
        newTaco.transform.position = tacoHolder.position;

        // create list of ingredients (( randomize here ))
        List<ingredientType> newIngredients = new List<ingredientType>();
        newIngredients.Add(ingredientType.LETTUCE);
        newIngredients.Add(ingredientType.TOMATO);
        newIngredients.Add(ingredientType.LETTUCE);

        // set ingredients
        tacoScript.s_ingredients = newIngredients;

        // spawn all ingredients
        foreach (ingredientType ingr in tacoScript.s_ingredients)
        {
            GameObject newIngrObj = Instantiate(GetTacoIngredientObject(ingr), tacoHolder.position, Quaternion.identity);
            newIngrObj.transform.parent = newTaco.transform;
            tacoScript.currIngredients.Add(newIngrObj);
        }

        return newTaco;
    }

    public GameObject GetTacoIngredientObject(ingredientType ingredient)
    {
        if (ingredient == ingredientType.LETTUCE)
        {
            return ingredientsPrefabs[0];
        }
        else if (ingredient == ingredientType.TOMATO)
        {
            return ingredientsPrefabs[1];
        }

        return null;
    }

}
