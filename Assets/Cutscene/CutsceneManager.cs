using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI phoneText;
    public bool endOfCutscene;

    [Space(10)]
    public List<string> phone_texts;

    [Range(0.0f, 5.0f)]
    public float messageDelay;

    

    [Header("Typing out the message")]
    public bool typeOutMessage;

    [Range(0.0f, 0.5f)]
    public float textSpeed;

    [Tooltip("Max charcters per line.\nWon't move the whole word to the next line currently")]
    public int characterLimit;

    private int counter;



    // Start is called before the first frame update
    void Start()
    {

        // clear text in phoneText.text
        phoneText.text = string.Empty;
        counter = 0;

        //check if typing out the text or printing the whole message
        if (typeOutMessage)
        {
            StartCoroutine(Typeline());
        }
        else { 
            StartCoroutine(PrintText()); 
        }
    }

    //for printing the entire message at once
    IEnumerator PrintText() {

        //Add each element from phone_texts to phoneText
        foreach (string line in phone_texts)
        {
            phoneText.text += line + "\n";

            yield return new WaitForSeconds(messageDelay);
        }

        endOfCutscene = true;

    }

    //For typing out the message
    IEnumerator Typeline()
    {
        foreach (string line in phone_texts)
        {
            //Add each element from phone_texts to phoneText
            foreach (char c in line.ToCharArray())
            {
                phoneText.text += c;

                counter++;

                yield return new WaitForSeconds(textSpeed);
                if (counter == characterLimit) {
                    phoneText.text += "\n";
                    counter = 0;
                }
                //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Cutscene/Texting");
            }
            phoneText.text += "\n";
            yield return new WaitForSeconds(messageDelay);
        }

        endOfCutscene = true;
    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
