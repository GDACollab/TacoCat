using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CustomerDialogue : MonoBehaviour
{
    [SerializeField] private List<string> capybaraBarks;
    [SerializeField] private List<string> frogBarks;
    [SerializeField] private List<string> ravenBarks;
    [SerializeField] private List<string> sheepBarks;
    [SerializeField] private List<string> fishBarks;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textMesh;

    public void CreateDialogue(Customer customer, scoreType score)
    {
        textMesh = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = PickDialogue(customer.customerSpecies, score);
        Debug.Log(customer.customerSpecies);
        dialogueBox.SetActive(true);
    }

    private string PickDialogue(Customer.species species, scoreType score)
    {
        switch (species)
        {
            case Customer.species.Capybara:
                return capybaraBarks[Random.Range(0, capybaraBarks.Count - 1)];

            case Customer.species.Frog:
                return frogBarks[Random.Range(0, frogBarks.Count - 1)];

            case Customer.species.Raven:
                return ravenBarks[Random.Range(0, ravenBarks.Count - 1)];

            case Customer.species.Sheep:
                return sheepBarks[Random.Range(0, sheepBarks.Count - 1)];

            case Customer.species.Fish:
                return fishBarks[Random.Range(0, fishBarks.Count - 1)];

            default:
                return "Thanks!";
        }
    }
}
