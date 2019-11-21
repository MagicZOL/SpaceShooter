using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip run1F;
    public AnimationClip run2B;
    public AnimationClip run3L;
    public AnimationClip run4R;
}
public class PlayCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    private Transform tr;
    public float moveSpeed = 10.0f;
    public float rotSpeed = 80.0f;

    public PlayerAnim playerAnim;
    public Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        anim.clip = playerAnim.idle;
        anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Vector3 movedir = (Vector3.forward * v) + (Vector3.right * h);

        tr.Translate(movedir * moveSpeed * Time.deltaTime, Space.Self);

        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);
    }
}
