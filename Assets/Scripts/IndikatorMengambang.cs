using UnityEngine;

public class IndikatorMengambang : MonoBehaviour
{
    [Header("Pengaturan Animasi")]
    public float kecepatan = 5f; // Semakin besar, semakin cepat naik-turun
    public float tinggi = 0.15f; // Jarak naik-turun panah

    private Vector3 posisiAwal;

    private void OnEnable()
    {
        // Menyimpan posisi panah setiap kali ia dimunculkan
        posisiAwal = transform.localPosition;
    }

    private void Update()
    {
        // Rumus matematika agar benda mengambang mulus
        transform.localPosition = posisiAwal + new Vector3(0f, Mathf.Sin(Time.time * kecepatan) * tinggi, 0f);
    }
}