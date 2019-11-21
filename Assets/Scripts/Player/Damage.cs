using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";

    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;
    //event 특성 : public인데도 객체외부에서 OnPlayerDie를 실행할 수 없다. 읽을 수는 있다
    //그렇기에 주로 알리는 용도로 event를 사용한다.

    private float initHp = 100.0f;
    public float currHp;

    //BloodScreen 텍스처를 저장하기 위한 변수
    public Image bloodScreen;

    //Hp bar Image를 저장하기 위한 변수
    public Image hpBar;

    //생명 게이지의 처음 색상(녹색)
    private readonly Color initColor = new Vector4(0, 1.0f, 0.0f, 1.0f);
    private Color currColor;

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        initHp = GameManager.instance.gameData.hp;
        currHp = GameManager.instance.gameData.hp - currHp;
    }
    // Start is called before the first frame update
    void Start()
    {
        //불러온 데이터 값을 hp에 적용
        initHp = GameManager.instance.gameData.hp;
        currHp = initHp;

        //생명 게이지의 초기 색상을 설정
        hpBar.color = initColor;
        currColor = initColor;
    }

    //충돌한 Collider의 IsTrigger 옵션이 체크 됐을 때 발생
    void OnTriggerEnter ( Collider coll)
    {
        //충돌한 Coliider의 태그가 BULLET이면 Player currHp를 차감
        if (coll.tag == bulletTag) 
        {
            Destroy(coll.gameObject);

            //혈흔 효과를 표현할 코루틴 함수 호출
            StartCoroutine(ShowBloodScreen());
            currHp -= 5.0f;
            Debug.Log("Player Hp" + currHp.ToString());

            //생명 게이지의 색상 및 크기 변경 함수를 호출
            DisplayHpbar();

            //Player의 생명이 0이하이면 사망 처리
            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    void DisplayHpbar()
    {
        //생명 수치가 50%일 때 까지는 녹색에서 노란색으로 변경
        if (currHp / initHp > 0.5f)
            currColor.r = (1 - (currHp / initHp)) * 2.0f;
        else //생명 수치가 0%일 때 까지는 노란색에서 빨강색으로 변경
            currColor.g = (currHp / initHp) * 2.0f;

        //Hpbar의 색상 변경
        hpBar.color = currColor;
        hpBar.fillAmount = (currHp / initHp);
    }

    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스처의 알파값을 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, UnityEngine.Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);

        //BloodScreen 텍스처의 색상을 모두 0으로 변경
        bloodScreen.color = Color.clear;
    }

    //Player의 사망 처리 루틴
    void PlayerDie()
    {
        OnPlayerDie();
        GameManager.instance.isGameOver = true;
        /*
        Debug.Log("PlayerDie !");

        //"ENEMY" 태그로 지정된 모든 적 캐릭터를 추출해 배열에 저장
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        //특정한 태그를 가지고 있는 게임오브젝트를 enemies에 저장
        //FindGameObjectsWithTag : 위험한 코드이지만 객체를 참조할땐 쉬운 방법이므로 위험하지만 자주쓸 수 밖에 없는 코드이다.

        //배열의 처음부터 순회하면서 적 캐릭터의 OnPlayerDie 함수를 호출
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
            //DontRequireReceiver : OnPlayerDie라는 함수가 반환값이 있으면 따로 받지 않겠다
            //SendMessage를 쓰는 이유는 enemyTag가 적에게만 태그가 적용되어있다는 확신이 없을때 쓰게 된다
            //SendMessage로 적이 아닌 드럼통같이 다른애에게 전달시 프로그램이 죽어버릴수 있다.
            
            //EnemyAI enemy = enemies[i].GetComponent<EnemyAI>();
            //enemy.OnPlayerDie();
            //이런 방식의 코드는 barrel에 enemyTag가 있을경우 enemy변수에 null값이 전달되서 오류 및 동작이 멈춘다
        }
        */  
    }
}
