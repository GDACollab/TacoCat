using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI phoneText;
    public bool endOfCutscene;

    //testbubble text variables
    public GameObject alexBubble;
    public GameObject jamieBubble;

    AudioManager audioManager;

    private float startingPosition;

    public float positionX = 1.0f;
    public float positionY = 1.0f;

    

    [Range(0.0f, 10.0f)]
    public float scroll;

    [Tooltip("[WIP] Max charcters per line.\nWon't move the whole word to the next line currently")]
    public int characterLimit;

    public enum character { ALEX, JAMIE };
    [System.Serializable]
    public struct TextList
    {
        public character person;
        public List<string> texts;
    }

    public List<TextList> CutsceneOneDialogue;
    public List<TextList> CutsceneTwoDialogue;
    public List<TextList> CutsceneThreeDialogue;

    private List<TextList> chosenDialogue;

    private TextList list;

    

    [Range(0.0f, 0.5f)]
    public float messageDelayAlex;



    /*[Header("Typing out the message")]
    public bool typeOutAlex;*/

    [Range(0.0f, 0.5f)]
    public float textSpeedAlex;

    
    private int counter;

    [Space]

    [Header("Jamie")]

    [Range(0.0f, 5.0f)]
    public float messageDelayJamie;



    /*[Header("Typing out the message")]
    public bool typeOutJamie;

    [Range(0.0f, 0.5f)]
    public float textSpeedJamie;*/



    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        // clear text in phoneText.text
        phoneText.text = string.Empty;
        
        counter = 0;
        startingPosition = 0;

        //testing
        //Instantiate(alexBubble, alexBubble.transform.position, transform.rotation);
        //Instantiate(jamieBubble, jamieBubble.transform.position, transform.rotation);

        StartCoroutine(begin());

    }
    
    public IEnumerator begin()
    {
        switch (GameManager.instance.cutsceneIndex)
        {
            case 0:
                chosenDialogue = CutsceneOneDialogue;
                GameManager.instance.cutsceneIndex++;
                break;
            case 1:
                chosenDialogue = CutsceneTwoDialogue;
                GameManager.instance.cutsceneIndex++;
                break;
            case 2:
                chosenDialogue = CutsceneThreeDialogue;
                break;
            default:
                chosenDialogue = CutsceneOneDialogue;
                break;
        }

        foreach (TextList a in chosenDialogue)
        {
            if (a.person == 0)
            {
                //Alex


                phoneText.text += "<align=right><b><color=#0000ffff>Alex</b></color> \n";
                yield return StartCoroutine(Typeline(a.texts));
                

            }
            else
            {
                //Jamie

                //slight pause so Jamie doesn't respond instantly
                yield return new WaitForSeconds(messageDelayJamie);

                //scroll all texts
                //increase all y values of instances by scroll

                phoneText.text += "<align=left><b><color=#ff00ffff>Jamie</b></color> \n";
                yield return StartCoroutine(PrintText(a.texts));
               // phoneText.text = "";
            }

        }

        yield return new WaitForSeconds(4);

        endOfCutscene = true;
    }
    

    //for printing the entire message at once
    IEnumerator PrintText(List<string> characterText) {

        //Add each element from phone_texts to phoneText

        foreach (string l in characterText)
        {
            phoneText.text += l + "\n";

            //insert instance of jamiebubble text += l + "\n"
            //GameObject JBubble = Instantiate(jamieBubble, new Vector3(-5,startingPosition,0), Quaternion.identity);
            startingPosition += scroll;
            audioManager.Play(audioManager.recieveTextSFX);
            Debug.Log("ReceiveTextSFX");
            yield return new WaitForSeconds(messageDelayJamie);
        }
            

    }

    //For typing out the message
    
    IEnumerator Typeline(List<string> l)
    {
        foreach (string s in l)
        {

            string[] words = s.Split(" ");

            foreach(string word in words)
            {
                if (counter >= characterLimit)
                {
                    //insert instance of alexbubble text += "\n"
                    phoneText.text += "\n";
                    counter = 0;
                }

                //Add each element from phone_texts to phoneText
                foreach (char c in word)
                {
                    //insert instance of alexbubble text += c
                    phoneText.text += c;

                    counter++;
                    yield return new WaitForSeconds(textSpeedAlex);

                    audioManager.Play(audioManager.typingSFX);
                }
                phoneText.text += " ";
            }

            phoneText.text += "\n";
            counter = 0;



            //insert instance of alexbubble text += "\n"

            //phoneText.text += "\n";
            //phoneText.text = "";

            //scroll all texts
            //increase all y values of instances by scroll
            //GameObject ABubble = Instantiate(alexBubble, new Vector3(5, startingPosition, 0), Quaternion.identity);
            startingPosition += scroll;
            audioManager.Play(audioManager.sendTextSFX);
            yield return new WaitForSeconds(messageDelayAlex);
            
        }
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}
