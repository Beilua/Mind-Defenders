using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleBarLoading : MonoBehaviour
{
    public Image barFillImage;
    public int nextSceneIndex = 2;
    public float loadSpeed = 0.8f;

    void Start()
    {
        StartCoroutine(LoadGameplay());
    }

    IEnumerator LoadGameplay()
    {
        barFillImage.fillAmount = 0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneIndex);
        
        operation.allowSceneActivation = false; 

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            while (barFillImage.fillAmount < targetProgress)
            {
                barFillImage.fillAmount += loadSpeed * Time.deltaTime;
                yield return null;
            }

            if (operation.progress >= 0.9f && barFillImage.fillAmount >= 0.99f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}