using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI phoneTextAlex;
    public TextMeshProUGUI phoneTextJamie;


    [Tooltip("Max charcters per line.\nWon't move the whole word to the next line currently")]
    public int characterLimit;

    [Space]


    public List<string> alex_texts;

    [Range(0.0f, 5.0f)]
    public float messageDelayAlex;

    

    [Header("Typing out the message")]
    public bool typeOutAlex;

    [Range(0.0f, 0.5f)]
    public float textSpeedAlex;

    
    private int counter;

    [Space]

    public List<string> jamie_texts;

    [Range(0.0f, 5.0f)]
    public float messageDelayJamie;



    [Header("Typing out the message")]
    public bool typeOutJamie;

    [Range(0.0f, 0.5f)]
    public float textSpeedJamie;



    // Start is called before the first frame update
    void Start()
    {

        // clear text in phoneText.text
        phoneTextAlex.text = string.Empty;
        phoneTextJamie.text = string.Empty;
        counter = 0;


        //check if typing out the text or printing the whole message
        if (typeOutAlex)
        {
            StartCoroutine(TypelineAlex());
        }
        else { 
            StartCoroutine(PrintTextAlex()); 
        }

        //check if typing out the text or printing the whole message
        if (typeOutJamie)
        {
            StartCoroutine(TypelineJamie());
        }
        else
        {
            StartCoroutine(PrintTextJamie());
        }
    }

    //for printing the entire message at once
    IEnumerator PrintTextAlex() {

        //Add each element from phone_texts to phoneText
        foreach (string line in alex_texts)
        {
            phoneTextAlex.text += line + "\n";

            yield return new WaitForSeconds(messageDelayAlex);
        }

    }

    //For typing out the message
    IEnumerator TypelineAlex()
    {
        foreach (string line in alex_texts)
        {
            //Add each element from phone_texts to phoneText
            foreach (char c in line.ToCharArray())
            {
                phoneTextAlex.text += c;

                counter++;

                yield return new WaitForSeconds(textSpeedAlex);
                if (counter == characterLimit) {
                    phoneTextAlex.text += "\n";
                    counter = 0;
                }
                
            }
            phoneTextAlex.text += "\n";
            yield return new WaitForSeconds(messageDelayAlex);
        }
    }

    //for printing the entire message at once
    IEnumerator PrintTextJamie()
    {

        //Add each element from phone_texts to phoneText
        foreach (string line in jamie_texts)
        {
            phoneTextJamie.text += line + "\n";

            yield return new WaitForSeconds(messageDelayJamie);
        }

    }

    //For typing out the message
    IEnumerator TypelineJamie()
    {
        foreach (string line in jamie_texts)
        {
            //Add each element from phone_texts to phoneText
            foreach (char c in line.ToCharArray())
            {
                phoneTextJamie.text += c;

                counter++;

                yield return new WaitForSeconds(textSpeedJamie);
                if (counter == characterLimit)
                {
                    phoneTextJamie.text += "\n";
                    counter = 0;
                }

            }
            phoneTextJamie.text += "\n";
            yield return new WaitForSeconds(messageDelayJamie);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
