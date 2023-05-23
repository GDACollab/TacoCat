using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum INGREDIENT_TYPE { NONE, FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS }
public enum scoreType { NONE, PERFECT, GOOD, OKAY, FAILED } // possible scores a taco can get.

public class TacoMakingGameManager : MonoBehaviour
{
    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public AudioManager audioManager;
    public TacoUIManager uiManager;
    public IngredientBenchManager benchManager;
    public GameObject background;

    public bool endOfGame;

    [Header("Submission Taco")]
    public Taco submissionTaco;

    [Header("Hand")]
    public PlayerHand hand;

    [Header("Customers")]
    public CustomerManager customerManager;
    public int perfectCounter; //counts the number of perfect tacos in a row, resets when a !perfect taco is submitted
    public int comboCounter;   //counts the number for 3 combos in total throughout the whole minigame
    public int totalCustomers; //Total number of customers that will appear
    public int customersLeftToGenerate; //the number of customers left to generate in the scene
    public int difficulty = 1;
    [HideInInspector]
    public int submittedCustomers;
    public int lineSize;

    [Header("Score")]
    public float gameScore = 0; // max score is 3 * numOfCustomers served
    public float maxGameScore;
    public float gasAmount = 0;
    public float minimumGasThreshold = .5f; // 0 is none, 1 is full
    public int nitroCharges = 0;

    [Header("Prefabs")]
    public GameObject tacoPrefab;
    public List<GameObject> allIngredientPrefabs;
    public List<GameObject> allIngredientBinPrefabs;

    public TMP_Text clockTime;
    public bool activateTutorial = true;

    public void Start()
    {
        difficulty = Mathf.Min(difficulty, 3);
        customerManager.difficulty = difficulty;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioManager = gameManager.audioManager;
        benchManager = GetComponentInChildren<IngredientBenchManager>();
        uiManager = GetComponentInChildren<TacoUIManager>();
        
        for(int i = 0; i < background.transform.childCount; i++){
            background.transform.GetChild(i).gameObject.SetActive(false);
        }
        background.transform.GetChild(difficulty-1).gameObject.SetActive(true);
        
        activateTutorial = (difficulty!=1) ? false : activateTutorial;

        CreateNewSubmissionTaco();

        customersLeftToGenerate = totalCustomers;
    }

    public void Update()
    {
        if(activateTutorial){
            if(Input.GetKeyDown(KeyCode.Space)){
                activateTutorial = false;
            }
            if(gameManager.currGame==currGame.TACO_MAKING){
                setTutorial(activateTutorial);
            }
        }
        else{
            CustomerRotation();

            // check for end
            // if (submittedCustomers == totalCustomers)
            if (gasAmount >= minimumGasThreshold)
            {
                //uiManager.endText.SetActive(true);
                endOfGame = true;
            }
        }
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
        //Calls to create a new customer as long as there are more customers to generate up to max number of customers in line at once
        if (customersLeftToGenerate > 0 && customerManager.customerList.Count < lineSize + 1)
        {
            customersLeftToGenerate--;
            //The ID passed in for each customer starts at 1 and counts up to totalCustomers
            Customer customer = customerManager.CreateNewCustomer(totalCustomers - customersLeftToGenerate).GetComponent<Customer>();

        }

        if (customersLeftToGenerate < 3 && gasAmount < minimumGasThreshold) 
        {
            customersLeftToGenerate += 3;
        }

        if (customersLeftToGenerate < 3 && gasAmount < minimumGasThreshold) 
        {
            customersLeftToGenerate += 3;
        }
    }

    // submit taco to customer to be graded
    public void SubmitTaco()
    {
        //Can't submit taco until customer is finished moving
        if (customerManager.currCustomer.moveRoutine == null)
        {
            scoreType score = customerManager.currCustomer.ScoreTaco(submissionTaco);
            NewTacoScore(score);
        
            Debug.Log("Submitted Taco! Customer Score " + score);

            //audioManager.Play("event:/SFX/Taco Making/Bell Ding");

            CreateNewSubmissionTaco();

            customerManager.RemoveCurrentCustomer(score);

            submittedCustomers++;
            uiManager.newOrderTaken = false;
        }
    }

    // Parameter: score from completed Taco
    // Updates gameScore, perfectCounter and comboCounter as necessary
    public void NewTacoScore(scoreType score)
    {
        uiManager.DisplayScore(score);
        if (score == scoreType.PERFECT)
        {
            gameScore += 3;
            
            perfectCounter++;
   
            if (perfectCounter % 3 == 0 && perfectCounter != 0 && submittedCustomers <= totalCustomers)
            {
                comboCounter++;
                if (nitroCharges < 3)
                {
                    nitroCharges++;
                }
                uiManager.DisplayNitro(nitroCharges); //updates the nitro display
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
        SetGasAmount();
        uiManager.DisplayGas(gasAmount); //100 is the max fuel amount, 
        //DisplayGas ^^ needs to be changed later so that you don't need to insert max fuel everytime you want to update the bar
    }


    // << SET GAS AMOUNT FROM CURRENT SCORE >>
    public void SetGasAmount()
    {
        // gas amount == correct tacos / max game score (( need to create new variables ))
        maxGameScore = totalCustomers * 3;
        gasAmount = gameScore/maxGameScore;
        Debug.Log("gasAmout: "+gasAmount+" gameScore: "+ gameScore+" maxGameScore: "+ maxGameScore+" totalCustomers: "+ totalCustomers);
    }
    
    private void setTutorial(bool set){
        transform.Find("Tutorial Canvas").gameObject.SetActive(set);
        GetComponent<InputManager>().enabled = !set;
        Time.timeScale = (set) ? 0 : 1;
    }


    #region HELPER FUNCTIONS ==============================================================

    // << GET CURRENT INGREDIENT IN BIN >>
    public IngredientBin GetIngredientBin(int index)
    {
        return benchManager.ingredientBins[index];
    }

    // << ADD INGREDIENT TO SUBMISSION TACO >>
    public void AddIngredientToTaco(INGREDIENT_TYPE type)
    {
        submissionTaco.addIngredient(type); 
        submissionTaco.addingredientObject(GetIngredientObject(type));
    }


    // return prefab that is related to the input enum type
    public GameObject GetIngredientObject(INGREDIENT_TYPE ingrType)
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


    // return prefab that is related to the input enum type
    public GameObject GetIngredientBinSprite(INGREDIENT_TYPE ingrType)
    {
        // for each object in prefab list
        foreach (GameObject obj in allIngredientBinPrefabs)
        {
            // look at the ingredient script inside prefab
            if (obj.GetComponent<Ingredient>().type == ingrType)
            {
                return obj;
            }
        }

        Debug.Log("Could not find " + ingrType + " bin prefab");

        return null;
    }

    #endregion
}
