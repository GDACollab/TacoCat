using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CustomerDialogue : MonoBehaviour
{
    [Header("Neutral Barks")]
    [SerializeField] private List<string> capybaraBarksNeutral;
    [SerializeField] private List<string> frogBarksNeutral;
    [SerializeField] private List<string> ravenBarksNeutral;
    [SerializeField] private List<string> sheepBarksNeutral;
    [SerializeField] private List<string> fishBarksNeutral;
    [Header("Good Barks")]
    [SerializeField] private List<string> capybaraBarksGood;
    [SerializeField] private List<string> frogBarksGood;
    [SerializeField] private List<string> ravenBarksGood;
    [SerializeField] private List<string> sheepBarksGood;
    [SerializeField] private List<string> fishBarksGood;
    [Header("Bad Barks")]
    [SerializeField] private List<string> capybaraBarksBad;
    [SerializeField] private List<string> frogBarksBad;
    [SerializeField] private List<string> ravenBarksBad;
    [SerializeField] private List<string> sheepBarksBad;
    [SerializeField] private List<string> fishBarksBad;
    [Header("Dialouge Box")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textMesh;

    public void CreateDialogue(Customer customer, scoreType score)
    {
        //textMesh = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = PickDialogue(customer.custSpecies, score);
        Debug.Log(customer.custSpecies);
        dialogueBox.SetActive(true);
    }

    private string PickDialogue(Customer.species species, scoreType score)
    {
    
        switch (species)
        {
            case Customer.species.Capybara:
                if (score == scoreType.PERFECT) { return capybaraBarksGood[Random.Range(0, capybaraBarksGood.Count - 1)];}
                else if (score == scoreType.FAILED) {return capybaraBarksBad[Random.Range(0, capybaraBarksBad.Count - 1)]; }
                else { return capybaraBarksNeutral[Random.Range(0, capybaraBarksNeutral.Count - 1)]; }

            case Customer.species.Frog:
                if (score == scoreType.PERFECT) { return frogBarksGood[Random.Range(0, frogBarksGood.Count - 1)];}
                else if (score == scoreType.FAILED) {return frogBarksBad[Random.Range(0, frogBarksBad.Count - 1)]; }
                else { return frogBarksNeutral[Random.Range(0, frogBarksNeutral.Count - 1)]; }

            case Customer.species.Raven:
                if (score == scoreType.PERFECT) { return ravenBarksGood[Random.Range(0, ravenBarksGood.Count - 1)];}
                else if (score == scoreType.FAILED) {return ravenBarksBad[Random.Range(0, ravenBarksBad.Count - 1)]; }
                else { return ravenBarksNeutral[Random.Range(0, ravenBarksNeutral.Count - 1)]; }

            case Customer.species.Sheep:
                if (score == scoreType.PERFECT) { return sheepBarksGood[Random.Range(0, sheepBarksGood.Count - 1)];}
                else if (score == scoreType.FAILED) {return sheepBarksBad[Random.Range(0, sheepBarksBad.Count - 1)]; }
                else { return sheepBarksNeutral[Random.Range(0, sheepBarksNeutral.Count - 1)]; }

            case Customer.species.Fish:
                if (score == scoreType.PERFECT) { return fishBarksGood[Random.Range(0, fishBarksGood.Count - 1)];}
                else if (score == scoreType.FAILED) {return fishBarksBad[Random.Range(0, fishBarksBad.Count - 1)]; }
                else { return fishBarksNeutral[Random.Range(0, fishBarksNeutral.Count - 1)]; }

            default:
                return "Thanks!";
        }
    }
}
