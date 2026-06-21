using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyNPC : MonoBehaviour
{
    [Header("Data Pertarungan Musuh")]
    [Tooltip("Masukkan Data Musuh (ScriptableObject) khusus untuk NPC ini")]
    public OpponentData dataMusuh;
    
    [Tooltip("Ketik nama Scene arena pertarungan ritme kamu (contoh: Gameplay)")]
    public string namaSceneRhythm = "Gameplay";

    [Header("UI Visual (Opsional)")]
    [Tooltip("Masukkan objek teks 'Tekan E untuk Bertarung' yang ada di atas kepala musuh")]
    public GameObject tombolInteraksiUI; 

    private bool playerDiDekat = false;

    private void Start()
    {
        // Sembunyikan tulisan "Tekan E" saat game baru mulai
        if (tombolInteraksiUI != null) tombolInteraksiUI.SetActive(false);
    }

    private void Update()
    {
        // Jika player ada di dekat musuh DAN menekan tombol E
        if (playerDiDekat && Input.GetKeyDown(KeyCode.E))
        {
            MulaiPertarungan();
        }
    }

    private void MulaiPertarungan()
    {
        // 1. Serahkan data musuh ini ke Memori Global (GameManager)
        GameManager.musuhPilihanSaatIni = dataMusuh;
        GameManager.urutanLaguSaatIni = 0; // Mulai dari lagu pertama musuh tersebut

        // 2. Pindah/Loading ke Scene Pertarungan
        Debug.Log($"[Overworld] Memulai pertarungan melawan: {dataMusuh.namaMusuh}");
        SceneManager.LoadScene(namaSceneRhythm);
    }

    // Mendeteksi player masuk ke area dekat musuh
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDiDekat = true;
            if (tombolInteraksiUI != null) tombolInteraksiUI.SetActive(true);
        }
    }

    // Mendeteksi player menjauh dari musuh
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDiDekat = false;
            if (tombolInteraksiUI != null) tombolInteraksiUI.SetActive(false);
        }
    }
}