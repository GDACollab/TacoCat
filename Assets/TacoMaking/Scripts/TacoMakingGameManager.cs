using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ingredientType { FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS }
public enum scoreType { PERFECT, GOOD, OKAY, FAILED } // possible scores a taco can get.

public class TacoMakingGameManager : MonoBehaviour
{
    [HideInInspector]
    public IngredientBenchManager benchManager;
    
    [Header("Prefabs")]
    public GameObject tacoPrefab;
    public List<GameObject> allIngredientPrefabs;

    [Header("Submission Taco")]
    public Taco submissionTaco;

    [Header("Hand")]
    public PlayerHand hand;

    [Header("Customers")]
    public Customer currCustomer;
    public CustomerManager customerManager;
    public int perfectCounter; //counts the number of perfect tacos in a row, resets when a !perfect taco is submitted
    public int comboCounter;   //counts the number for 3 combos in total throughout the whole minigame
    public int customersLeftToGenerate; //the number of customers left to generate in the scene

    [Header("Score")]
    public int gameScore = 0; // max score is 3 * numOfCustomers served

    public void Start()
    {
        benchManager = GetComponentInChildren<IngredientBenchManager>();

        // create init submission taco
        GameObject taco = Instantiate(tacoPrefab, GetComponentInChildren<IngredientBenchManager>().tacoSpawnPoint.position, Quaternion.identity);
        submissionTaco = taco.GetComponent<Taco>();
        taco.transform.parent = transform;
    }

    public void Update()
    {
        CustomerRotation();
    }

    // continue through remaining customers
    public void CustomerRotation()
    {
        // if curr customer is null, and more customers remaining
            // create a new cutsomer from customer manager
            // store new customer in curr cutsomer
        
        //Creates and stores a new customer whenever the current one is deleted
        if (customersLeftToGenerate > 0 && currCustomer == null)
        {
            currCustomer = customerManager.CreateNewCustomer(0).GetComponent<Customer>();
            customersLeftToGenerate--;
        }    
    }

    // submit taco to customer to be graded
    public void SubmitTaco(Taco submissionTaco, Customer currentCustomer)
    {
        // send the submission taco to current customer to be graded

        // print to console the result of the taco

        // delete the customer game object

        // delete the submission taco gameobject
    }


    // update game score variable
    public void NewTacoScore(scoreType score)
    {
        // if score == PERFECT -> gameScore += 3

             // << UPDATE PERFECT SCORE COUNTER >>
             // perfectCounter++; //increment perfect counter
             // if (perfectCounter % 3 == 0) comboCounter++; //display combo stuff //maybe move elsewhere

        // if score == GOOD -> gameScore += 2

        // if

        // if

        // read the Taco Making Design Doc for the rest of the score amounts
    }

    #region HELPER FUNCTIONS ==============================================================

    // << GET CURRENT INGREDIENT IN BIN >>
    public IngredientBin GetIngredientBin(int index)
    {
        return benchManager.ingredientBins[index];
    }

    // << ADD INGREDIENT TO SUBMISSION TACO >>
    public void AddIngredientToTaco(ingredientType type)
    {
        submissionTaco.addIngredient(type); 
        submissionTaco.addingredientObject(GetIngredientObject(type));
    }


    // return prefab that is related to the input enum type
    public GameObject GetIngredientObject(ingredientType ingrType)
    {
        // for each object in prefab list
        foreach(GameObject obj in allIngredientPrefabs)
        {
            // look at the ingredient script inside prefab
            if (obj.GetComponent<Ingredient>().type == ingrType)
            {
                return obj;
            }
        }

        Debug.Log("Could not find " + ingrType + " prefab");

        return null;
    }

    #endregion
}
