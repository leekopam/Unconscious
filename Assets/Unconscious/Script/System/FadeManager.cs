using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;
    public GameObject fadeImageOBJ;
    public float fadeTime = 1f;

    public void FadeAndLoadScene(string sceneName)
    {
        if (fadeImageOBJ.activeSelf == false) { fadeImageOBJ.SetActive(true); }
        StartCoroutine(FadeSequence(sceneName));
    }

    private IEnumerator FadeSequence(string sceneName)
    {
        // 페이드 아웃
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // 씬 로드
        SceneManager.LoadScene(sceneName);

        // 페이드 인
        elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }
}