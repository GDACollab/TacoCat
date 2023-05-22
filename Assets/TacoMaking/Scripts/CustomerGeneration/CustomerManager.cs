using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomerManager : MonoBehaviour
{
    
    [HideInInspector]
    public TacoMakingGameManager tacoGameManager;

    [Header("Customer Vars")]
    public GameObject customerPrefab;
    public Customer currCustomer;
    public float transitionTime;       //How long it takes in seconds for the customer to transition between positions
    [SerializeField] private float transitionDelay; //The most that a customers transition time can be randomly offset (used to make customers move at diff speeds)
    public List<Transform> positionList = new List<Transform>(); //Used as the points the customer transitions to/from 
    public List<Customer> customerList = new List<Customer>();

    [Header("Dialogue Vars")]
    [SerializeField] private float dialogueDelay;
    private float dialogueDelayTime = 0;
    [SerializeField] private float dialogueChance;

    [Header("Misc")]
    [HideInInspector] public int difficulty = 1;
    


    //before calling check if customers left to generate == 0
    private void Start()
    {
        tacoGameManager = GetComponentInParent<TacoMakingGameManager>();
    }

    private void FixedUpdate()
    {
        //Updates the customer positions when the current customer gets removed
        if (currCustomer == null && customerList.Count > 0)
        {
            UpdateCustomers();
        }
    }

    public GameObject CreateNewCustomer(int customer_number)
    {
        Debug.Log("Created Customer");

        //Creates a new customer and sets all of its vars
        Customer customerScript = Instantiate(customerPrefab, transform).GetComponent<Customer>();
        customerScript.transform.position = positionList[5].position;
        customerScript.transitionTime = transitionTime;
        customerScript.currPosition = -1;
        customerScript.difficulty = difficulty;
        //Temp for testing
        if (Random.Range(0f, 1f) < dialogueChance)
        { 
            customerScript.hasEndingDialogue = true;
        }
        
        customerScript.hasIntroDialgue = false;

        customerList.Add(customerScript);
        UpdateCustomers();

        return currCustomer.gameObject;
    }

    //Member used to remove the current customer
    public void RemoveCurrentCustomer(scoreType tacoScore)
    {
        //If there is a current customer, then starts its transition out of frame
        if (currCustomer != null)
        {
            //Delays the destruction of the customer so that they have time to move offscreen
            Vector3 endPosition = positionList[0].position;
            endPosition.y = Random.Range(-5.0f, 2.8f);
            
            currCustomer.transitionOffset = 0;   
            if (customerList[0].hasEndingDialogue)
            {
                //currCustomer.dialoguePause = dialogueDelay;
                //dialogueDelayTime = dialogueDelay;
                currCustomer.GetComponent<CustomerDialogue>().CreateDialogue(currCustomer, tacoScore);
                currCustomer.transitionTime = currCustomer.transitionTime * 2;
            }
            currCustomer.MoveCustomer(endPosition);
            //Destroy(currCustomer.gameObject, transitionTime + currCustomer.dialoguePause);
            Destroy(currCustomer.gameObject, currCustomer.transitionTime);
            customerList.RemoveAt(0);
            currCustomer = null;
        }
    }

    //Member to update the positions of all the customers in line
    private void UpdateCustomers()
    {
        //Assign the new current customer when the current one gets removed
        if (currCustomer == null && customerList.Count > 0)
        {
            currCustomer = customerList[0];

            //tacoAudioManager.OrderAudio(); //needs to be edited later
        }

        //Iterate through all the customers and check if their position needs to be updated
        for (int i = 0; i < customerList.Count; i++)
        {
            //If the customers current position has changed, then update its variables
            if (customerList[i].currPosition != i)
            {
                //Adds a delay to subsequent customers being moved so that they don't all move at the same exact time              
                customerList[i].transitionOffset = (i + 1) * transitionDelay;
                customerList[i].dialoguePause = dialogueDelayTime;
                customerList[i].currPosition = i;
                customerList[i].MoveCustomer(positionList[i + 1].position);
            }
        }
        dialogueDelayTime = 0;
    }

    // spawn a bunch of customers at once to debug order generation
    public void DebugSpawnCustomers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newCustomer = CreateNewCustomer(i);
            newCustomer.transform.position += new Vector3(i, 0, 0);
        }
    }
}
