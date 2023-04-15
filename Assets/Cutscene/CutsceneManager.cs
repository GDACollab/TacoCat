using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI phoneText;


    [Tooltip("[WIP] Max charcters per line.\nWon't move the whole word to the next line currently")]
    public int characterLimit;

    [Space]

    public List<string> phone_texts;
    public List<string> order_of_texts;

    private List<List<string>> master_list;

    [System.Serializable]
    public struct PhoneText
    {
        public List<string> text;
        public string person;
    }
    public List<PhoneText> list;

    [Range(0.0f, 0.5f)]
    public float messageDelayAlex;



    [Header("Typing out the message")]
    public bool typeOutAlex;

    [Range(0.0f, 0.5f)]
    public float textSpeedAlex;

    
    private int counter;

    [Space]

    [Header("Jamie")]

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
        phoneText.text = string.Empty;
        
        counter = 0;

        master_list.Add(order_of_texts);
        master_list.Add(phone_texts);


        //check if typing out the text or printing the whole message
        if (typeOutAlex)
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
        foreach (List<string> line in master_list)
        {
            foreach (string s in line)
            {
                phoneText.text += line + "\n";

                yield return new WaitForSeconds(messageDelayAlex);
            }
            
        }

    }

    //For typing out the message
    IEnumerator Typeline()
    {
        foreach (List<string> line in master_list)
        {
            foreach (string s in line)
            {
                //Add each element from phone_texts to phoneText
                foreach (char c in s.ToCharArray())
                {
                    phoneText.text += c;

                    counter++;

                    yield return new WaitForSeconds(textSpeedAlex);
                    if (counter == characterLimit)
                    {
                        phoneText.text += "\n";
                        counter = 0;
                    }

                }
                phoneText.text += "\n";
                yield return new WaitForSeconds(messageDelayAlex);
            }
        }
    }

    //for printing the entire message at once
    

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
