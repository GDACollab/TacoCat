using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public TacoMakingGameManager tacoGameManager;

    [Header("Inputs")]
    public KeyCode bin1 = KeyCode.A;
    public KeyCode bin2 = KeyCode.S;
    public KeyCode bin3 = KeyCode.D;
    public KeyCode bin4 = KeyCode.F;
    public KeyCode bin5 = KeyCode.G;
    public KeyCode submit = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(bin1))
        {
            PickFromBin(0);
        }
        else if (Input.GetKeyDown(bin2))
        {     
            PickFromBin(1);
        }
        else if (Input.GetKeyDown(bin3))
        {
            PickFromBin(2);
        }
        else if (Input.GetKeyDown(bin4))
        {
            PickFromBin(3);
        }
        else if (Input.GetKeyDown(bin5))
        {
            PickFromBin(4);
        }
        else if (Input.GetKeyDown(submit))
        {
            tacoGameManager.SubmitTaco();
        }
    }


    public void PickFromBin(int bin_index)
    {
        //Sends message to playerHand to pickup/place ingredient, sends the location info (bin+taco) it needs as well
        tacoGameManager.hand.PickUpIngredient(tacoGameManager.GetIngredientBin(bin_index), tacoGameManager.submissionTaco);

        // Following function is now performed in PlayerHand.cs upon hand reaching taco
        //tacoGameManager.AddIngredientToTaco(tacoGameManager.GetIngredientBin(bin_index).ingredientType);


    }
}
