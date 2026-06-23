using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [Header("Komponen Fader")]
    public CanvasGroup faderGroup;
    
    public float kecepatanFade = 1.5f;

    private void Awake()
    {
        // Sistem Pengaman: Cegah ada 2 Fader menumpuk di satu scene
        if (Instance == null) 
        { 
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
            return; 
        }
    }

    private void Start()
    {
        // Sistem Peringatan Otomatis jika referensi CanvasGroup lepas
        if (faderGroup == null)
        {
            Debug.LogError("[SceneFader] GAGAL FADE IN! Kolom Fader Group KOSONG di Inspector. Buka Prefab-mu dan masukkan ulang Panel_LayarHitam!");
            return;
        }

        faderGroup.alpha = 1f; 
        faderGroup.blocksRaycasts = true; 
        StartCoroutine(FadeInRoutine());
    }

    public void PindahSceneDenganFade(string namaScene)
    {
        if (faderGroup != null)
        {
            StartCoroutine(FadeOutRoutine(namaScene));
        }
        else
        {
            Debug.LogWarning("[SceneFader] Gagal Fade Out karena Fader Group kosong. Terpaksa pindah instan ke: " + namaScene);
            SceneManager.LoadScene(namaScene);
        }
    }

    private IEnumerator FadeInRoutine()
    {
        // Menggunakan unscaledDeltaTime agar fader TETAP JALAN walaupun karakter atau game sedang nge-freeze
        while (faderGroup.alpha > 0f)
        {
            faderGroup.alpha -= Time.unscaledDeltaTime * kecepatanFade;
            yield return null;
        }
        
        faderGroup.alpha = 0f;
        faderGroup.blocksRaycasts = false; 
    }

    private IEnumerator FadeOutRoutine(string namaScene)
    {
        faderGroup.blocksRaycasts = true; 
        
        while (faderGroup.alpha < 1f)
        {
            faderGroup.alpha += Time.unscaledDeltaTime * kecepatanFade;
            yield return null;
        }

        SceneManager.LoadScene(namaScene);
    }
} 