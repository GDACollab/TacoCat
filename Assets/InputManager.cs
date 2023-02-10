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
        //int bin = 0;
        if (Input.GetKeyDown(KeyCode.A))
        {
            PickFromBin(0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {     
            PickFromBin(1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PickFromBin(2);
        }
    }


    public void PickFromBin(int bin_index)
    {
        tacoGameManager.AddIngredientToTaco(tacoGameManager.GetIngredientBin(bin_index).ingredientType);
    }
}
