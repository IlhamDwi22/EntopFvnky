using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject saveLoadPanel;

    [Header("Main Menu Scene")]
    public string namaSceneMainMenu = "MainMenuScene";

    [HideInInspector] public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Pastikan semua panel tertutup saat awal masuk scene
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
    }

    private void Update()
    {
        // Mendeteksi tombol Escape untuk membuka/menutup pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapePress();
        }
    }

    private void HandleEscapePress()
    {
        // 1. Jika panel sub-options sedang terbuka, tutup dan kembali ke pause panel
        if (optionsPanel != null && optionsPanel.activeSelf)
        {
            CloseOptions();
            return;
        }

        // 2. Jika panel sub-save/load sedang terbuka, tutup dan kembali ke pause panel
        if (saveLoadPanel != null && saveLoadPanel.activeSelf)
        {
            CloseSaveLoad();
            return;
        }

        // 3. Jika sedang tidak di sub-panel, lakukan toggle pause game biasa
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Hentikan seluruh waktu, fisika, dan animasi di scene

        if (pausePanel != null) pausePanel.SetActive(true);

        // Kunci kontrol player (Mendukung PlayerOverworld maupun PlayerTopDown)
        SetPlayerInputActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Jalankan kembali waktu game

        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);

        // Buka kembali kunci kontrol player
        SetPlayerInputActive(true);
    }

    private void SetPlayerInputActive(bool active)
    {
        PlayerOverworld overworldPlayer = FindAnyObjectByType<PlayerOverworld>();
        if (overworldPlayer != null) overworldPlayer.enabled = active;

        PlayerTopDown topDownPlayer = FindAnyObjectByType<PlayerTopDown>();
        if (topDownPlayer != null) topDownPlayer.enabled = active;
    }

    // --- NAVIGASI PANEL ---
    public void OpenOptions()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void OpenSaveLoad()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(true);
    }

    public void CloseSaveLoad()
    {
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // PENTING: Wajib diset kembali ke 1 agar Main Menu tidak ikut beku!
        SceneManager.LoadScene(namaSceneMainMenu);
    }

    // Fungsi untuk mengeluarkan game (Quit Game)
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Menghentikan mode Play di Unity Editor
        #else
        Application.Quit(); // Mengeluarkan game asli yang sudah dibuild
        #endif
    }
}
