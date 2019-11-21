using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct PlayerSfx
{
    //AudioClip : 사운드파일을 저장할 타입, 사운드파일의 객체
    //AudioSource : 어떤 사운드를 재생할지, 소리를 낼지
    //AudioListen : 들어서 플레이어에게 재생해주는
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    //무기타입
    public enum WeaponType
    {
        RIFLE=0,
        SHOTGUN
    }
    //주인공이 현재 들고 있는 무기를 저장할 변수
    public WeaponType currWeapon = WeaponType.RIFLE;

    //총알 프리팹
    public GameObject bullet; 
    //GameObject선언을 한 이유는 프리팹에서 쓰기위해 Transform선언으로 해도 되긴되나 하이라잌키창에서 드래그앤드롭이 되는것이다 프리팹에서는x
    //프리팹을 사용하는 이유는 1. 똑같은 물건들을 하나의 설정만으로 모든 설정이 동일하게 바뀌기 때문에 사용한다(동일한 속성)
    //또한 2. 파일로 저장할수 있기때문에 다른 씬에서도 동일하게 바로 사용할수있다.(다른 씬에서 사용)
    //3. 물건을 동적으로 상황에 따라 생성해야 할 때
    //씬 화면에 나오는 모든 것들은 메모리상에 올라간 객체이기 때문에 객체를 생성도 안하고 화면에 나올수 없다.
    //필요한 순간에 동적으로 메모리 할당여부가 결정되는것이 총알의 속성이다. 이때 프리팹을 이용하는것이 좋다.

    //총알 발사 좌표
    public Transform firePos;

    //탄피 추출 파티클
    public ParticleSystem cartridge;

    //총구 화염 파티클
    private ParticleSystem muzzleFlash; 

    //AudioSource컴포넌트를 저장할 변수
    private AudioSource _audio;
    //오디오 클립을 저장할 변수
    public PlayerSfx playerSfx;

    //Shake 클래스를 저장할 변수
    private Shake shake;

    //탄창 이미지 Image UI
    public Image magazineImg;
    //남은 총알 수 Text UI
    public Text magazineText;

    //최대 총알 수
    public int maxBullet=10;
    //남은 총알 수
    public int remainingBullet = 10;

    //재장전 시간
    public float reloadTime = 2.0f;
    //재장전 여부를 판단할 변수
    private bool isReloading = false;

    //변경할 무기 이미지
    public Sprite[] weaponIcons;
    //교체할 무기 이미지 UI
    public Image weaponImage;

    // Start is called before the first frame update
    void Start()
    {
        //FirePos 하위에 있는 컴포넌트 추출-자주사용 하기에 숙지 필수★
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>(); //하위 오브젝트중 <파티클>의 속성객체를 부른다.
        //드래그앤 드랍뿐 아니라 해당 코드로 호출하여 총구화염객체를 변수에 넣을수있다
        //만약 자식객체로 여러개가 있다면 첫번쨰것을 호출하게 되고
        //만약 여러개의 자식객체를 가져오고 싶다면 GetComponentsInChildren으로 쓴다

        //AudioSource 컴포넌트 추출
        _audio = GetComponent<AudioSource>();

        //Shake 스크립트를 추출
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //마우스 왼쪽 버튼 클릭시 Fire 함수 호출
        if(!isReloading && Input.GetMouseButtonDown(0))
        {
            //총알 수를 하나 감소
            --remainingBullet;
            Fire();

            //남은 총알이 없을 경우 재장전 코루틴 호출
            if (remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        //셰이크 효과 호출
        StartCoroutine(shake.ShakeCamera());

        //Bullet 프리팹을 동적으로 생성
        //Instantiate(bullet, firePos.position, firePos.rotation); //Instantiate : 객체로 만드는 함수 대상,나타날위치,방향,(지정된 부모에 총알이 생겼으면 좋을것 같을때 부모객체)
        //파일-하드디스크, 객체-메모리

        var _bullet = GameManager.instance.GetBullet(); //비활성화 상태가 반환되거나 null이 반환된다
        if(_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }
        //탄피 이펙트 실행
        cartridge.Play();

        //총쿠 화염 파티클 실행
        muzzleFlash.Play();

        //사운드 발생
        FireSfx();

        //재장전 이미지의 fillAmout 속성값 지정
        magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;
        //남은 총알 수 갱신
        UpdateBulletText();
    }

    void FireSfx()
    {
        //현재 들고 있는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currWeapon];
        //사운드발생
        _audio.PlayOneShot(_sfx, 1.0f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1.0f);

        //재장전 오디오의 길이 + 0.3초 동안 대기
        //Fire를 실행하지 않는 이유 : WaitForSeconds로 해당 코드시간동안 startcoroutin을 묶고 있기때문에
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        //각종 변숫값의 초기화
        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainingBullet = maxBullet;

        //남은 총알 수 갱신
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        // (남은 총알 수 / 최대 총알 수) 표시
        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainingBullet, maxBullet);
    }

    public void OnChangeWeapon()
    {
        currWeapon = (WeaponType)((int)++currWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currWeapon];
    }
}
