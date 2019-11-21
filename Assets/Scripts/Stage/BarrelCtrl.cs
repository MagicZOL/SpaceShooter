using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    //폭발 효과 프리팹을 저장할 변수
    public GameObject expEffect;
    //총알이 맞은 횟수
    private int hitCount = 0;
    //Rigidbody 컴포넌트를 저장할 변수
    private Rigidbody rb;

    //찌그러진 드럼통의 메쉬를 저장할 배열
    public Mesh[] meshes;
    //MeshFilter 컴포넌트를 저장할 변수
    private MeshFilter meshFilter;

    //드럼통의 텍스처를 저장할 배열
    public Texture[] textures;
    //MeshRenderer컴포넌트를 저장할 변수
    private MeshRenderer _renderer;

    //폭발 반경
    public float expRadius = 10.0f;

    //AudioSource컴포넌트를 저장할 변수
    public AudioSource _audio;
    //폭발음 오디오 클립
    public AudioClip expSfx;

    //Shake 클래스를 저장할 변수
    public Shake shake;

    // Start is called before the first frame update
    void Start()
    {
        //Rigidbody 컴포넌트를 추출해 저장
        rb = GetComponent<Rigidbody>();

        //MeshFilter 컴포넌트를 추출해 저장
        meshFilter = GetComponent<MeshFilter>();

        //MeshRenderer컴포넌트를 추출해 저정
        _renderer = GetComponent<MeshRenderer>();

        //난수를 발생시켜 불규칙적인 텍스처를 적용
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];

        //AudioSource 컴포넌트를 추출해 저장
        _audio = GetComponent<AudioSource>();

        //Shake 스크립트를 추출
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    //충돌이 발생했을 때 한번 호출되는 콜백 함수
    void OnCollisionEnter(Collision collision)
    {
        //충돌한 게임오브젝트의 태그를 비교
        if(collision.collider.CompareTag("BULLET"))
        {
            if (++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    //폭발 효과를 처리할 함수
    void ExpBarrel()
    {
        //폭발 효과 프리팹을 동적으로 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity); //Quaternion.identity : 회전을 0 으로 초기화
        Destroy(effect, 2.0f);
        //Rigidbpdy 컴포넌트의 mass를 1.0으로 수정해 폭발이 잘 되게 무게를 가볍게 함
        //rb.mass = 1.0f;
        //위로 솟구치는 힘을 가함
        //rb.AddForce(Vector3.up * 1000.0f);
        

        //난수를 발생
        //정수를 넣으면 처음값~마지막값전까지 즉,Random.Range(0, 10)인 경우 1~9까지 
        //실수를 넣으면 처음값~마지막값까지 즉, Random.Range(1.0f, 10.0f)인 경우 1.0f~10.0f 차이 기억
        int idx = Random.Range(0, meshes.Length);
        //찌그러진 메쉬를 적용
        meshFilter.sharedMesh = meshes[idx]; //sharedMesh : Mesh정보가 바뀐다
        //콜라이더 변경
        GetComponent<MeshCollider>().sharedMesh = meshes[idx];

        //폭발력 생성
        IndirectDamage(transform.position);

        //폭발음 발생
        _audio.PlayOneShot(expSfx, 1.0f);

        //셰이크 효과 호출
        StartCoroutine(shake.ShakeCamera(0.5f, 0.6f, 0.8f));

    }

    //폭발력을 주변에 전달하는 함수
    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 9); //지정된 위치, 반경(지정된 반경에 무언가  있는지 찾는다), 1을 왼쪽으로9칸=512 9번쨰레이어를 지정하게됨
        //Physics.OverlapSphere : 충돌되는 사물이 뭔지 식별하기 위해 사용한다
        //pos의 위치에서 expRadius반경으로 overlapSphere의 가상의 구를 생성하여 9번레이어값에 일치하는것을 찾아서 colls배열에 저장한다.

        foreach (var coll in colls)
        {
            //폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트 추출
            var _rb = coll.GetComponent<Rigidbody>();
            //드럼통의 무게를 가볍게 함
            _rb.mass = 1.0f;
            //폭발력 전달
            _rb.AddExplosionForce(1200.0f, pos, expRadius, 1000.0f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
