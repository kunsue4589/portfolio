using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class miniEnemy : MonoBehaviour
{
    private Animator anim;
    
    public float speed;
    public Transform pointA;  // �ش A
    public Transform pointB;  // �ش B
    private SpriteRenderer spriteRenderer;
    private bool isRight = true;
    private bool movingTowardsA = true;
    public player players;

    // Start is called before the first frame update
    void Start()
    {
       spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        anim.SetBool("run", true);
        players = FindObjectOfType<player>();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (movingTowardsA)
        {
            MoveTowards(pointA);
        }
        else
        {
            MoveTowards(pointB);
        }
    }

    void MoveTowards(Transform target)
    {
       
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // ��� Enemy 件֧�ش���·ҧ���� ��Ѻ��ȷҧ��ѧ�ش���
        if (Vector2.Distance(transform.position, target.position) < 1f)
        {
            movingTowardsA = !movingTowardsA;
            Flip();

        }
    }
    void Flip()
    {
        isRight = !isRight;
        spriteRenderer.flipX = !isRight;
    }

  
    public void die()
    {

    }
}
