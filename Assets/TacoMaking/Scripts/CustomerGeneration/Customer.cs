using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpeciesType {Fish, Ravens, Sheep, Frogs, Capybaras};

public class Customer: MonoBehaviour
{
    CustomerManager customerManager;
    TacoMakingGameManager tacoGameManager;

    public Taco submittedTaco;

    [Header("Order UI")]
    public OrderBubble orderUI;

    public enum species {Fish,Raven,Sheep,Frog,Capybara}; //species selectable by CreateCustomerOrder
    public List<ingredientType> order; //ingredients in the order

    private Vector3 prevPos;
    private Vector3 targetPos;
    private float interpolater;
    [HideInInspector] public float transitionTime; //How long it takes in seconds for the customer to move between positions
    private float currTransitionTime; //Used for keeping track of time during transitions
    [HideInInspector] public float transitionOffset; //The most that a customers transition time can be randomly offset (used to make customers move at diff speeds)
    [HideInInspector] private float transitionOffsetTimer;
    [HideInInspector] public int currPosition;
    private Coroutine lastRoutine = null;
    [HideInInspector] public bool hasEndingDialogue;
    [HideInInspector] public bool hasIntroDialgue;
    [HideInInspector] public float dialoguePause;

    // List of possible colors to tint this customer's sprite once their taco is finished.
    // Elements correspond to the values in the scoreType enum in TacoMakingGameManager.cs .
    public List<Color> colorAfterTacoFinished = new List<Color>
    {
        Color.gray,
        new Color(0 / 255.0f, 243 / 255.0f, 27 / 255.0f), // Bright green
        new Color(192 / 255.0f, 255.0f, 101 / 255.0f), // Light green
        new Color(249 / 255.0f, 141 / 255.0f, 0 / 255.0f), // Orange
        Color.red
    };

    private void Awake()
    {
        tacoGameManager = GetComponentInParent<TacoMakingGameManager>();
        customerManager = GetComponentInParent<CustomerManager>();
    }

    void Start()
    {
        orderUI.gameObject.SetActive(false);

        order = CreateCustomerOrder(3, 4);
        // ShowBubbleOrder(order);
    }

    public List<ingredientType> CreateCustomerOrder(int minOrderLength, int maxOrderLength) 
    {

        Debug.Log("Created Customer Order");

        // Decide on customer species
        species custSpecies;
        custSpecies = (species)Random.Range(0,4); //Selects random species from the range of possible options

        // get menu from bench manager
        List<ingredientType> menu = tacoGameManager.benchManager.menu;        
        int orderLength = Random.Range(minOrderLength, maxOrderLength + 1); // randomize order length

        // To be returned
        List<ingredientType> s_order = new List<ingredientType>(orderLength);

        // Decides on possible ingredients + said element's weight
        List<int> custPreference = new List<int> { 0, 1, 2, 3, 4 };
        // I would like to switch this with calling for the required value (ie getting fish.value)
        // but afaik we don't have that implemented, and I don't want to risk messing with it rn
        switch (custSpecies)
        {
            case species.Fish: //No fish, 2x sour cream
                custPreference = new List<int> { 0, 1, 3, 4, 4 };
                break;
            case species.Raven: //2x fish
                custPreference = new List<int> { 0, 1, 2, 2, 3, 4 };
                break;
            case species.Sheep: //2x cabbage
                custPreference = new List<int> { 0, 0, 1, 2, 3, 4 };
                break;
            case species.Frog: // 1/2x fish, 2x jalapenos
                custPreference = new List<int> { 0, 0, 1, 1, 2, 3, 3, 3, 3, 4, 4 };
                break;
            case species.Capybara: // 1/2x Pico
                custPreference = new List<int> { 0, 0, 1, 2, 2, 3, 3, 4, 4 };
                break;
        }
        Debug.Log(custPreference);

        // Fill list with random items from menu
        for (int i = 0; i < orderLength; i++)
        {
            int randValue = custPreference[Random.Range(0, custPreference.Count)];
            if (s_order.Contains(menu[randValue])) //If item pulled has already been added, all instances of it are removed as future possibilities
            {
                while (custPreference.Contains(randValue)) {
                    custPreference.Remove(randValue);
                    Debug.Log("Removed item "+randValue+" from "+string.Join(",",custPreference));
                }
            } 
            s_order.Add(menu[randValue]);
        }

        return s_order;
    }

    public SpeciesType RandomizeSpecies()    //Generates a Random Species
    {
        return (SpeciesType)Random.Range(0,4);
    }

