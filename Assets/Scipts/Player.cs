using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int coins;
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 7.5f;
    public float jumpContinuesForce = 1f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Image HealthImage;

    public AudioClip jumpClip;
    public AudioClip hurtClip;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float fireRate;

    [Header("Invincibility")]
    public float iFrameDuration = 1f;
    private bool isInvincible;

    private float fireTimer;
    
    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator animator;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    public int extraJumpsValue = 1;
    private int extraJumps;

    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        HealthImage = GameObject.FindWithTag("Health").GetComponent<Image>();

        extraJumps = extraJumpsValue;


        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (Checkpoint.saveScene == currentScene && Checkpoint.savePosition != Vector2.zero)
        {
            // ✔ ใช้ checkpoint ของด่านนี้เท่านั้น
            transform.position = Checkpoint.savePosition;
        }
        else
        {
            // ✔ ถ้าไม่ใช่ → ไป Start
            GameObject start = GameObject.Find("Start");
            if (start != null)
            {
                transform.position = start.transform.position;
            }
        }

    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if(rb.linearVelocityX != 0)
        {
            if(rb.linearVelocityX > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            extraJumps = extraJumpsValue;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySFX(jumpClip);
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
                PlaySFX(jumpClip);
                jumpBufferCounter = 0f;
            }

        }

        if(Input.GetKey(KeyCode.Space) && rb.linearVelocityY > 0)
        {
            rb.AddForceY(jumpContinuesForce);
        }

        SetAnimation(moveInput);

        HealthImage.fillAmount = health / 100f;

        if(rb.linearVelocityY < 0)
        {
            rb.gravityScale = 3f;
        }
        else
        {
            rb.gravityScale = 2f;
        }

        if(transform.position.y < -10)
        {
            Die();
        }

        HandleShooting();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0)
            {
                animator.Play("Player_Idle");
            }
            else
            {
                animator.Play("Player_Run");
            }
        }
        else
        {
            if (rb.linearVelocityY > 0)
            {
                animator.Play("Player_Jump");
            }
            else
            {
                animator.Play("Player_Fall");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damagee"))
        {
            if (isInvincible) return;

            PlaySFX(hurtClip);
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            StartCoroutine(InvincibilityFrames());

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = new Color(Color.red.r, Color.red.g, Color.red.g, spriteRenderer.color.a);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(Color.white.r, Color.white.g, Color.white.g, spriteRenderer.color.a);
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    public void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Strawberry")
        {
            health += 25;
            if (health > 100)
            {
                health = 100;
            }
            Destroy(collision.gameObject);
        }

    }

    private void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if(Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if(spriteRenderer.flipX)
        {
            bulletScript.SetDirection(Vector2.left);
        }
        else
        {
            bulletScript.SetDirection(Vector2.right);
        }
    }
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        float elapsed = 0f;

        while(elapsed < iFrameDuration)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.3f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);

            elapsed += 0.2f;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        isInvincible = false;
    }


}
