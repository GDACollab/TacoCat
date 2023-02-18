using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ingredientType = CustomerManager.ingredientType;


public class IngredientBenchManager : MonoBehaviour
{
    TacoMakingGameManager gameManager;

    [Tooltip("List of references to each ingredient bin")]
    public List<IngredientBin> ingredientBins;

    [Tooltip("Current menu of ingredients to choose from")]
    public List<ingredientType> menu;

    [Header("Taco")]
    public Transform tacoSpawnPoint;

    public float ingredientScale = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponentInParent<TacoMakingGameManager>();

        // go through menu and spawn ingredients in bin
        SetBinIngredients();
    }

    // << PLACE INGREDIENTS IN BINS >>
    public void SetBinIngredients()
    {
        // << MATCH PREFABS AND ENUMS >>
        // first get all objects that == ingredientType in menu
        List<GameObject> ingrObjs = new List<GameObject>(); // store prefabs needed
        foreach(ingredientType ingr in menu)
        {
            // get matching object in gamemanager
            GameObject ingr_obj = gameManager.GetIngredientObject(ingr);
            ingr_obj.transform.localScale = Vector3.one * ingredientScale;


            ingrObjs.Add(ingr_obj);
        }

        // << INSTANTIATE PREFABS IN BIN >>
        // for each bin, set ingredient and spawn
        for (int i = 0; i < ingredientBins.Count; i++)
        {
            // check if ingr objs exist
            if (i < ingrObjs.Count)
            {
                // set ingredient
                ingredientBins[i].SetIngredient(ingrObjs[i]);
            }
        }

    }

}
