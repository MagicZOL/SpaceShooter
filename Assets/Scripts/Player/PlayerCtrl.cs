using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//클래스는 System.Serializable 어트리뷰트를 명시해야 인스펙터 뷰에 노출됨
[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    private Transform tr;

    [SerializeField] // private이지만 유니티에서만 접근이 가능하게 만들어준다. 자주 사용하기 떄문에 습관을 들여주면 좋다.
    private float moveSpeed = 10.0f;

    //회전속도 변수
    [SerializeField] private float rotSpeed = 80.0f;

    //인스펙터 뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;

    //Animation 컴포넌트를 저장하기 위한 변수
    public Animation anim;

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        moveSpeed = GameManager.instance.gameData.speed;
    }
    // Start is called before the first frame update
    void Start()
    {
        //트랜스폼 캐싱 : 미리 불러와서 가져다쓴다
        tr = GetComponent<Transform>();

        //Animation 컴포넌트를 변수에 할당
        anim = GetComponent<Animation>();

        //Animation컴포넌트의 애니메이션 클립을 지정하고 실행
        anim.clip = playerAnim.idle;
        anim.Play();

        //불러온 데이터 값을 moveSpeed에 적용
        moveSpeed = GameManager.instance.gameData.speed;
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");


        //Debug.Log("h=" + h.ToString());
        //Debug.Log("v=" + v.ToString());
        
        /*
        if(v<0)
        {
            moveSpeed = 5.0f;
        }
        else
        {
            moveSpeed = 10.0f;
        }
        */
        /*
        //전후좌우 이동 방향 벡터 계산
        1.Up
        2.Down
        3.Left
        4.Right
        5.Up/Left
        6.Up/Right
        7.Down/Left
        8.Down/Right
        */
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h); //Vector3.forward : (0,0,1), Vector3.right(1,0,0)
        //이동하기
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);
        /*
        transform.Translate로 선언하지 않는 이유는 느려서이다.
        tr.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);
        Translate(어떤 방향으로 얼마큼 갈것이냐 방향과거리선언, 
        Vector3.forward = Vector3(0,0,1) : 앞쪽으로 1유닛만큼 가라
        * moveSpeed : 매 프레임마다 10유닛씩 가라
        v : 0,-1,1 : 0 버튼 안눌렀으면 값이 0 이니까 가지말라, 1 : +10유닛이동 앞으로가게됨, -1:-10유닛이동 뒤로가라
        deltaTime : 현재프레임과 전 프레임사이의 시간 * 30분의1 (1초당 30프레임을 보통 호출하므로)
        빠르게 10유닛씩 이동하지 않는 이유는 바로 1로 되는것이 아닌 소수점을 지나 1값이 나오므로
        Space.Self : 방향의 기준이 캐릭터 좌표이다
        */
        if (v >= 0.1f)
        {
            anim.CrossFade(playerAnim.runF.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f) 
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);
        }
        else
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
        /*
        //테스트
        if(v>0)
        {
            anim.Play("RunF");
        }
        else if(v<0)
        {
            anim.Play("RunB");
        }
        else if(h>0)
        {
            anim.Play("RunR");
        }
        else if(h<0)
        {
            anim.Play("RunL");
        }
        else
        {
            anim.Play("Idle");
        }
        */
    }
}
