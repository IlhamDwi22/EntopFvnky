using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerOverworld : MonoBehaviour
{
    [Header("Pengaturan Pergerakan")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;

    [Header("Deteksi Tanah (Ground Check)")]
    [Tooltip("Tarik objek kosong (GroundCheck) yang ada di kaki karakter ke sini")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    [Tooltip("Pilih layer yang dianggap sebagai tanah/pijakan")]
    public LayerMask groundLayer;

    [Header("Komponen Animasi")]
    [Tooltip("Tarik komponen Animator dari karakter ke kolom ini di Inspector")]
    public Animator anim; 

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isFacingRight = true;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        
        rb.freezeRotation = true; 
    }

    private void Update()
    {
        // --- LOGIKA BARU: FREEZE SAAT DIALOG ---
        if (DialogManager.Instance != null && DialogManager.Instance.sedangBicara)
        {
            horizontalInput = 0f; // Reset input jalan menjadi nol
            
            // Paksa animator kembali ke animasi Idle (Speed = 0)
            // Arah hadap (FacingDirection) sengaja tidak diubah agar karakter tetap menghadap ke posisi terakhirnya
            if (anim != null)
            {
                anim.SetFloat("Speed", 0f);
            }
            
            // Hentikan momentum kecepatan fisik secara instan
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            
            return; // Keluar dari fungsi Update agar semua input tombol di bawah diabaikan
        }

        // 1. Ambil Input Kiri/Kanan (A/D atau Panah Kiri/Kanan) jika normal
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. Cek apakah karakter sedang menginjak tanah
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 3. Deteksi Input Lompat
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 4. Balikkan hadapan fisik karakter (Kiri/Kanan)
        FlipKarakter();

        // 5. UPDATE PARAMETER ANIMATOR (Idle & Walking)
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        // Kunci juga kecepatan fisik di FixedUpdate agar tidak ada celah pergerakan gaib saat dialog
        if (DialogManager.Instance != null && DialogManager.Instance.sedangBicara)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Terapkan kecepatan pada Rigidbody secara normal jika tidak sedang dialog
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}