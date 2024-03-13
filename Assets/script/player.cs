using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class player : MonoBehaviour
{
    [Header("movement")]
    public Rigidbody2D rb;
    public float moveSpeed = 10f;
    private bool isRight = true;

    [Header("jump")]
    private bool isGrounded = true;
    public float jumpForce = 50f;
    public LayerMask groundLayer;
    public float groundCheckRadius = 1f;
    private int jumpCount = 1;

 
   

    [Header("attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attaceRate = 2f;
    float attacknext = 0f;


    [Header("HP")]
    public float maxHealth = 100; // ���ʹ�٧�ش�ͧ Player
    private float currentHealth; // ���ʹ�Ѩ�غѹ�ͧ Player
    public bool isDead = false; // ��������͵�Ǩ�ͺ��� Player ����������

    public RectTransform uiTransform;
    public float lerpSpeed = 5f;


    private SpriteRenderer spriteRenderer;
    public Animator animator;
    public GameController gameController;
 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();


        currentHealth = maxHealth; // ��駤�����ʹ������������ҡѺ���ʹ�٧�ش

    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead)
        {
            if (Time.time >= attacknext)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Attack();
                    attacknext = Time.time + 1f / attaceRate;
                }
            }


            PlayerMovement();
        }

        UpdateUI();

      
    }
  

    void Attack()
    {
        //���animation  attack
        animator.SetTrigger("attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("we hit " + enemy.name);
           
        }
    }
    void OnDrawGizmosSelected()
    {
        //�Ҵ��Ǩ��ͧ���С��⨷��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        //�Ҵ��Ǩ��ͧ�����社��
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }

    private void PlayerMovement()
    {

        /////// �������͹���//////////
        

            float horizontalInput = Input.GetAxis("Horizontal");
            Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            rb.velocity = movement;

          
    

       
        //�ѧ���蹷��зӡ������¹ animation �����ҧ idol �Ѻ run
        if (horizontalInput != 0)
        {
            animator.SetBool("run", true);
        }

        else
        {
            animator.SetBool("run", false);
        }


        if (horizontalInput < 0 && isRight)
        {
            Flip();
        }
        else if (horizontalInput > 0 && !isRight)
        {

            Flip();
        }


        ///////�������͹���///////


        ///////��á��ⴴ///////
        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);

        if (jumpCount >0 && Input.GetKeyDown(KeyCode.W))
        {
            jumpCount--;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Debug.Log("jump");

            // ���¡�� jumpAnimation
            animator.SetTrigger("jump");
         

        }
        if (isGrounded) 
        {
            jumpCount = 1;
        }

        if (!isGrounded)
        {
            animator.SetBool("isGround", false);
        }
        else
        {
            animator.SetBool("isGround", true);
        }
        ///////��á��ⴴ///////
    }

    void Flip()
    {
        isRight = !isRight;
        spriteRenderer.flipX = !isRight;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {

            // ��� Player ���Ѻ player ���Ŵ���ʹ
            TakeDamage(20); // Ŵ���ʹ���� ... ˹��� 

        }
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Ŵ���ʹ�������������·���к�
        animator.SetTrigger("dammaged");
        if (currentHealth <= 0)
        {


            
            Die(); // ������ʹŴŧ����� 0 player �е��
            animator.SetBool("isdead", true);
            isDead = true;


        }
    }

    IEnumerator DieWithDelay()
    {
        yield return new WaitForSeconds(2f);

        // ����� GameController ��Һ��Ҽ����蹵������
        gameController.PlayerDied();
    }

    void Die()
    {
        StartCoroutine(DieWithDelay());
    }

    void UpdateUI()
    {
        // ��Ѻ��� scale Y �ͧ UI ���������ʹ
        float scaleFactorX = currentHealth / maxHealth; // ��Ѻ scale X �������㹪�ǧ 0-1
        float targetScaleX = Mathf.Lerp(scaleFactorX, uiTransform.localScale.x,  Time.deltaTime * lerpSpeed);
        uiTransform.localScale = new Vector3(targetScaleX, uiTransform.localScale.y, uiTransform.localScale.z);
    }
    
}