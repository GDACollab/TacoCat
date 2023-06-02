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

    public bool skip = false;

    public float lineHeight;

    public string fontAssetName = "TacocatMorganFont-Regular_1 SDF";

    public float bubbleVerticalSize;

    //public float alexTextCrawlCountdown = CutsceneManager.textSpeedAlex;

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

    }

    private TMP_FontAsset FindFontAsset(string fontName)
    {
        TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();

        foreach (TMP_FontAsset font in fonts)
        {
            if (font.name == fontName)
            {
                return font;
            }
        }
        return null;
    }

    public void UpdateBubbleHeight()
    {
        //backgroundImage.transform.localScale = new Vector3(1, Mathf.Max(0.38f, (bubbleVerticalSize) + 0.05f), 1);
        //bubbleVerticalSize += lineHeight;

        if (characterType == CutsceneManager.character.ALEX)
        {
            cutsceneManager.MoveBubblesUp(bubbleVerticalSize);
        }
        else if (characterType == CutsceneManager.character.JAMIE)
        {
            cutsceneManager.MoveBubblesUp(bubbleVerticalSize);
        }
    }
    
    public IEnumerator InstantTextFill(string s)
    {
        messageText.text = s;
        yield return new WaitForSeconds(0.01f);
        UpdateBubbleHeight();
    }

    //For typing out the message
    public IEnumerator TextCrawl(string s)
    {
        if(cutsceneManager.chosenDialogue == cutsceneManager.credits){
            skip = true;
        }else{
            skip = false;
        }
        
        string[] words = s.Split(" ");
        //float waitTime = 0.1f;
        if(skip){
            messageText.text = s;
            yield return new WaitForSeconds(0.01f);
            UpdateBubbleHeight();
        }
        else{
            foreach (string word in words)
            {
                //Add each element from phone_texts to phoneText
                foreach (char c in word)
                {

                    messageText.text += c;
                    yield return alexCrawlCountdown();
                    if(!skip){
                        cutsceneManager.audioManager.Play(cutsceneManager.audioManager.typingSFX);
                    }
                }
                messageText.text += " ";
                UpdateBubbleHeight();
                
            }
        }
        //waitTime = 0.1f;
        skip = false;
    }

   

    IEnumerator alexCrawlCountdown()
    {
        float skipAmount = cutsceneManager.textSpeedAlex;
        
        if (skip == true)
        {
            skipAmount = 0f;
        }
        //Debug.Log("new alexCrawlCountdown" + skip + skipAmount);
        if (Input.GetKey(KeyCode.Space))
        {
                skip = true;
                //Debug.Log("hit space, skip value: " + skip + skipAmount);
                //cutsceneManager.audioManager.Play(cutsceneManager.audioManager.skipSFX);
                yield break;
        }
        yield return new WaitForSeconds(skipAmount);

        yield return null;
    }
}
