using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    //스파크 프리팹 저장할 변수
    public GameObject sparkEffect;

    //자신이 콜라이더를 가지고 있을때 다른충돌체와 충동하게 됬을때 호출이되는 함수 : OnCollisionEnter
    private void OnCollisionEnter(Collision collision)
    {
        //충돌한 게임오브젝트의 태그값 비교
        if(collision.collider.tag == "BULLET")
        {
            ShowEffect(collision);

            //충돌한 게임오브젝트 삭제,Destory : 객체 삭제
            // Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    void ShowEffect(Collision collision)
    {
        //충돌 지점의 정보를 추출
        ContactPoint contact = collision.contacts[0];
        //법선 벡터가 이루는 회전각도를 추출 (법선 : 수직)
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal); //normal : 면과 수직이 되는 것
        //FromToRotation : 두 매개변수의 회전값을 일치시켜주는 함수 
        //-Vector3 : 총알의 뒷면이 다았을때 충돌이되면서 스파크가 발생하기 위해 총알방향을 뒤집어준것이다. 그렇게 되면 벽에 총알의 뒷면이 닿게 된다.

        //스파크 효과를 생성
        Instantiate(sparkEffect, contact.point, rot);
    }
}
