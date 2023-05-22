using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAnimator : MonoBehaviour
{

    Customer customer;
    public List<GameObject> customerRigs;
    public Animator currAnimator;

    // check if new customer, then wave
    public bool hasOrdered;

    public void Start()
    {
        customer = GetComponent<Customer>();

        // turns off all rigs at start
        foreach (GameObject customer in customerRigs)
        {
            customer.SetActive(false);
        }

    }

    public void ChooseSpeciesRig(CUST_SPECIES species)
    {
        foreach (GameObject cust in customerRigs)
        {
            if (cust.GetComponent<RigSpecies>().species == species)
            {
                currAnimator = cust.GetComponent<Animator>();
                currAnimator.gameObject.SetActive(true);
                return;
            }
        }
    }

    private void Update()
    {
        if (currAnimator)
        {
            currAnimator.gameObject.SetActive(true);

            // update the ordered bool, which is set in the customer manager when the customer becomes current
            currAnimator.SetBool("hasOrdered", hasOrdered);
        }
        else if (customer)
        {
            ChooseSpeciesRig(customer.species);
        }
        else
        {
            Debug.LogError("Cannot find Customer or Animator", this.gameObject);
        }
    }
}
