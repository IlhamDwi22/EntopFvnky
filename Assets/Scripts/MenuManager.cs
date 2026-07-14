using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject titlePanel;
    public GameObject menuPanel;
    public GameObject optionsPanel;
    public GameObject savePanel;

    [Header("Navigation Buttons")]
    public GameObject firstSelectedButton; // Tombol pertama yang disorot otomatis di Main Menu
    public GameObject optionsFirstButton;  // Elemen pertama yang disorot di panel Options (misal: Slider)
    public GameObject optionsOpenButton;   // Tombol "Studio Config" di menu utama untuk kembali disorot
    public GameObject saveFirstButton;     // Tombol pertama yang disorot di panel Save
    public GameObject saveOpenButton;      // Tombol "Save Game" di menu utama untuk kembali disorot

    [Header("Audio Setup")]
    public AudioSource sfxSource;
    public AudioClip transitionSound; // Suara saat geser menu

    [Header("Camera & Slide Setup")]
    public Camera mainCamera; // Masukkan Main Camera di Inspector
    public Transform titleCameraTarget;   // Posisi kamera saat di Title Screen
    public Transform menuCameraTarget;    // Posisi kamera saat di Main Menu
    public Transform optionsCameraTarget; // Posisi kamera saat di Options
    public Transform saveCameraTarget;    // Posisi kamera saat di Save/Load
    
    [Range(0.1f, 2f)]
    public float slideDuration = 0.4f; // Kecepatan geser (semakin kecil semakin cepat)
    
    [Tooltip("Gunakan kurva seperti Ease-In-Out untuk efek transisi yang dinamis (Persona style)")]
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool isAtTitleScreen = true;
    private bool isTransitioning = false; // Mencegah spam tombol saat sedang geser layar

    void Start()
    {
        // Kondisi awal saat game dijalankan
        titlePanel.SetActive(true);
        menuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // Posisikan kamera di titik awal (Title) secara instan
        if (mainCamera != null && titleCameraTarget != null)
        {
            mainCamera.transform.position = titleCameraTarget.position;
        }
    }

    void Update()
    {
        // Mendeteksi input apapun saat berada di Title Screen
        if (isAtTitleScreen && !isTransitioning)
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
        
        // Mulai pergerakan kamera dari Title ke Menu
        StartCoroutine(SlideMenuTransition(
            titlePanel, 
            menuPanel, 
            menuCameraTarget, 
            firstSelectedButton
        ));
    }

    // Fungsi untuk tombol "Studio Config"
    public void OpenOptions()
    {
        if (isTransitioning) return;

        // Mulai pergerakan kamera dari Menu ke Options
        StartCoroutine(SlideMenuTransition(
            menuPanel, 
            optionsPanel, 
            optionsCameraTarget, 
            optionsFirstButton
        ));
    }

    // Fungsi untuk tombol "Back" di dalam panel Options
    public void CloseOptions()
    {
        if (isTransitioning) return;

        // Mulai pergerakan kamera dari Options kembali ke Menu
        StartCoroutine(SlideMenuTransition(
            optionsPanel, 
            menuPanel, 
            menuCameraTarget, 
            optionsOpenButton
        ));
    }

    // Fungsi untuk tombol "Save Game" di Menu Utama
    public void OpenSavePanel()
    {
        if (isTransitioning) return;

        // Geser ke Save Panel
        StartCoroutine(SlideMenuTransition(
            menuPanel, 
            savePanel, 
            saveCameraTarget, 
            saveFirstButton
        ));
    }

    // Fungsi untuk tombol "Back" di dalam Save Panel
    public void CloseSavePanel()
    {
        if (isTransitioning) return;

        // Kembali ke Menu Utama
        StartCoroutine(SlideMenuTransition(
            savePanel, 
            menuPanel, 
            menuCameraTarget, 
            saveOpenButton
        ));
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

    /// <summary>
    /// Coroutine utama untuk menangani semua transisi geser kamera dan pergantian UI
    /// </summary>
    private IEnumerator SlideMenuTransition(GameObject panelToHide, GameObject panelToShow, Transform targetCameraPos, GameObject buttonToSelect)
    {
        isTransitioning = true;

        // Mainkan SFX Transisi
        if (sfxSource != null && transitionSound != null)
        {
            sfxSource.PlayOneShot(transitionSound);
        }

        // Matikan UI yang lama agar bersih saat kamera bergerak
        panelToHide.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        // Logika geser kamera
        if (mainCamera != null && targetCameraPos != null)
        {
            Vector3 startPos = mainCamera.transform.position;
            Vector3 endPos = targetCameraPos.position;
            float elapsedTime = 0f;

            while (elapsedTime < slideDuration)
            {
                elapsedTime += Time.deltaTime;
                float percentage = elapsedTime / slideDuration;
                
                // Aplikasikan kurva animasi agar gerakan terlihat berbobot/nge-snap
                float curveValue = slideCurve.Evaluate(percentage); 
                
                mainCamera.transform.position = Vector3.Lerp(startPos, endPos, curveValue);
                yield return null;
            }
            
            // Pastikan posisi akhirnya pas 100%
            mainCamera.transform.position = endPos;
        }
        else
        {
            Debug.LogWarning("Main Camera atau Camera Target belum di-assign di Inspector!");
        }

        // Nyalakan UI baru setelah kamera sampai tujuan
        panelToShow.SetActive(true);
        EventSystem.current.SetSelectedGameObject(buttonToSelect);

        isTransitioning = false;
    }
}