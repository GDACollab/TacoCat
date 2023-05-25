using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraEffectManager : MonoBehaviour
{
    public Image blackFadeImage;
    public bool isFading;

    private void Start()
    {
        blackFadeImage.gameObject.SetActive(true);
    }

    public void StartFadeOut(float duration = 3)
    {
        StartCoroutine(Fade(1, duration));
    }

    public void StartFadeIn(float duration = 3)
    {
        StartCoroutine(Fade(0, duration));
    }

    IEnumerator Fade(float alpha, float duration)
    {
        if (!isFading)
        {

            isFading = true;
            float elapsedTime = 0f;
            Color startColor = blackFadeImage.color;
            Color targetColor = new Color(0f, 0f, 0f, alpha);

            while (elapsedTime < duration)
            {
                // Calculate the current alpha value based on the elapsed time and duration
                float normalizedTime = elapsedTime / duration;
                blackFadeImage.color = Color.Lerp(startColor, targetColor, normalizedTime);

                // if target reached, break out of loop
                if (blackFadeImage.color.a == alpha) { 
                    elapsedTime = duration;
                    isFading = false;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            blackFadeImage.color = targetColor;

            isFading = false;
        }

    }
}
