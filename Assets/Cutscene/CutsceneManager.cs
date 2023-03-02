using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI phoneText;

    public List<string> phone_texts;

    public string first_message = "This should print first.";
    public string second_message = "This should print second.";
    
    

    // Start is called before the first frame update
    void Start()
    {
        phone_texts.Add(first_message);
        phone_texts.Add(second_message);

        // clear text in phoneText.text
        phoneText.text = string.Empty;
        
        //Add each element from phone_texts to phoneText
        foreach (string line in phone_texts) {
            phoneText.text += line + "\n";
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
