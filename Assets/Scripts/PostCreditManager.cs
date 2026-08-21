using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PostCreditManager : MonoBehaviour
{
    [Header("Referensi Video Player")]
    public VideoPlayer videoPlayer;

    [Header("Nama Scene Menu Utama")]
    public string namaSceneMainMenu = "MainMenuScene";

    [Header("Pengaturan Tambahan")]
    [Tooltip("Apakah pemain diperbolehkan menekan Space/Escape/Enter untuk melempar video langsung ke Main Menu?")]
    public bool izinkanSkip = true;

    private bool sudahPindah = false;

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            // Daftarkan event ketika video selesai diputar
            videoPlayer.loopPointReached += OnVideoFinished;
            
            // Mainkan video
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("[PostCreditManager] Komponen VideoPlayer tidak ditemukan! Langsung pindah ke Main Menu.");
            KembaliKeMainMenu();
        }
    }

    private void Update()
    {
        // Fitur Skip Video dengan Space, Escape, atau Enter
        if (izinkanSkip && !sudahPindah)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("[PostCreditManager] Video dilewati (Skip) oleh pemain.");
                KembaliKeMainMenu();
            }
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("[PostCreditManager] Video selesai diputar.");
        KembaliKeMainMenu();
    }

    private void KembaliKeMainMenu()
    {
        if (sudahPindah) return;
        sudahPindah = true;

        // Lepas event listener untuk mencegah kebocoran memori
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(namaSceneMainMenu);
    }
}
