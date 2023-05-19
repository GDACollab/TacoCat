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

    public float lineHeight;

    public string fontAssetName = "TacocatMorganFont-Regular_1 SDF";

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
        // Set the font!
        TMP_FontAsset fontAsset = FindFontAsset(fontAssetName);

        if (fontAsset != null)
        {
            messageText.font = fontAsset;
        }
        else
        {
            Debug.LogError("Font asset not found: " + fontAssetName);
        }


        // damn it was getting tense right here , just you and the bubbles - mortal enemies
        //messageText.fontSize = 15;

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
        bubbleVerticalSize += lineHeight;

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
                yield return new WaitForSeconds(cutsceneManager.textSpeedAlex);

                //cutsceneManager.audioManager.Play(cutsceneManager.audioManager.typingSFX);
            }
            messageText.text += " ";
        }

        UpdateBubbleHeight();
    }

}
