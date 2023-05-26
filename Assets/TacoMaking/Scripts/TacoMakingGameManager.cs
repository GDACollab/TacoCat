using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum INGREDIENT_TYPE { NONE, FISH, SOUR_CREAM, PICO_DE_GALLO, CABBAGE, SLICED_JALAPENOS }
public enum SUBMIT_TACO_SCORE { NONE, COMBO, PERFECT, GOOD, OKAY, FAILED } // possible scores a taco can get.

public enum TACOMAKING_STATE { LOADING, TUTORIAL, PLAY, END, END_TRANSITION }

public class TacoMakingGameManager : MonoBehaviour
{
    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public AudioManager audioManager;
    public TacoUIManager uiManager;
    public TacoMakingLighting lightingManager;
    public IngredientBenchManager benchManager;
    public GameObject background;

    [Header("States")]
    public TACOMAKING_STATE state = TACOMAKING_STATE.LOADING;

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

    public void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioManager = gameManager.audioManager;
        benchManager = GetComponentInChildren<IngredientBenchManager>();
        uiManager = GetComponentInChildren<TacoUIManager>();

        // get difficulty
        difficulty = Mathf.Min(difficulty, 3);
        customerManager.difficulty = difficulty;

        /*
        // enable / disable backgrounds
        for (int i = 0; i < background.transform.childCount; i++){
            background.transform.GetChild(i).gameObject.SetActive(false);
        }

        background.transform.GetChild(difficulty-1).gameObject.SetActive(true);
        */        

        // create taco to place ingredients on
        CreateNewSubmissionTaco();

        // init customer count
        customersLeftToGenerate = totalCustomers;

        // set lighting manager start values
        lightingManager.timeOfDay = gameManager.main_gameTimer;

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForSeconds(1);

        // determine if tutorial is needed
        if (difficulty == 1)
        {
            state = TACOMAKING_STATE.TUTORIAL;
            nitroCharges = 1;
            uiManager.DisplayNitro(nitroCharges); //updates the nitro display

        }
        else
        {
            state = TACOMAKING_STATE.PLAY;
        }

        yield return new WaitUntil(() => uiManager.camEffectManager != null);

        uiManager.camEffectManager.StartFadeIn(2f);

    }

    public void Update()
    {
        switch(state)
        {
            case TACOMAKING_STATE.LOADING:
                break;
            case TACOMAKING_STATE.TUTORIAL:
                if (!uiManager.camEffectManager.isFading)
                {
                    uiManager.tutorialCanvas.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        uiManager.tutorialCanvas.SetActive(false);
                        state = TACOMAKING_STATE.PLAY;
                    }
                }
                break;
            case TACOMAKING_STATE.PLAY:
                if (hand.state == PlayerHand.HAND_STATE.DISABLED) { hand.state = PlayerHand.HAND_STATE.HOME; }

                StartCoroutine(uiManager.OpenWindow());

                CustomerRotation();

                // engage minimum gas threshold, or reached max gas
                if ( (submittedCustomers >= totalCustomers && gasAmount >= minimumGasThreshold) || gasAmount > 1)
                {
                    state = TACOMAKING_STATE.END;
                }

                //update lightingManager
                lightingManager.timeOfDay = gameManager.main_gameTimer;
                break;
            case TACOMAKING_STATE.END:

                uiManager.endCanvas.SetActive(true);

                uiManager.endCanvas.SetActive(true);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    uiManager.endCanvas.SetActive(false);
                    uiManager.camEffectManager.StartFadeOut(1.5f);
                    state = TACOMAKING_STATE.END_TRANSITION;
                }

                break;
            case TACOMAKING_STATE.END_TRANSITION:
                break;
            default:
                break;
        }
    }

    public void CreateNewSubmissionTaco()
    {
        if (submissionTaco != null)
        {
            GameObject oldTaco = submissionTaco.gameObject;
            Destroy(oldTaco, 3);

            submissionTaco = null;
        }

        // create init submission taco
        GameObject taco = Instantiate(tacoPrefab, benchManager.tacoSpawnPoint);
        submissionTaco = taco.GetComponent<Taco>();

        submissionTaco.PlayEnterAnim();
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
        if (customerManager.currCustomer != null && customerManager.currCustomer.moveRoutine == null)
        {
            SUBMIT_TACO_SCORE score = customerManager.currCustomer.ScoreTaco(submissionTaco);
            SUBMIT_TACO_SCORE comboContextScore = NewTacoScore(score);

            uiManager.ClearDisplayOrder();

            // play the corresponding taco anim
            if (comboContextScore == SUBMIT_TACO_SCORE.COMBO)
            {
                submissionTaco.PlayComboAnim();
                //yield return new WaitForSeconds(1.5f);

            }
            else if (comboContextScore == SUBMIT_TACO_SCORE.PERFECT)
            {
                submissionTaco.PlayPerfectAnim();
                //yield return new WaitForSeconds(1);
            }
            else
            {
                submissionTaco.PlayExitAnim();
            }

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
    public SUBMIT_TACO_SCORE NewTacoScore(SUBMIT_TACO_SCORE score)
    {
        

        uiManager.DisplayScore(score);
        if (score == SUBMIT_TACO_SCORE.PERFECT)
        {
            gameScore += 3;
            
            perfectCounter++;
   
            if (perfectCounter % 3 == 0 && perfectCounter != 0 && submittedCustomers <= totalCustomers)
            {
                score = SUBMIT_TACO_SCORE.COMBO;

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

        if (score == SUBMIT_TACO_SCORE.GOOD)
        {
            gameScore += 2;
        }

        if (score == SUBMIT_TACO_SCORE.OKAY)
        {
            gameScore += 1;
        }

        if (score == SUBMIT_TACO_SCORE.FAILED)
        {
            gameScore += 0;
        }

        SetGasAmount();
        uiManager.DisplayGas(gasAmount);

        return score;
    }


    // << SET GAS AMOUNT FROM CURRENT SCORE >>
    public void SetGasAmount()
    {
        // gas amount == correct tacos / max game score (( need to create new variables ))
        maxGameScore = totalCustomers * 3;
        gasAmount = gameScore/maxGameScore;
        Debug.Log("gasAmout: "+gasAmount+" gameScore: "+ gameScore+" maxGameScore: "+ maxGameScore+" totalCustomers: "+ totalCustomers);
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
