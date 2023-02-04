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
        List<INGREDIENT_TYPE> newIngredients = new List<INGREDIENT_TYPE>();
        newIngredients.Add(INGREDIENT_TYPE.LETTUCE);
        newIngredients.Add(INGREDIENT_TYPE.TOMATO);
        newIngredients.Add(INGREDIENT_TYPE.LETTUCE);

        // set ingredients
        tacoScript.currIngredients = newIngredients;

        // spawn all ingredients
        foreach (INGREDIENT_TYPE ingr in tacoScript.currIngredients)
        {
            GameObject newIngrObj = Instantiate(GetTacoIngredientObject(ingr), tacoHolder.position, Quaternion.identity);
            newIngrObj.transform.parent = newTaco.transform;
            tacoScript.currIngredients.Add(newIngrObj);
        }

        return newTaco;
    }

    public GameObject GetTacoIngredientObject(INGREDIENT_TYPE ingredient)
    {
        if (ingredient == INGREDIENT_TYPE.LETTUCE)
        {
            return ingredientsPrefabs[0];
        }
        else if (ingredient == INGREDIENT_TYPE.TOMATO)
        {
            return ingredientsPrefabs[1];
        }

        return null;
    }

}
