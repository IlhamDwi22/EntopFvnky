using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OverworldPlayer : MonoBehaviour
{
    [Header("Pengaturan Karakter")]
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Memastikan rotasi Z terkunci agar karakter tidak terguling saat menabrak tembok
        rb.freezeRotation = true; 
    }

    private void Update()
    {
        // Mengambil input WASD atau Panah dari keyboard
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        // Menggerakkan karakter menggunakan sistem fisika Unity
        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }
}