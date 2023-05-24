using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{

    public LightingManager lightManager;
    public TIME_OF_DAY currentDayCycle;
    public float fadeDuration;
    [Header("List of Assets")]
    public List<GameObject> morning;
    public List<GameObject> midday;
    public List<GameObject> evening;
    public List<GameObject> night;
   
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        currentDayCycle = lightManager.dayCycleState;
        Debug.Log(lightManager.dayCycleState);
        switch (currentDayCycle)
        {
            case TIME_OF_DAY.NIGHT:
                foreach (GameObject obj in evening) //fade out
                {

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    Color newColor = sr.color;
                    newColor.a = Mathf.Lerp(sr.color.a, 0f, 2f * Time.deltaTime);
                    sr.color = newColor;
                }
                foreach (GameObject obj in night) // fade in
                {

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    Color newColor = sr.color;
                    newColor.a = Mathf.Lerp(sr.color.a, 1f, 2f * Time.deltaTime);
                    sr.color = newColor;
                }
                break;
            case TIME_OF_DAY.EVENING:
                foreach (GameObject obj in midday) //fade out
                {

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    Color newColor = sr.color;
                    newColor.a = Mathf.Lerp(sr.color.a, 0f, 2f * Time.deltaTime);
                    sr.color = newColor;
                }
                foreach (GameObject obj in evening) // fade in
                {

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    Color newColor = sr.color;
                    newColor.a = Mathf.Lerp(sr.color.a, 1f, 2f * Time.deltaTime);
                    sr.color = newColor;
                }
                break;
            case TIME_OF_DAY.MIDDAY:
                foreach (GameObject obj in morning) //fade out
                {

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    Color newColor = sr.color;
                    newColor.a = Mathf.Lerp(sr.color.a, 0f, 2f * Time.deltaTime);
                    sr.color = newColor;
                }
                foreach (GameObject obj in midday) // fade in
                {
                    
                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    Color newColor = sr.color;
                    newColor.a = Mathf.Lerp(sr.color.a, 1f, 2f * Time.deltaTime);
                    sr.color = newColor;
                }
                //Mathf.Lerp(sunLight.intensity, curSunIntensity, lightColorAdjustSpeed * Time.deltaTime);
                break;
            case TIME_OF_DAY.MORNING:
                foreach (GameObject obj in night) //fade out
                {

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    Color newColor = sr.color;
                    newColor.a = Mathf.Lerp(sr.color.a, 0f, 2f * Time.deltaTime);
                    sr.color = newColor;
                }
                foreach (GameObject obj in morning) // fade in
                {

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    Color newColor = sr.color;
                    newColor.a = Mathf.Lerp(sr.color.a, 1f, 2f * Time.deltaTime);
                    sr.color = newColor;
                }
                break;
        }
    }
        
}
