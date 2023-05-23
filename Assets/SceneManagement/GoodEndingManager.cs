using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoodEndingManager : MonoBehaviour
{
    public Image image;
    public float fadeTime = 1f;
    private float currentAlpha = 1f;

    // Start is called before the first frame update
    void Start()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, currentAlpha);
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene("Credits");
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            currentAlpha = Mathf.Lerp(1f, 0f, timer / fadeTime);

            image.color = new Color(image.color.r, image.color.g, image.color.b, currentAlpha);

            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
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

    // Update is called once per frame
    void Update()
    {

    }
}
