using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerTopDown : MonoBehaviour
{
    [Header("Pengaturan Pergerakan Top-Down")]
    public float moveSpeed = 5f;

    [Header("Komponen Animasi")]
    public Animator anim;

    private Rigidbody2D rb;
    private Vector2 movement;
    
    private enum FacingDirection { Down, Up, Side }
    private FacingDirection currentFacing = FacingDirection.Down;
    
    private bool isFacingRight = true;
    private string currentAnimState = "";

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        
        rb.gravityScale = 0f; 
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.sedangBicara)
        {
            movement = Vector2.zero;
            UpdateAnimationState();
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.sedangBicara)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * moveSpeed;
    }

    private void UpdateAnimationState()
    {
        if (anim == null) return;

        // Ambil nilai mutlak/positif dari pergerakan untuk parameter animasi
        float horizontalSpeed = Mathf.Abs(movement.x);
        float verticalSpeed = Mathf.Abs(movement.y);

        // 1. SISTEM FLIP ANTI-MACET
        if (movement.x > 0.1f && !isFacingRight)
        {
            FlipKarakter();
        }
        else if (movement.x < -0.1f && isFacingRight)
        {
            FlipKarakter();
        }

        // 2. PENENTUAN ANIMASI BERDASARKAN ANGKA AMBANG BATAS (THRESHOLD)
        string targetAnim = "";

        if (horizontalSpeed > 0.1f) 
        {
            // Jika bergerak ke kanan/kiri secara horizontal melebihi batas toleransi
            targetAnim = "Walking"; 
            currentFacing = FacingDirection.Side;
        }
        else if (movement.y > 0.1f)
        {
            targetAnim = "Walk_Up";
            currentFacing = FacingDirection.Up;
        }
        else if (movement.y < -0.1f)
        {
            targetAnim = "Walk_Down";
            currentFacing = FacingDirection.Down;
        }
        else
        {
            // Jika benar-benar diam di bawah batas toleransi, cek arah hadap terakhir
            if (currentFacing == FacingDirection.Side) targetAnim = "Idle";
            else if (currentFacing == FacingDirection.Up) targetAnim = "Idle_Up";
            else if (currentFacing == FacingDirection.Down) targetAnim = "Idle_Down";
        }

        // 3. EKSEKUSI ANIMASI
        if (targetAnim != currentAnimState && !string.IsNullOrEmpty(targetAnim))
        {
            anim.Play(targetAnim);
            currentAnimState = targetAnim;
        }
    }

    private void FlipKarakter()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}