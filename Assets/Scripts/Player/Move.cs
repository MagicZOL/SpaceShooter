using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    Transform tr;
    float addLerp = 0;
    // Start is called before the first frame update
    void Start()
    {
        tr=GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        addLerp += Time.deltaTime * 0.1f;
        transform.position = Vector3.Lerp(transform.position, new Vector3(3, 3, 0), addLerp);

        //transform.Translate(Vector3.left * Time.deltaTime, Space.World);
    }
}
