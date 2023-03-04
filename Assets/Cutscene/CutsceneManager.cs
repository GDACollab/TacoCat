using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI phoneText;

    public List<string> phone_texts;

    [Range(0.0f, 5.0f)]
    public float messageDelay;

    [Header("Typing out the message")]
    public bool typeOutMessage;
    public float textSpeed;
    //private int index;



    // Start is called before the first frame update
    void Start()
    {

        // clear text in phoneText.text
        phoneText.text = string.Empty;
        //index = 0;

        //check if typing out the text or printing the whole message
        if (typeOutMessage)
        {
            StartCoroutine(Typeline());
        }
        else { 
            StartCoroutine(PrintText()); 
        }
    }

    IEnumerator PrintText() {

        //Add each element from phone_texts to phoneText
        foreach (string line in phone_texts)
        {
            phoneText.text += line + "\n";

            yield return new WaitForSeconds(messageDelay);
        }

    }

    IEnumerator Typeline()
    {
        foreach (string line in phone_texts)
        {
            //Add each element from phone_texts to phoneText
            foreach (char c in line.ToCharArray())
            {
                phoneText.text += c;

                yield return new WaitForSeconds(textSpeed);
            }
            phoneText.text += "\n";
            yield return new WaitForSeconds(messageDelay);
        }
    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
