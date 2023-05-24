using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    //public TextMeshProUGUI phoneText;
    public bool endOfCutscene;

    public AudioManager audioManager;

    public float startingPosition;

    public float scrollFactor;

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
    public List<TextList> GoodEndingDialogue;
    public List<TextList> BadEndingDialogue;

    private List<TextList> chosenDialogue;

    public float unskipableDelay;

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

    public RectTransform jamieCallsAlexObject;

    public Image image;
    public float fadeTime = 1f;
    private float currentAlpha = 0f;

    /*[Header("Typing out the message")]
    public bool typeOutJamie;

    [Range(0.0f, 0.5f)]
    public float textSpeedJamie;*/



    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        
        startingPosition = 0;

        image.color = new Color(image.color.r, image.color.g, image.color.b, currentAlpha);

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
                GameManager.instance.cutsceneIndex++;
                break;
            case 3:
                chosenDialogue = GoodEndingDialogue;
                break;
            case 4:
                chosenDialogue = BadEndingDialogue;
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
                //yield return new WaitForSeconds(unskipableDelay);
                yield return StartCoroutine(AlexText_TypeLine(textLine.texts));
            }
            else
            {
                //Jamie
                //slight pause so Jamie doesn't respond instantly

                
                //yield return jamieCountdown();
                //yield return new WaitForSeconds(unskipableDelay);
                yield return StartCoroutine(JamieText_InstantPrint(textLine.texts));
            }
        }

        yield return new WaitForSeconds(3);

        if (chosenDialogue != GoodEndingDialogue && chosenDialogue != BadEndingDialogue)
        {
            endOfCutscene = true;
        }
        else if (chosenDialogue == GoodEndingDialogue)
        {
            RectTransform rectTransform = jamieCallsAlexObject.GetComponent<RectTransform>();
            Vector3 startPosition = rectTransform.anchoredPosition3D;
            Vector3 targetPosition = new Vector3(-360, 0, 0);
            float duration = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                rectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.anchoredPosition3D = targetPosition;

            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeOut());
            yield return new WaitForSeconds(fadeTime);
            SceneManager.LoadScene("GoodEnding");
        }
        else if (chosenDialogue == BadEndingDialogue)
        {
            StartCoroutine(FadeOut());
            yield return new WaitForSeconds(fadeTime);
            SceneManager.LoadScene("BadEnding");
        }
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            currentAlpha = Mathf.Lerp(0f, 1f, timer / fadeTime);

            image.color = new Color(image.color.r, image.color.g, image.color.b, currentAlpha);

            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
    }


    IEnumerator jamieCountdown()
    {
        for (float timer = messageDelayJamie; timer >= 0; timer -= Time.deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yield break;
            }
            yield return null;
        }

    }

    IEnumerator alexCountdown()
    {
        for (float timer = messageDelayAlex; timer >= 0; timer -= Time.deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yield break;
            }
            yield return null;
        }

    }



    public void MoveBubblesUp(float amount)
    {
        amount *= scrollFactor;
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
            
            yield return alexCountdown();
            yield return new WaitForSeconds(unskipableDelay);

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
            
            yield return jamieCountdown();
            yield return new WaitForSeconds(unskipableDelay);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
