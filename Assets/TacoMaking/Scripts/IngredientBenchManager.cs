using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBenchManager : MonoBehaviour
{
    TacoMakingGameManager gameManager;

    [Tooltip("List of references to each ingredient bin")]
    public List<IngredientBin> ingredientBins;

    [Tooltip("Current menu of ingredients to choose from")]
    public List<INGREDIENT_TYPE> menu;

    [Header("Taco")]
    public Transform tacoSpawnPoint;

    public float ingredientScale = 0.1f;

    [Header("Animator")]
    public Animator anim;

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
        foreach(INGREDIENT_TYPE ingr in menu)
        {
            // get matching object in gamemanager
            GameObject ingr_obj = gameManager.GetIngredientBinSprite(ingr);
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
                ingredientBins[i].SetIngredientBin(ingrObjs[i]);
            }
        }

    }

    public void PlayIngredientAnim(INGREDIENT_TYPE ingredient)
    {
        switch(ingredient)
        {
            case INGREDIENT_TYPE.CABBAGE:
                anim.Play("CabbageShuffle");
                break;
            case INGREDIENT_TYPE.FISH:
                anim.Play("FishShuffle");
                break;
            case INGREDIENT_TYPE.PICO_DE_GALLO:
                anim.Play("PicoShuffle");
                break;
            case INGREDIENT_TYPE.SLICED_JALAPENOS:
                anim.Play("JalShuffle");
                break;
            case INGREDIENT_TYPE.SOUR_CREAM:
                anim.Play("SourCreamShuffle");
                break;
            default:
                break;
        }
    }

}
