using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }
    public Image fadeImage;
    public float fadeTime = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(FadeInOnStart());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FadeInOnStart()
    {
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeSequence(sceneName));
    }

    private IEnumerator FadeSequence(string sceneName)
    {
        // ÆäÀÌµå ¾Æ¿ô
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
        StartCoroutine(FadeInOnStart());
    }
}