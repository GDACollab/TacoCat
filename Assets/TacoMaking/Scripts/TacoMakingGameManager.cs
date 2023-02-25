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
    public int totalCustomers; //Total number of customers that will appear
    public int customersLeftToGenerate; //the number of customers left to generate in the scene
    public int lineSize;

    [Header("Score")]
    public int gameScore = 0; // max score is 3 * numOfCustomers served
    public int gasAmount = 0;
    public int nitroCount = 0;

    public void Start()
    {
        benchManager = GetComponentInChildren<IngredientBenchManager>();

        CreateNewSubmissionTaco();

        customersLeftToGenerate = totalCustomers;
    }

    public void Update()
    {
        CustomerRotation();     

    }

    public void CreateNewSubmissionTaco()
    {

        if (submissionTaco != null) { Destroy(submissionTaco.gameObject); }

        // create init submission taco
        GameObject taco = Instantiate(tacoPrefab, GetComponentInChildren<IngredientBenchManager>().tacoSpawnPoint.position, Quaternion.identity);
        submissionTaco = taco.GetComponent<Taco>();
        taco.transform.parent = transform;
    }


    // continue through remaining customers
    public void CustomerRotation()
    {
        // if curr customer is null, and more customers remaining
        // create a new cutsomer from customer manager
        // store new customer in curr cutsomer

        //Calls to create a new customer as long as there are more customers to generate up to max number of customers in line at once
        if (customersLeftToGenerate > 0 && customerManager.customerList.Count < lineSize)
        {
            customersLeftToGenerate--;
            //The ID passed in for each customer starts at 1 and counts up to totalCustomers
            currCustomer = customerManager.CreateNewCustomer(totalCustomers - customersLeftToGenerate).GetComponent<Customer>();
        }    
    }

    // submit taco to customer to be graded
    public void SubmitTaco()
    {
        scoreType score = currCustomer.ScoreTaco(submissionTaco);
        NewTacoScore(score);

        Debug.Log("Submitted Taco! Customer Score " + score);

        CreateNewSubmissionTaco();

        customerManager.RemoveCurrentCustomer();
    }



    // Parameter: score from completed Taco
    // Updates gameScore, perfectCounter and comboCounter as necessary
    public void NewTacoScore(scoreType score)
    {
        if (score == scoreType.PERFECT)
        {
            gameScore += 3;
            perfectCounter++;
            if (perfectCounter % 3 == 0 && perfectCounter != 0)
            {
                comboCounter++; //display combo stuff //maybe move elsewhere
            }
        }
        else
        {
            perfectCounter = 0;
        }

        if (score == scoreType.GOOD)
        {
            gameScore += 2;
        }

        if (score == scoreType.OKAY)
        {
            gameScore += 1;
        }

        if (score == scoreType.FAILED)
        {
            gameScore += 0;
        }
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
