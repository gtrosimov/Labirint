using TMPro;
using UnityEngine;

public class CutScene_Move : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Animator anim;
    Rigidbody rb;    
    Vector3 dir;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        dir = new Vector3(hor, 0.0f, ver);
        dir = transform.TransformDirection(dir);

        TestAnim(hor, ver);       
    }
    void FixedUpdate()
    {
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
    }
    void TestAnim(float h, float v)
    {
        if (h != 0 || v != 0)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }
    }    
}
