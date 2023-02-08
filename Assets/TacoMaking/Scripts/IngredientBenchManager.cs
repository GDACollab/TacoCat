using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ingredientType = CustomerManager.ingredientType;


public class IngredientBenchManager : MonoBehaviour
{
    TacoMakingGameManager gameManager;

    public List<IngredientBin> ingredientBins; 

    public List<ingredientType> menu;

    [Header("Taco")]
    public Transform tacoSpawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponentInParent<TacoMakingGameManager>();

        // go through menu and spawn ingredients in bin
        SetBinIngredients();
    }

    public void SetBinIngredients()
    {
        // << MATCH PREFABS AND ENUMS >>
        // first get all objects that == ingredientType in menu
        List<GameObject> ingrObjs = new List<GameObject>(); // store prefabs needed
        foreach(ingredientType ingr in menu)
        {
            GameObject ingr_obj = gameManager.GetIngredientObject(ingr);
            ingrObjs.Add(ingr_obj);
        }

        // << INSTANTIATE PREFABS IN BIN >>
        // for each bin, set ingredient and spawn
        for (int i = 0; i < ingredientBins.Count; i++)
        {
            // check if ingr objs exist
            if (i < ingrObjs.Count)
            {
                ingredientBins[i].SetIngredient(ingrObjs[i]);
            }
        }

    }

}