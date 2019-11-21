using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GYSwitch : MonoBehaviour
{
    [SerializeField] RectTransform handleRectTransform;

    public MenuCtrl menuCtrl;

    bool isOn; //Turn : on, False: off

    private void Start()
    {
        isOn = false;
        handleRectTransform.anchoredPosition = new Vector2(-30, 0);
        menuCtrl.SwitchIsOn(isOn);
    }
    //스위치 0n/off 동작
    public void OnClickSwitch()
    {
        if (isOn)
        {
            isOn = false;
            handleRectTransform.anchoredPosition = new Vector2(-30, 0);
        }
        else
        {
            isOn = true;
            handleRectTransform.anchoredPosition = new Vector2(30, 0);
        }
        menuCtrl.SwitchIsOn(isOn);
    }
}
