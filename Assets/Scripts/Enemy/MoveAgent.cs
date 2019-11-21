using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    //순찰 지점들을 저장하기 위한 List 타입 변수
    public List<Transform> wayPoints;
    //다음 순찰 지점의 배열의 Index
    public int nextIdx;

    //readonly 와 비슷한 것은 프로퍼티get, const가 있다 - 읽기 전용
    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;
    //회전할 떄의 속도를 조절하는 계수
    private float damping = 1.0f;

    //NavMeshAgent 컴포넌트를 저장할 변수
    private NavMeshAgent agent;
    //적 캐릭터의 Transform 컴포넌트를 저장할 변수
    private Transform enemyTr;

    //순찰 여부를 판단하는 변수
    private bool _patrolling;
    //patrolling 프로퍼티 정의(getter, setter)
    //프로퍼티의 경우 새 변수가 생기는게 아니라 변수 _patrolling에 할당이 된다
    //프로퍼티는 public인 변수보다 더 많은 것을 할 수있게 만들 수 있다 if제약조건 등등
    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if(_patrolling)
            {
                agent.speed = patrolSpeed;
                //순찰 상태의 회전계수
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    //추적 대상의 위치를 저장하는 변수
    private Vector3 _traceTarget;
    //traceTarget의 프로퍼티 정의 (getter, setter)
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            //추적상태의 회전 계수
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }

    //다음 목적지까지 이동 명령을 내리는 함수
    void MoveWayPoint()
    {
        //최단거리 경로 계산이 끝나지 않았으면 다음을 수행하지 않음
        if (agent.isPathStale) return;

        //다음 목적지를 wayPoints배열에서 추출한 위치로 다음 목적지를 지정
        agent.destination = wayPoints[nextIdx].position;
        //내비게이션 기능을 활성화해서 이동을 시작함
        agent.isStopped = false;
    }

    //주인공을 추적할 때 이동시키는 함수
    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return; //isPathStale : 경로계산

        agent.destination = pos;
        agent.isStopped = false;
    }

    //순찰 및 추적을 정지시키는 함수
    public void Stop()
    {
        agent.isStopped = true;
        //바로 정지하기 위해 속도를 0으로 설정
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }

    //NavMeshAgent의 이동 속도에 대한 프로퍼티 정의(getter)
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }
    void Start()
    {
        //적 캐릭터의 Transform 컴포넌트 추출 후 변수에 저장
        enemyTr = GetComponent<Transform>();
        //NavMeshAgent 컴포넌트를 추출 후 변수에 저장
        agent = GetComponent<NavMeshAgent>();
        //목적지에 가까워질수록 속도를 줄이는 옵션을 비활성화
        agent.autoBraking = false;
        
        //자동으로 회전하는 기능을 비활성화
        agent.updateRotation = false;

        agent.speed = patrolSpeed;

        //하이러키 뷰의 WayPoint 게임오브젝트 추출
        var group = GameObject.Find("WayPoint");
        if (group != null)
        {
            //WayPoint 하위에 있는 모든 Transform 컴포넌트를 추출한 후 List 타입의 wayPoints 배열에 추가
            //GetComponentInChildren : 자식 중 하나
            //GetComponentsInChildren : 모든 자식
            group.GetComponentsInChildren<Transform>(wayPoints);
            //배열의 첫번째 항목 삭제
            //첫번째 요소인 부모 WayPoint까지 가져오므로 제외시키기 위해 제거 필요한것은 자식들뿐
            wayPoints.RemoveAt(0);

            //첫번째로 이동할 위치를 불규칙하게 추출
            nextIdx = Random.Range(0, wayPoints.Count);

            //NavMeshAgent 컴포넌트를 추출한 후 변수에 저장
            agent = GetComponent<NavMeshAgent>();
            //목적지에 가까워질수록 속도를 줄이는 옵션을 비활성화
            agent.autoBraking = false;
        }

        //MoveWayPoint();
        this.patrolling = true;
    }

    // Update is called once per frame
    void Update()
    {
        //적 캐릭터가 이동 중일 때만 회전
        if(agent.isStopped == false)
        {
            //NavMeshAgent가 가야 할 방향 벡터를 쿼너티언 타입의 각도로 변환
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            //보간 함수를 사용해 점진적으로 회전시킴
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
        //순찰 모드가 아닐 경우 이후 로직을 수행하지 않음
        if (!_patrolling) return;
        //NavMeshAgent가 이동하고 있고 목적지에 도착했는지 여부를 계산
        if (agent.velocity.sqrMagnitude >= 2.0f * 0.2f && agent.remainingDistance <= 0.5f) //velocity.sqrMagnitude : 이동속도.벡터거리계산함수?, remainingDistance:목적지까지 남은거리
        {
            //다음 목적지의 배열 첨자를 계산
            //nextIdx = ++nextIdx % wayPoints.Count; //wayPoints.Count를 나누는 이유는 범위가 넘어가지 않도록 나머지를 할당해준다
            nextIdx = Random.Range(0, wayPoints.Count);
            //다음 목적지로 이동 명령을 수행
            MoveWayPoint();
        }
    }
}
