using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ingredientType = CustomerManager.ingredientType;


public class TacoMakingGameManager : MonoBehaviour
{
    IngredientBenchManager benchManager;
    
    public GameObject tacoPrefab;
    public List<GameObject> allIngredientPrefabs;

    [Header("Submission Taco")]
    public Taco submissionTaco;

    public void Start()
    {
        benchManager = GetComponentInChildren<IngredientBenchManager>();

        GameObject taco = Instantiate(tacoPrefab, GetComponentInChildren<IngredientBenchManager>().tacoSpawnPoint.position, Quaternion.identity);
        submissionTaco = taco.GetComponent<Taco>();
        taco.transform.parent = transform;
    }

    public IngredientBin GetIngredientBin(int index)
    {
        return benchManager.ingredientBins[index];
    }

    public void AddIngredientToTaco(ingredientType type)
    {
        submissionTaco.addIngredient(type);
        submissionTaco.addingredientObject(GetIngredientObject(type));
    }


    #region HELPER FUNCTIONS ==============================================================
    // return prefab that is related to the input enum type
    public GameObject GetIngredientObject(CustomerManager.ingredientType ingrType)
    {
        // for each object in prefab list
        foreach(GameObject obj in allIngredientPrefabs)
        {
            // look at the ingredient script inside prefab
            if (obj.GetComponent<Ingredient>().type  == ingrType)
            {
                return obj;
            }
        }

        return null;
    }
    #endregion
}
