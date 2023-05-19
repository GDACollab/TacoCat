using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CutsceneManager : MonoBehaviour
{
    //public TextMeshProUGUI phoneText;
    public bool endOfCutscene;

    public AudioManager audioManager;

    public float startingPosition;

    //public float positionX = 1.0f;
    //public float positionY = 1.0f;

    //2 QOL

    // text too close to edges
    // bubble size updating late

    //[Range(0.0f, 10.0f)]
    //public float scroll;

    //[Tooltip("[WIP] Max charcters per line.\nWon't move the whole word to the next line currently")]
    //public int characterLimit;

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

    [Range(0.0f, 0.5f)]
    public float messageDelayAlex;

    [Range(0.0f, 0.5f)]
    public float textSpeedAlex;

    [Header("Jamie")]

    [Range(0.0f, 5.0f)]
    public float messageDelayJamie;

    public List<GameObject> currentBubbles = new List<GameObject>();

    public GameObject alexMessagePrefab;
    public Transform alexMessageParent;


    public GameObject jamieMessagePrefab;
    public Transform jamieMessageParent;

    /*[Header("Typing out the message")]
    public bool typeOutJamie;

    [Range(0.0f, 0.5f)]
    public float textSpeedJamie;*/



    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        
        startingPosition = 0;

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

        foreach (TextList textLine in chosenDialogue)
        {
            if (textLine.person == 0)
            {
                // ALEX
                yield return StartCoroutine(AlexText_TypeLine(textLine.texts));
            }
            else
            {
                //Jamie
                //slight pause so Jamie doesn't respond instantly
                yield return new WaitForSeconds(messageDelayJamie);
                yield return StartCoroutine(JamieText_InstantPrint(textLine.texts));
            }
        }

        yield return new WaitForSeconds(4);
        endOfCutscene = true;
    }

    public void MoveBubblesUp(float amount)
    {
        Debug.Log("moved bubbles up by: " + amount);
        foreach (GameObject existingBubble in currentBubbles)
        {
            existingBubble.transform.position += new Vector3(0, amount, 0);
        }
    }

    public IEnumerator AlexText_TypeLine(List<string> l)
    {
        foreach (string s in l)
        {

            GameObject bubble = Instantiate(alexMessagePrefab, alexMessageParent);
            bubble.transform.position = alexMessageParent.position;
            bubble.GetComponent<BubbleManager>().Init(character.ALEX, s, this);

            yield return StartCoroutine(bubble.GetComponent<BubbleManager>().TextCrawl(s));
            currentBubbles.Add(bubble);

            //audioManager.Play(audioManager.sendTextSFX);
            yield return new WaitForSeconds(messageDelayAlex);

        }
    }

    //for printing the entire message at once
    IEnumerator JamieText_InstantPrint(List<string> characterText)
    {
        //Add each element from phone_texts to phoneText
        foreach (string l in characterText)
        {
            //phoneText.text += l + "\n";

            //insert instance of jamiebubble text += l + "\n"
            GameObject bubble = Instantiate(jamieMessagePrefab, jamieMessageParent);
            bubble.GetComponent<BubbleManager>().Init(character.JAMIE, l, this);

            yield return StartCoroutine(bubble.GetComponent<BubbleManager>().InstantTextFill(l));

            currentBubbles.Add(bubble);

            //audioManager.Play(audioManager.recieveTextSFX);
            Debug.Log("ReceiveTextSFX");
            yield return new WaitForSeconds(messageDelayJamie);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
