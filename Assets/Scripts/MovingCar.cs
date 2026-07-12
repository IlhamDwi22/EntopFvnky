using UnityEngine;

public class MovingCar : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;

    public void Setup(Vector3 target, float moveSpeed, bool flipX)
    {
        targetPosition = target;
        speed = moveSpeed;

        // Membalik arah sprite mobil berdasarkan sumbu X localScale
        if (flipX)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void Update()
    {
        // Menggerakkan mobil mendekati titik tujuan
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Jika sudah sampai sangat dekat dengan tujuan, hancurkan mobil
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
