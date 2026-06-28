using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerOverworld : MonoBehaviour
{
    [Header("Pengaturan Pergerakan")]
    public float moveSpeed = 8f;
    public float jumpForce = 15f;

    [Header("Deteksi Tanah (Ground Check)")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Komponen Animasi")]
    public Animator anim; 

    private Rigidbody2D rb;
    private CapsuleCollider2D myCollider; // Memori untuk mengenali badan sendiri
    private float horizontalInput;
    private bool isFacingRight = true;
    
    [Header("Status (Hanya untuk dilihat)")]
    public bool isGrounded; 
    
    private float jumpCooldownTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CapsuleCollider2D>();
        
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        
        rb.freezeRotation = true; 
    }

    private void Update()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.sedangBicara)
        {
            horizontalInput = 0f; 
            if (anim != null) anim.SetFloat("Speed", 0f);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return; 
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Kurangi waktu jeda
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        // --- LOGIKA GROUND CHECK ANTI-BOCOR ---
        isGrounded = false;
        if (groundCheck != null)
        {
            // Ambil semua objek yang bersentuhan dengan lingkaran sensor
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
            foreach (Collider2D col in colliders)
            {
                // SYARAT MUTLAK: Objek yang diinjak BUKAN badan player itu sendiri DAN bukan Trigger (seperti area dialog)
                if (col != myCollider && !col.isTrigger)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        // --- LOGIKA LOMPAT ANTI-TERBANG ---
        bool jumpInput = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);

        if (jumpInput && isGrounded && jumpCooldownTimer <= 0f && rb.linearVelocity.y <= 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCooldownTimer = 0.25f; 
        }

        FlipKarakter();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.sedangBicara)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void FlipKarakter()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));

        if (horizontalInput != 0)
        {
            anim.SetFloat("FacingDirection", horizontalInput);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            // Indikator Warna: Hijau = Menginjak lantai dengan benar, Merah = Melayang di udara
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}