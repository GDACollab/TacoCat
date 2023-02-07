using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ingredientType = CustomerManager.ingredientType;


public class InputManager : MonoBehaviour
{
    public TacoMakingGameManager tacoGameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            tacoGameManager.AddIngredientToTaco(ingredientType.PICO_DE_GALLO);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            tacoGameManager.AddIngredientToTaco(ingredientType.SOUR_CREAM);
        }
    }
}
