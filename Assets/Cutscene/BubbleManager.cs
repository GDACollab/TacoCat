using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BubbleManager : MonoBehaviour
{
    public CutsceneManager cutsceneManager;

    public TextMeshProUGUI messageText;
    public Image backgroundImage;
    public Image tickImage;
    public string textContents;

    public CutsceneManager.character characterType;

    public List<Sprite> backgroundOptions;
    public List<Sprite> tickOptions;

    public float bubbleVerticalSize;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(CutsceneManager.character type, string text, CutsceneManager manager)
    {
        characterType = type;
        textContents = text;
        cutsceneManager = manager;

        UpdateVisuals();
    }

    public void UpdateVisuals() 
    {
        //messageText.text = textContents;
        if (characterType == CutsceneManager.character.ALEX)
        {
            backgroundImage.sprite = backgroundOptions[0];
            tickImage.sprite = tickOptions[0];
        }
        else
        {
            backgroundImage.sprite = backgroundOptions[1];
            tickImage.sprite = tickOptions[1];
        }

        

    }

    public void UpdateBubbleHeight()
    {
        //Debug.Log("update Bubble Height");
        int lineCount = messageText.textInfo.lineCount;
        backgroundImage.transform.localScale = new Vector3(1, lineCount * 0.25f, 1);
        float oldBubbleHeight = bubbleVerticalSize;
        bubbleVerticalSize = lineCount * 0.25f * 4;

        if (bubbleVerticalSize != oldBubbleHeight)
        {
            cutsceneManager.MoveBubblesUp(bubbleVerticalSize - oldBubbleHeight);
        }
    }

    
    public IEnumerator InstantTextFill(string s)
    {
        messageText.text = s;
        yield return new WaitForSeconds(0.1f);
        UpdateBubbleHeight();
    }

    //For typing out the message

    public IEnumerator TextCrawl(string s)
    {
        string[] words = s.Split(" ");

            foreach (string word in words)
            {

                //Add each element from phone_texts to phoneText
                foreach (char c in word)
                {
                    //insert instance of alexbubble text += c
                    messageText.text += c;
                    UpdateBubbleHeight();
                    yield return new WaitForSeconds(cutsceneManager.textSpeedAlex);

                    cutsceneManager.audioManager.Play(cutsceneManager.audioManager.typingSFX);
                }
                messageText.text += " ";
            }

    }
    
}
