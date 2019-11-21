using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTest : MonoBehaviour
{
    //델리게이트에 연결할 함수의 원형 정의
    //함수의 결과를 변수로 전달하는게 아니라 함수의 내용을 매개변수로 전달할 수 있다.
    delegate void CalNumDelegate(int num);

    //델리게이트 변수 선언
    CalNumDelegate calNum;

    // Start is called before the first frame update
    void Start()
    {
        //calNum 델리게이트 변수에 OnePlusNum 함수 연결
        calNum = OnePlusNum;
        //함수 호출
        //함수 끝에 ()를 붙이는건 함수를 실행하라는 의미이다.
        calNum(4);

        //calNum 델리게이트 변수에 PowerNum 함수 연결
        calNum = PowerNum;
        calNum(5);
    }

    void OnePlusNum(int num)
    {
        int result = num + 1;
        Debug.Log("One Plus = " + result);
    }

    void PowerNum(int num)
    {
        int result = num * num;
        Debug.Log("Power = " + result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
