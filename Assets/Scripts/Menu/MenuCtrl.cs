using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class MenuCtrl : MonoBehaviour
{
    public GameObject settingsPanel;

    AudioSource audioSource;

    Coroutine settingsCoroutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        //설정창 열기
        settingsPanel.SetActive(true);
        if (settingsCoroutine != null) StopCoroutine(settingsCoroutine);
        settingsCoroutine = StartCoroutine(openAndCloseSettings(true, 1));

    }

    public void CloseSettings()
    {
        if (settingsCoroutine != null) StopCoroutine(settingsCoroutine);
        settingsCoroutine = StartCoroutine(openAndCloseSettings(false, 1, () =>
        {
            settingsPanel.SetActive(false);
        }));

        //설정창 닫기
        //settingsPanel.SetActive(false);
    }

    //Switch를 위한 함수
    public void SwitchIsOn(bool isOn)
    {
        if (isOn == true)
            audioSource.Stop();
        else
            audioSource.Play();
    }

    IEnumerator openAndCloseSettings(bool isOpen, float duration, Action callback = null)
    {
        CanvasGroup canvasGroup = settingsPanel.GetComponent<CanvasGroup>();
        float currentTime = 0;

        //if (isOpen)
        //    settingsPanel.SetActive(true);

        while(currentTime < duration)
        {
            if (isOpen)
                canvasGroup.alpha = Mathf.Lerp(0, 1, currentTime / duration);
            else
                canvasGroup.alpha = Mathf.Lerp(1, 0, currentTime / duration);

            currentTime += Time.deltaTime;
            yield return null;
        }
        //if(callback != null)
        //    callback.Invoke(); //callback이 할당되면 이 시점에 실행

        callback?.Invoke();

        //if (isOpen == false)
        //    settingsPanel.SetActive(false);
    }
}
