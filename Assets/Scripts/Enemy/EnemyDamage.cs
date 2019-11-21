using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    
    //생명 게이지
    private float hp = 100.0f;
    //초기 생명 수치
    private float initHp = 100.0f;
    //피격 시 사용할 혈흔 효과
    private GameObject bloodEffect;

    //생명 게이지 프리팹을 저장할 변수
    public GameObject hpBarPrefab;
    //생명 게이지의 위치를 보정할 오프셋
    public Vector3 hpBarOffset = new Vector3(0, 2.2f, 0);
    //부모가 될 Canvas 객체
    private Canvas uiCanvas;
    //생명 수치에 따라 fillAmout 속성을 변경할 Image
    private Image hpBarImage;

    public EnemyAI enemyAI;
    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        //혈흔 효과 프리팹을 로드
        //프리팹은 정해진 프리팹이 할당되기때문에 실행하면 절대 바꿀수 없지만
        //파일은 실행중에도 이름만 같다면 다른이펙트로 대체가 가능하다
        //Resources는 어떤 타입이라도 할당이 가능하다.
        //Resources.Load는 Resources폴더가 있어야 하며 폴더안에 불러올 것이 있어야한다.
        //프로젝트파일이 많다고 다 실행되는것은 아니지만 Resources 폴더에 있는것들은 무조건 실행파일이 되서 관리를 잘 해야한다.
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");

        //생명 게이지의 생성 및 초기화
        SetHpBar();
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        //UI Canvas 하위로 생명 게이지를 생성
        GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform);
        //fillAmout 속성을 변경할 Image를 추출
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        //생명 게이지가 따라가야 할 대상과 오프셋 값 설정
        var _hpBar = hpBar.GetComponent<EnemyHpBar>();
        _hpBar.targetTr = this.gameObject.transform;
        _hpBar.offset = hpBarOffset;
    }
    private void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == bulletTag )
        {
            //혈흔 효과를 생성하는 함수 호출
            ShowBloodEffect(coll);

            //총알 삭제
            //Destroy(coll.gameObject);
            coll.gameObject.SetActive(false);

            //생명 게이지 차감
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            //생명 게이지의 fillAmout 속성을 변경
            hpBarImage.fillAmount = hp / initHp;

            if(hp <= 0.0f)
            {
                //적 캐릭터의 상태를 DIE로 변경
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;

                //적 캐릭터가 사망한 이후 생명 게이지를 투명 처리
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;

                //적 캐릭터의 사망 횟수를 누적시키는 함수 호출
                GameManager.instance.InKillCount();
                //Capsule collider 컴포넌트를 비활성화
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }

    void ShowBloodEffect(Collision coll)
    {
        //총알이 충돌한 지점 산출
        Vector3 pos = coll.contacts[0].point;
        //총알의 충돌했을 때의 법선 벡터
        Vector3 _normal = coll.contacts[0].normal;
        //총알의 충돌 시 방향 벡터의 회전값 계산
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        //혈흔 효과 생성
        //파일에서 불러온다
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
}