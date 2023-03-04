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

    



    // Start is called before the first frame update
    void Start()
    {

        // clear text in phoneText.text
        phoneText.text = string.Empty;

        StartCoroutine(Type());
    }

        IEnumerator Type() {

            //Add each element from phone_texts to phoneText
            foreach (string line in phone_texts)
            {
                phoneText.text += line + "\n";

                yield return new WaitForSeconds(messageDelay);
            }

        }
        

        
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
