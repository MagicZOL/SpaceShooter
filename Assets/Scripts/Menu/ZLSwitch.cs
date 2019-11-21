using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//코드쓰기가 막막하다면 주석을달면서 어떤것들이 필요한지 정리한다
public class ZLSwitch : MonoBehaviour
{
    [SerializeField] RectTransform handleRectTransform;

    bool isOn; //Turn : on, False: off

    public GameObject target;

    AudioSource audioSource;

    Image switchHandle;
    private void Start()
    {
        switchHandle = GetComponent<Image>();
        audioSource = target.GetComponent<AudioSource>();
        isOn = false;
        handleRectTransform.anchoredPosition = new Vector2(-30, 0);
    }
    //스위치 0n/off 동작
    public void OnClickSwitch()
    {
        if (isOn)
        {
            audioSource.Play();
            handleRectTransform.anchoredPosition = new Vector2(-30, 0);
            isOn = false;
        }
        else
        {
            audioSource.Stop();
            handleRectTransform.anchoredPosition = new Vector2(30, 0);
            isOn = true;
        }
        //isOn = (isOn) ? false : true;
        //Debug.Log(isOn);
    }
}
