using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer: MonoBehaviour
{
    CustomerManager customerManager;
    TacoMakingGameManager tacoGameManager;

    public Taco submittedTaco;

    [Header("Order UI")]
    public OrderBubble orderUI;

    public List<ingredientType> order; //ingredients in the order

    [HideInInspector] public Vector3 prevPos;
    [HideInInspector] public Vector3 targetPos;
    [HideInInspector] public float interpolater;
    [HideInInspector] public float transitionTime;     //How long it takes in seconds for the customer to move between positions
    [HideInInspector] public float currTransitionTime; //Used for keeping track of time during transitions
    [HideInInspector] public int currPosition;

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

    private void LateUpdate()
    {
        //Update customers position every frame
        MoveCustomer();
    }

    public List<ingredientType> CreateCustomerOrder(int minOrderLength, int maxOrderLength) 
    {

        Debug.Log("Created Customer Order");

        // get menu from bench manager
        List<ingredientType> menu = tacoGameManager.benchManager.menu;        
        int orderLength = Random.Range(minOrderLength, maxOrderLength + 1); // randomize order length

        // To be returned
        List<ingredientType> s_order = new List<ingredientType>(orderLength);

        // Fill list with random items from menu
        for (int i = 0; i < orderLength; i++)
        {
            s_order.Add(menu[Random.Range(0, menu.Count)]);
        }

        return s_order;
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
        int numSameIngredients = compareIngredients(tacoToScore);
        int correctPlacementCount = compareIngredientOrder(tacoToScore);


        // [[ BASE CASES ]]

        // if no ingredients in taco, fail
        if (tacoToScore.ingredients.Count == 0) { return scoreType.FAILED; }

        // if more ingredients in taco than order, fail
        if (tacoToScore.ingredients.Count > order.Count + 1) { return scoreType.FAILED; }


        // << PERFECT >> ingredients are the same and order is perfect
        if (numSameIngredients == order.Count && correctPlacementCount == order.Count)
        {
            return scoreType.PERFECT; //perfect score 
        }
        // << GOOD >> ingredients are the same, but order is wrong
        else if (numSameIngredients == order.Count && correctPlacementCount != order.Count)
        {
            return scoreType.GOOD;
        }
        // << OKAY TACO >>  1 missing/extra ingredients, incorrect order
        else if ((numSameIngredients == order.Count - 1 || tacoToScore.ingredients.Count > order.Count) && correctPlacementCount != order.Count)
        {
            return scoreType.OKAY;
        }
        else
        {
            return scoreType.FAILED;
        }


        /*
        //compares the list of ingredients in the taco submitted and the list of ingredients in the customer's order returns the taco's score
        if (currTaco.ingredients.Count == order.Count)
        { //if taco lengths are equal
            if (Taco.ingredientCompare(currTaco.ingredients, order) == currTaco.ingredients.Count)
            { //if # of matching in order ingredients == length of either list
                customerManager.perfectCounter++; //increment perfect counter
                if (customerManager.perfectCounter % 3 == 0) customerManager.s_comboCounter++; //display combo stuff //maybe move elsewhere
                return scoreType.PERFECT; //perfect score 
            }
        }
        List<ingredientType> currTacoSorted = new List<ingredientType>(currTaco.ingredients);
        List<ingredientType> s_orderSorted = new List<ingredientType>(order);
        currTacoSorted.Sort();
        s_orderSorted.Sort();

        //an algorithm to remove duplicates from the submitted / current taco NEEDS TO BE PUT HERE FOR THIS TO WORK

        int numMatching = Taco.ingredientCompare(currTacoSorted, s_orderSorted); //number of matching ingredients (ignoring order)
        //this will need to change depending on how we see duplicates
        int longerLength = (currTaco.ingredients.Count > order.Count) ? currTaco.ingredients.Count : order.Count;
        //definitely a better way to do this next section lmao
        customerManager.perfectCounter = 0;
        if (numMatching - longerLength == 0)
        {
            return scoreType.GOOD;
        }
        else if (numMatching - longerLength == 1)
        {
            return scoreType.OKAY;
        }
        else return scoreType.FAILED;
        */
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

        if (taco.ingredients.Count > order.Count) { Debug.Log("Submitted More Ingredients than order"); }

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
    public void MoveCustomer()
    {
        //Stops moving customer once they are at their new position
        if (currTransitionTime < transitionTime)
        {
            //Math to make the transition ease in and out
            interpolater = currTransitionTime / transitionTime;
            interpolater = interpolater * interpolater * (3f - 2f * interpolater);
            transform.position = Vector3.Lerp(prevPos, targetPos, interpolater);
            currTransitionTime += Time.deltaTime;
        }
    }


}
