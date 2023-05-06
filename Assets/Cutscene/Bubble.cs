using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bubble : MonoBehaviour
{
    public SpriteRenderer mySpriteRenderer;

    public Sprite alexBubbleSprite;
    public Sprite jamieBubbleSprite;
    public Sprite alexBubbleTickSprite;
    public Sprite jamieBubbleTickSprite;

    [Space(1)]
    public float scaleX = 1.0f;
    public float scaleY = 1.0f;

    [Space(1)]
    [Tooltip("X position of the center of the bounding box containing the bubble and the bubble tick.")]
    public float positionX = 1.0f;
    [Tooltip("Y position of the center of the bounding box containing the bubble and the bubble tick.")]
    public float positionY = 1.0f;

    public enum CHARACTERS {ALEX, JAMIE};

    [Tooltip("Padding from the left or right side of the screen. Alex's bubbles are padded from the right side and Jamie's bubbles are padded from the left.")]
    public float leftOrRightPadding = 0.0f;

    [Tooltip("The TextMeshPro object that will be copied for each instance of this prefab. Needed to determine the height of the text.")]
    public TextMeshProUGUI textMeshProUGUIToClone;

    // The clone of textMeshProUGUIToClone
    private TextMeshProUGUI s_myTextMeshProUGUI;

    // Call this method right after instantiating this object instead of using this object's constructor.
    public void Init(
        CHARACTERS character, // Value of ALEX or JAMIE
        float positionY, // Y position in screen space
        Canvas canvasToUse, // The canvas to put this bubble and its text on
        string text = "Text wasn't passed to this text bubble!" // The text to put inside the bubble
        )
    {
        this.transform.SetParent(canvasToUse.transform);

        // Make a clone of the TMPUGUI object
        // Note: The TMPGUI object needs to be a child of a Canvas element in the scene to successfully render text.
        s_myTextMeshProUGUI = Instantiate(textMeshProUGUIToClone, canvasToUse.transform);

        // Set properties in the newly created TMPGUI object
        // TODO: FINISH THIS
        s_myTextMeshProUGUI.text = text;

        // Scale bubble based on size of text inside the bubble
        // mySpriteRenderer.transform.localScale can be used to resize the sprite
        // TODO: FINISH THIS

        if (character == CHARACTERS.ALEX)
        {
            mySpriteRenderer.sprite = alexBubbleSprite;

            // Move bubble to right side of screen
            // TODO: FINISH THIS
        }
        else if (character == CHARACTERS.JAMIE)
        {
            mySpriteRenderer.sprite = jamieBubbleSprite;

            // Move bubble to left side of screen
            // TODO: FINISH THIS
        }

        // Set y coordinate of the position.
        // Make this y coordinate of the center of the bounding box containing the box and the box tick.
        // TODO: FINISH THIS
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Determines how many lines of non-whitespace characters there are in a string of text
    int GetNumberOfLinesInText(string text)
    {
        return text.Split('\n', '\r').Length;
    }

    // Given the additional height required for the text, return the height scale factor needed for the bubble to contain the text.
    // TODO: FINISH THIS
    float CalculateScaleFactor(float additionalHeight)
    {
        return 1.0f;
    }

    // Returns a Vector3 representing the corresponding canvas space point for this rectangle of a given screen space point.
    // The z component of the returned vector will be 0.
    Vector3 ScreenSpaceToCanvasSpace(Vector2 screenSpacePoint)
    {
        Vector2 result = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, screenSpacePoint, Camera.main, out result);
        return new Vector3(result.x, result.y, 0);
    }
}
