using UnityEngine;
using UnityEngine.InputSystem; // Wajib untuk New Input System
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject titlePanel;
    public GameObject menuPanel;
    public GameObject firstSelectedButton; // Tombol pertama yang disorot otomatis di Main Menu

    [Header("Audio Setup")]
    public AudioSource sfxSource;
    public AudioClip transitionSound; // Suara beat drop saat masuk menu

    [Header("Options Panel")]
    public GameObject optionsPanel;
    public GameObject optionsFirstButton; // Elemen pertama yang disorot di panel Options (misal: Slider)
    public GameObject optionsOpenButton;  // Tombol "Studio Config" di menu utama untuk kembali disorot

    private bool isAtTitleScreen = true;

    void Start()
    {
        // Kondisi awal saat game dijalankan
        titlePanel.SetActive(true);
        menuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    void Update()
    {
        // Mendeteksi input apapun saat berada di Title Screen
        if (isAtTitleScreen)
        {
            if ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) || 
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
            {
                TransitionToMainMenu();
            }
        }
    }

    void TransitionToMainMenu()
    {
        isAtTitleScreen = false;
        
        // Mainkan SFX Transisi jika sudah dimasukkan
        if (sfxSource != null && transitionSound != null)
        {
            sfxSource.PlayOneShot(transitionSound);
        }

        // Matikan title, nyalakan menu
        titlePanel.SetActive(false);
        menuPanel.SetActive(true);

        // Otomatis menyorot tombol pertama (untuk navigasi keyboard)
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    // Fungsi untuk tombol "Studio Config"
    public void OpenOptions()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    // Fungsi untuk tombol "Back" di dalam panel Options
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsOpenButton); // Kembalikan sorotan ke tombol Studio Config
    }
}