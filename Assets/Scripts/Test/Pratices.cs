using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pratices : MonoBehaviour
{
    Transform tr;

    float current = 0;
    float duration = 0;

    Vector3 v3; //도착지좌표
    float dis;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();

        StartCoroutine(moveCube());
    }

    IEnumerator moveCube()
    {
        //움직인다 처음 위치에서 10만큼
        //현재시간,흐른 시간 처음위치, 목표위치

        v3 = new Vector3(tr.position.x + 10, tr.position.y, tr.position.z);
        while (Vector3.Distance(tr.position, v3) >= 0.1f)
        {
            //float dis = Vector3.Distance(tr.position, v3);
            tr.rotation = Quaternion.Euler(0, 90, 0);
            tr.position += tr.forward * Time.deltaTime;
            Debug.Log(dis);
            yield return null;
        }
        yield return null;
    }
}
