using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    //상태를 저장할 변수
    public State state = State.PATROL;

    //주인공의 위치를 저장할 변수
    private Transform playerTr;
    //적 캐릭터의 위치를 저장할 변수
    private Transform enemyTr;

    //공격 사정거리
    public float attackDist = 5.0f;
    //추적 사정거리
    public float traceDist = 10.0f;

    //사망 여부를 판단할 변수
    public bool isDie = false;

    //코루틴에서 사용할 지연시간 변수, 계속 선언을 하지않고 변수로 사용가능
    private WaitForSeconds ws;

    //이동을 제어하는 MoveAgent 클래스를 저장할 변수
    private MoveAgent moveAgent;

    //Animator 컴포넌트를 저장할 변수
    private Animator animator;

    //총알 발사를 제어하는 EnemyFire 클래스를 저장할 변수
    private EnemyFire enemyFire;

    //애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    //IsMove를 해시값으로 추출하는 이유는 만약 애니메이션파라미터 이름이 바뀌면 일일이 코드도 바꿔줘야한다
    //IsMove를 해시값으로 가져오는게 아니라 애니메이터의 IsMove라는 속성의 해시값을 가져 오기 때문에 파라미터이름이 바뀐다고 코드까지 바꿀 필요는 없다 본질을 가져온것이기때문에
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");


    private void Awake()
    {
        //주인공 게임오브젝트 추출
        var player = GameObject.FindGameObjectWithTag("PLAYER");
    
        //주인공의 Transform 컴포넌트 추출
        if(player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }

        //적 캐릭터의 Transform 컴포넌트 추출
        enemyTr = GetComponent<Transform>();
        //Animator 컴포넌트 추출
        animator = GetComponent<Animator>();

        //이동을 제어하는 MoveAgent 클래스를 추출
        moveAgent = GetComponent<MoveAgent>();

        //총알 발사를 제어하는 EnemyFire 클래스를 추출
        enemyFire = GetComponent<EnemyFire>();

        //코루틴의 지연시간 생성
        ws = new WaitForSeconds(0.3f);

        //Cycle Offset 값을 불규칙하게 변경
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        //Speed 값을 불규칙하게 변경
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 2.0f));
    }

    private void OnEnable() //Awake 다음에 호출되는 함수, 스위치처럼 객체를 있다없다 할 수 있는 함수
    {
        //CheckState 코루틴 함수 실행
        StartCoroutine(CheckState());
        //ACtion 코루틴 함수 실행
        StartCoroutine(Action());

        Damage.OnPlayerDie += this.OnPlayerDie;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
    }

    IEnumerator CheckState()
    {
        //적 캐릭터가 사망하기 전까지 도는 무한루프
        while(!isDie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            //공격 사정거리 이내인 경우
            if (dist <= attackDist)
            {
                state = State.ATTACK;
            }

            //추적 사정거리 이내 인경우
            else if (dist <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            //0.3초 동안 대기하는 동안 제어권을 양보
            yield return ws;
        }
    }

    //상태에 따라 적 캐릭터의 행동을 처리하는 코루틴 함수
    IEnumerator Action()
    {
        //적 캐릭터가 사망할 때까지 무한루프
        while (!isDie)
        {
            yield return ws;
            //상태에 따라 분기 처리
            switch (state)
            {
                case State.PATROL:
                    //총알 발사 정지
                    enemyFire.isFire = false;
                    //순찰 모드를 활성화
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    //총알 발사 정지
                    enemyFire.isFire = false;
                    //주인공의 위치를 넘겨 추적 모드로 변경
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    //총알 발사 시작
                    if (enemyFire.isFire == false)
                        enemyFire.isFire = true;
                    break;
                case State.DIE:
                    this.gameObject.tag = "Untagged";

                    isDie = true;
                    enemyFire.isFire = false;

                    //순찰 및 추적을 정지
                    moveAgent.Stop();

                    //사망 애니메이션의 종류를 지정
                    animator.SetInteger(hashDieIdx, Random.Range(0, 3));
                    //사망 애니메이션 실행
                    animator.SetTrigger(hashDie);

                    //Capsule Collider 컴포넌트를 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Speed 파라미터에 이동 속도를 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        //모든 코루틴 함수를 종료시킴
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }
}
