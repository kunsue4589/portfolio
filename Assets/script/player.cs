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
    public float maxHealth = 100; // เลือดสูงสุดของ Player
    private float currentHealth; // เลือดปัจจุบันของ Player
    public bool isDead = false; // ตัวแปรเพื่อตรวจสอบว่า Player ตายหรือไม่

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


        currentHealth = maxHealth; // ตั้งค่าเลือดเริ่มต้นให้เท่ากับเลือดสูงสุด

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
        //เล่นanimation  attack
        animator.SetTrigger("attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("we hit " + enemy.name);
           
        }
    }
    void OnDrawGizmosSelected()
    {
        //วาดตัวจำลองระยะการโจทตี
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        //วาดตัวจำลองระยะเช็คพื้น
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }

    private void PlayerMovement()
    {

        /////// การเคลื่อนที่//////////
        

            float horizontalInput = Input.GetAxis("Horizontal");
            Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            rb.velocity = movement;

          
    

       
        //ฟังก์ชั่นที่จะทำการเปลี่ยน animation ระหว่าง idol กับ run
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


        ///////การเคลื่อนที่///////


        ///////การกระโดด///////
        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);

        if (jumpCount >0 && Input.GetKeyDown(KeyCode.W))
        {
            jumpCount--;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Debug.Log("jump");

            // เรียกใช้ jumpAnimation
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
        ///////การกระโดด///////
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

            // ถ้า Player ชนกับ player ให้ลดเลือด
            TakeDamage(20); // ลดเลือดทีละ ... หน่วย 

        }
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // ลดเลือดตามความเสียหายที่ระบุ
        animator.SetTrigger("dammaged");
        if (currentHealth <= 0)
        {


            
            Die(); // ถ้าเลือดลดลงเหลือ 0 player จะตาย
            animator.SetBool("isdead", true);
            isDead = true;


        }
    }

    IEnumerator DieWithDelay()
    {
        yield return new WaitForSeconds(2f);

        // แจ้งให้ GameController ทราบว่าผู้เล่นตายแล้ว
        gameController.PlayerDied();
    }

    void Die()
    {
        StartCoroutine(DieWithDelay());
    }

    void UpdateUI()
    {
        // ปรับค่า scale Y ของ UI ตามค่าเลือด
        float scaleFactorX = currentHealth / maxHealth; // ปรับ scale X ให้อยู่ในช่วง 0-1
        float targetScaleX = Mathf.Lerp(scaleFactorX, uiTransform.localScale.x,  Time.deltaTime * lerpSpeed);
        uiTransform.localScale = new Vector3(targetScaleX, uiTransform.localScale.y, uiTransform.localScale.z);
    }
    
}