using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TacoUIManager : MonoBehaviour
{
    [Header("Score UI")]
    public Image star;
    public float starScale = 1;
    public Transform starPosition;
    public TMP_Text scoreText;
    [Header("Test the score. 0: Failed, 1: Okay, 2: Good, 3: Perfect.")]
    public int testScore = 3;
    
    [Header("Order Window UI")]
    public float ingredientScale = 0.1f;
    public Transform tacoPos, inPos1, inPos2, inPos3, inPos4;
    public GameObject tacoShell;
    [Header("Test Order Window UI")]
    public List<ingredientType> testIngredients = new List<ingredientType>(4);
    
    [Header("Gas/Nitro UI")]
    public Image fuelBar;
    public Image nCharge1, nCharge2, nCharge3;
    [Header("Test Gas/Nitro UI")]
    public float fuelAmount = 0;
    public float maxFuelAmount = 100;
    public int numNitroCharges = 0;
    
    TacoMakingGameManager tacoGameManager;
    
    private List<Image> starTracker = new List<Image>();
    private List<Transform> ingredient_pos;
    private List<GameObject> orderIngredientObjects = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        tacoGameManager = GetComponentInParent<TacoMakingGameManager>();
        
        ingredient_pos = new List<Transform>(5);
        
        ingredient_pos.Add(inPos1);
        ingredient_pos.Add(inPos2);
        ingredient_pos.Add(inPos3);
        ingredient_pos.Add(inPos4);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(testScore){
            case 0:
                DisplayScore(scoreType.FAILED);
                break;
            case 1:
                DisplayScore(scoreType.OKAY);
                break;
            case 2: 
                DisplayScore(scoreType.GOOD);
                break;
            case 3: 
                DisplayScore(scoreType.PERFECT);
                break;
        }
        
        DisplayOrder(testIngredients);
        
        DisplayFuel(fuelAmount, maxFuelAmount, numNitroCharges);
    }
    
    public void DisplayScore(scoreType score){
        clearDisplayScore();
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
                img.color = Color.black;
                starTracker.Add(img);
            }
        }
    }
    
    public void clearDisplayScore(){
        if(starTracker.Count>0){
            foreach(Image img in starTracker){
                Destroy(img);
            }
        }
    }
    
    public void DisplayOrder(List<ingredientType> order){
        ClearDisplayOrder();
        GameObject newTacoShell = Instantiate(tacoShell, tacoPos.position, Quaternion.identity);
        newTacoShell.transform.parent = tacoPos;
        // newTacoShell.transform.localScale = new Vector3(2f, 1.5f, 1f);
        orderIngredientObjects.Add(newTacoShell);
        
        for (int i = 0; i < order.Count; i++){
            GameObject ingredient = tacoGameManager.GetIngredientObject(order[i]);

            GameObject newIngredient = Instantiate(ingredient, ingredient_pos[i].position, Quaternion.identity);
            newIngredient.transform.parent = ingredient_pos[i];
            newIngredient.transform.localScale = Vector3.one * ingredientScale;

            orderIngredientObjects.Add(newIngredient);
        }
    }
    
    public void ClearDisplayOrder(){
        if(orderIngredientObjects.Count>=0){
            foreach(GameObject obj in orderIngredientObjects){
                Destroy(obj);
            }
        }
        orderIngredientObjects.Clear();
    }
    
    public void DisplayFuel(float fuel, float maxFuel, int nitro){
        fuelBar.fillAmount = Mathf.Clamp(fuel/maxFuel, 0, 1f);
        switch(nitro){
            case 1:
                nCharge1.color = Color.HSVToRGB(236f/360, 0.71f, 0.96f, true);
                nCharge2.color = Color.black;
                nCharge3.color = Color.black;
                break;
            case 2:
                nCharge1.color = Color.HSVToRGB(236f/360, 0.71f, 0.96f, true);
                nCharge2.color = Color.HSVToRGB(236f/360, 0.71f, 0.96f, true);
                nCharge3.color = Color.black;
                break;
            case 3:
                nCharge1.color = Color.HSVToRGB(236f/360, 0.71f, 0.96f, true);
                nCharge2.color = Color.HSVToRGB(236f/360, 0.71f, 0.96f, true);
                nCharge3.color = Color.HSVToRGB(236f/360, 0.71f, 0.96f, true);
                break;
            default:
                nCharge1.color = Color.black;
                nCharge2.color = Color.black;
                nCharge3.color = Color.black;
                break;
        }
    }
}