    // << SPAWN ORDER UI BOX >>
    public GameObject ShowBubbleOrder(List<ingredientType> order)
    {
        orderUI.gameObject.SetActive(true);

        orderUI.order = order;

        orderUI.ShowOrder();

        return null;
    }


    // compares the list of ingredients in the taco submitted and the list of ingredients in the customer's order returns the taco's score
    public scoreType ScoreTaco(Taco tacoToScore)
    {
        // [[ INGREDIENT COUNT]] =========================================================
        int ingredientCountDifference = tacoToScore.ingredients.Count - order.Count;
        Debug.Log("Ingredient Count Difference : " + ingredientCountDifference);

        // if no ingredients in taco, fail
        if (tacoToScore.ingredients.Count == 0) { return scoreType.FAILED; }

        // if +-2 ingredients in taco than order, fail
        else if (Mathf.Abs(ingredientCountDifference) >= 2) { return scoreType.FAILED; }

        // if +-1 ingredients in taco than order, then okay
        else if (Mathf.Abs(ingredientCountDifference) == 1) { return scoreType.OKAY; }

        // [[ SAME INGREDIENTS // INGREDIENT ORDER ]] ====================================
        int numSameIngredients = compareIngredients(tacoToScore);
        int correctPlacementCount = compareIngredientOrder(tacoToScore);

        // << PERFECT >> ingredients are the same and order is perfect
        if (numSameIngredients == order.Count && correctPlacementCount == order.Count)
        {
            
            return scoreType.PERFECT;
        }
        // << GOOD >> ingredients are the same, but order is wrong
        else if (numSameIngredients == order.Count && correctPlacementCount != order.Count)
        {
            return scoreType.GOOD;
        }
        // << OKAY TACO >>  1 missing/extra ingredients, incorrect order
        else if ((numSameIngredients == order.Count - 1) && correctPlacementCount != order.Count)
        {
            return scoreType.OKAY;
        }
        // << FAILED TACO >>
        else
        {
            return scoreType.FAILED;
        }
    }


    public int compareIngredients(Taco taco)
    {
        int sameIngredientCount = 0;

        foreach(ingredientType ingr in taco.ingredients)
        {
            if (order.Contains(ingr))
            {
                sameIngredientCount++;
            }
        }

        return sameIngredientCount;
    }

    public int compareIngredientOrder(Taco taco)
    {
        int correctPlacementCount = 0;

        if (taco.ingredients.Count > order.Count) { Debug.Log("Submitted " + (taco.ingredients.Count - order.Count) + " more Ingredients than order"); }

        // iterate through taco and check order placement
        for (int i = 0; i < order.Count; i++)
        {
            // if taco ingredient is valid
            if (i < taco.ingredients.Count)
            {
                // if taco ingredient is in the same index of customer order, add to count
                if (taco.ingredients[i] == order[i]) { correctPlacementCount++; }
            }
            else
            {
                Debug.Log("Less Ingredients than order");
            }
        }

        return correctPlacementCount;
    }

    //Moves the customer from their previous position to their new position in line
    public void MoveCustomer(Vector3 newPosition)
    {
        interpolater = 0;
        currTransitionTime = 0;
        transitionOffsetTimer = 0;
        prevPos = transform.position;
        targetPos = newPosition;
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        lastRoutine = StartCoroutine(MovePosition());
    }

    private IEnumerator MovePosition()
    {
        //Buffer for customers have dialogue
        while (dialoguePause > 0)
        {
            dialoguePause -= Time.deltaTime;
            yield return null;
        }

        //Buffer between different customers moving
        while (transitionOffsetTimer < transitionOffset)
        {
            transitionOffsetTimer += Time.deltaTime;
            yield return null;
        }

        //Stops moving customer once they are at their new position
        while (currTransitionTime < transitionTime)
        {
            //Math to make the transition ease in and out
            interpolater = currTransitionTime / transitionTime;
            interpolater = interpolater * interpolater * (3f - 2f * interpolater);
            transform.position = Vector3.Lerp(prevPos, targetPos, interpolater);
            currTransitionTime += Time.deltaTime;
            yield return null;
        }
        lastRoutine = null;
    }

    // Changes this customer's sprite appearance based on the score of their taco
    public void CustomerReaction(scoreType tacoScore)
    {
        SpriteRenderer mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySpriteRenderer.color = colorAfterTacoFinished[(int)tacoScore];
    }
}
