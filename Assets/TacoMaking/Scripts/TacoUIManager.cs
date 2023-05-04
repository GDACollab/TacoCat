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
    public Image star;
    public float starScale = 1;
    public Transform starPosition;
    public TMP_Text scoreText;
    public Color emptyStarColor = Color.black;
    
    [Header("Order Window UI")]
    public float ingredientScale = 0.1f;
    public float ingredientSpacing = 2f;
    public Transform ingredientPointParent;
    [Header("Test Order Window UI")]
    public List<ingredientType> testIngredients = new List<ingredientType>(4);
    
    [Header("Gas/Nitro UI")]
    public Image fuelBar;
    public Image nCharge1, nCharge2, nCharge3;

    [Header("Test Gas/Nitro UI")]
    public float fuelAmount = 0;
    public float maxFuelAmount = 1;
    public int numNitroCharges = 0;
    
    
    public List<Image> starTracker = new List<Image>();
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

        // Update Order
        if (customerManager.currCustomer != null)
        {
            DisplayOrder(customerManager.currCustomer.order, ingredientSpacing);
        }

        // Update Gas Amount (to be deleted later)
        //DisplayGas(fuelAmount, maxFuelAmount);
        //DisplayNitro(Vehicle.nitroCharges);


    }
    
    // Call this function to display the score. Takes in the score.
    public void DisplayScore(scoreType score){
        ClearDisplayScore();

        float starSpacing = 1.2f;
        // Display 3 stars and "Perfect Taco!" text
        if(score == scoreType.PERFECT){
            scoreText.text = "Perfect Taco!";
            List<Image> starImg = new List<Image>(5);
            starImg.Add(Instantiate(star, starPosition.position, transform.rotation));
            starImg.Add(Instantiate(star, new Vector3(starPosition.position.x+star.rectTransform.lossyScale.x*starSpacing, starPosition.position.y, starPosition.position.z), transform.rotation));
            starImg.Add(Instantiate(star, new Vector3(starPosition.position.x-star.rectTransform.lossyScale.x*starSpacing, starPosition.position.y, starPosition.position.z), transform.rotation));
            foreach(Image img in starImg){
                img.rectTransform.SetParent(starPosition);
                img.transform.localScale = Vector3.one*starScale;
                starTracker.Add(img);
            }
        }
        // Display 2 stars and "Good Taco!" text
        else if(score == scoreType.GOOD){
            scoreText.text = "Good Taco!";
            List<Image> starImg = new List<Image>(5);
            starImg.Add(Instantiate(star, new Vector3(starPosition.position.x+star.rectTransform.lossyScale.x*starSpacing/2, starPosition.position.y, starPosition.position.z), transform.rotation));
            starImg.Add(Instantiate(star, new Vector3(starPosition.position.x-star.rectTransform.lossyScale.x*starSpacing/2, starPosition.position.y, starPosition.position.z), transform.rotation));
            foreach(Image img in starImg){
                img.rectTransform.SetParent(starPosition);
                img.transform.localScale = Vector3.one*starScale;
                starTracker.Add(img);
            }
        }
        // Display 1 star and "Okay Taco" text
        else if(score == scoreType.OKAY){
            scoreText.text = "Okay Taco...";
            Image img = Instantiate(star, starPosition.position, transform.rotation);
            img.rectTransform.SetParent(starPosition);
            img.transform.localScale = Vector3.one*starScale;
            starTracker.Add(img);
        }
        // Display 3 black stars and "Failed Taco" text
        else if(score == scoreType.FAILED){
            scoreText.text = "Failed Taco...";
            List<Image> starImg = new List<Image>(5);
            starImg.Add(Instantiate(star, starPosition.position, transform.rotation));
            starImg.Add(Instantiate(star, new Vector3(starPosition.position.x+star.rectTransform.lossyScale.x*starSpacing, starPosition.position.y, starPosition.position.z), transform.rotation));
            starImg.Add(Instantiate(star, new Vector3(starPosition.position.x-star.rectTransform.lossyScale.x*starSpacing, starPosition.position.y, starPosition.position.z), transform.rotation));
            foreach(Image img in starImg){
                img.rectTransform.SetParent(starPosition);
                img.transform.localScale = Vector3.one*starScale;
                img.color = emptyStarColor;
                starTracker.Add(img);
            }
        }
    }
    
    // Clears the displayed score
    public void ClearDisplayScore(){
        if(starTracker.Count>0){
            foreach(Image img in starTracker){
                Destroy(img.gameObject);
            }

            starTracker.Clear();
        }
    }
    
    // Call this to display the order. Takes in the order as a list of ingredients. 
    public void DisplayOrder(List<ingredientType> order, float spacing)
    {
        ClearDisplayOrder();

        // Calculate the starting y position
        float startY = ingredientPointParent.position.y - ((order.Count - 1) * spacing) / 2;

        // Spawn each object at the appropriate position
        for (int i = 0; i < order.Count; i++)
        {
            Vector3 position = new Vector3(ingredientPointParent.position.x, startY + i * spacing, ingredientPointParent.position.z);

            GameObject ingredient = tacoGameManager.GetIngredientObject(order[i]);

            GameObject newIngredient = Instantiate(ingredient, position, Quaternion.identity);
            newIngredient.transform.parent = ingredientPointParent;
            newIngredient.transform.localScale = Vector3.one * ingredientScale;


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
