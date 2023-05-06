using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TacoUIManager : MonoBehaviour
{
    public TacoMakingGameManager tacoGameManager;
    CustomerManager customerManager;

    [Space(10)]
    public GameObject endText;
    public bool endOfGame;

    [Header("Score UI")]
    public GameObject starPrefab;
    public float starScale = 1;
    public float starSpacing = 10;
    public Transform starPosition;
    //public TMP_Text scoreText;
    public Color emptyStarColor = Color.black;
    
    [Header("Order Window UI")]
    public float ingredientScale = 0.1f;
    public float ingredientSpacing = 2f;
    public Transform ingredientPointParent;
    public bool newOrderTaken;
    [Header("Test Order Window UI")]
    public List<ingredientType> testIngredients = new List<ingredientType>(4);
    
    [Header("Gas/Nitro UI")]
    public Image fuelBar;
    public Image nCharge1, nCharge2, nCharge3;

    [Header("Test Gas/Nitro UI")]
    public float fuelAmount = 0;
    public float maxFuelAmount = 1;
    public int numNitroCharges = 0;
    
    
    public List<GameObject> starTracker = new List<GameObject>();
    private List<GameObject> orderIngredientObjects = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        customerManager = tacoGameManager.customerManager;

        DisplayGas(0);
        DisplayNitro(Vehicle.nitroCharges);

        DisplayScore(scoreType.FAILED);

    }

    // Update is called once per frame
    void Update()
    {
        // Score gets updated in Game Manager

        // Update Gas Amount (to be deleted later)
        //DisplayGas(fuelAmount, maxFuelAmount);
        //DisplayNitro(Vehicle.nitroCharges);

        if (customerManager.currCustomer != null && !newOrderTaken)
        {
            DisplayOrder(customerManager.currCustomer.order);
            newOrderTaken = true;
        }

    }
    
    // Call this function to display the score. Takes in the score.
    public void DisplayScore(scoreType score){
        ClearDisplayScore();

        // Display stars and set text based on score
        if (score == scoreType.PERFECT)
        {
            //scoreText.text = "Perfect Taco!";
            SpawnStars(3, starPosition);
        }
        else if (score == scoreType.GOOD)
        {
            //scoreText.text = "Good Taco!";
            SpawnStars(2, starPosition);
        }
        else if (score == scoreType.OKAY)
        {
            //scoreText.text = "Okay Taco...";
            SpawnStars(1, starPosition);
        }
        else if (score == scoreType.FAILED)
        {
            //scoreText.text = "Failed Taco...";
            SpawnStars(3, starPosition);
            foreach (GameObject star in starTracker)
            {
                star.GetComponent<SpriteRenderer>().color = emptyStarColor;
            }
        }
    }

    public void SpawnStars(int numStars, Transform parentTransform)
    {
        // Clear existing stars
        foreach (GameObject star in starTracker)
        {
            Destroy(star.gameObject);
        }
        starTracker.Clear();

        // Spawn new stars
        for (int i = 0; i < numStars; i++)
        {
            GameObject star = Instantiate(starPrefab, parentTransform);
            star.transform.localScale = Vector3.one * starScale;
            starTracker.Add(star);
        }

        // Position stars
        if (numStars == 1)
        {
            starTracker[0].transform.localPosition = Vector3.zero;
        }
        else if (numStars == 2)
        {
            starTracker[0].transform.localPosition = new Vector3(-starSpacing / 2f, 0f, 0f);
            starTracker[1].transform.localPosition = new Vector3(starSpacing / 2f, 0f, 0f);
        }
        else if (numStars == 3)
        {
            starTracker[0].transform.localPosition = new Vector3(-starSpacing, 0f, 0f);
            starTracker[1].transform.localPosition = Vector3.zero;
            starTracker[2].transform.localPosition = new Vector3(starSpacing, 0f, 0f);
        }
    }

    // Clears the displayed score
    public void ClearDisplayScore(){
        if(starTracker.Count>0){
            foreach(GameObject img in starTracker){
                Destroy(img.gameObject);
            }

            starTracker.Clear();
        }
    }
    
    // Call this to display the order. Takes in the order as a list of ingredients. 
    public void DisplayOrder(List<ingredientType> order)
    {
        ClearDisplayOrder();

        // Calculate the starting y position
        float startY = ingredientPointParent.position.y - ((order.Count - 1) * ingredientSpacing) / 2;

        // Spawn each object at the appropriate position
        for (int i = 0; i < order.Count; i++)
        {
            Vector3 position = new Vector3(ingredientPointParent.position.x, startY + i * ingredientSpacing, ingredientPointParent.position.z);

            GameObject ingredient = tacoGameManager.GetIngredientObject(order[i]);

            GameObject newIngredient = Instantiate(ingredient, position, Quaternion.identity);
            newIngredient.transform.parent = ingredientPointParent;
            newIngredient.transform.localScale = Vector3.one * ingredientScale;

            newIngredient.GetComponent<SpriteRenderer>().sortingLayerName = "PlayArea";
            newIngredient.GetComponent<SpriteRenderer>().sortingOrder = 2;

            orderIngredientObjects.Add(newIngredient);
        }
    }

    // Clears the displayed order
    public void ClearDisplayOrder(){
        if(orderIngredientObjects.Count>=0){
            foreach(GameObject obj in orderIngredientObjects){
                Destroy(obj);
            }
        }
        orderIngredientObjects.Clear();
    }

    // Call this to display the fuel and nitro amounts. 
    // It takes in the fuel amount, the maximum amount of fuel, and the number of nitro charges
    public void DisplayGas(float percent){
        fuelBar.fillAmount = Mathf.Clamp(percent, 0, 1f); // Fill the bar according to the given fuel amount
    }
    public void DisplayNitro(int nitro){ // Switch between the 4 possible states of nitro charges
        switch (nitro){
        case 1:
            nCharge1.color = Color.HSVToRGB(236f/360, 0.0f, 0.96f, true);
            nCharge2.color = Color.black;
            nCharge3.color = Color.black;
            break;
        case 2:
            nCharge1.color = Color.HSVToRGB(236f/360, 0.0f, 0.96f, true);
            nCharge2.color = Color.HSVToRGB(236f/360, 0.0f, 0.96f, true);
            nCharge3.color = Color.black;
            break;
        case 3:
            nCharge1.color = Color.HSVToRGB(236f/360, 0.0f, 0.96f, true);
            nCharge2.color = Color.HSVToRGB(236f/360, 0.0f, 0.96f, true);
            nCharge3.color = Color.HSVToRGB(236f/360, 0.0f, 0.96f, true);
            break;
        default:
            nCharge1.color = Color.black;
            nCharge2.color = Color.black;
            nCharge3.color = Color.black;
            break;
        }
    }
}
