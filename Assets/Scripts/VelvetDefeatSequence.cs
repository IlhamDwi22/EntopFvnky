using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VelvetDefeatSequence : MonoBehaviour
{
    [Header("Identitas Musuh")]
    public OpponentData dataVelvet;

    [Header("Dialog Skenario")]
    public DialogData dialogMenangVelvet; // Velvet: "Kau pikir..."
    public DialogData dialogZeroGlitch;   // Zero: "Apa yang sedang..."
    public DialogData dialogZeroKegelapan; // Zero: "Tiba-tiba sesosok..."
    public DialogData dialogLayarHitam;   // Zero: "Semua yang kulihat..."

    [Header("Visual Glitch & Kamui")]
    public Material kamuiMaterial;
    public GameObject panelKamui; // UI RawImage yang menggunakan material Kamui
    public float durasiKamui = 1.5f;
    [Tooltip("Centang jika ingin layar bergetar/glitch saat Zero cemas (sebelum Kamui). Jika dimatikan, dialog Zero berjalan bersih.")]
    public bool aktifkanGlitchPreKamui = true;

    [Header("Lampu & Lingkungan Studio")]
    public GameObject[] lampuStudio; // Lampu yang akan dibuat berkedip
    public SpriteRenderer[] dindingStudio; // Dinding yang akan menggelap
    public Color warnaDindingGelap = new Color(0.1f, 0.1f, 0.1f, 1f);

    [Header("Audio SFX")]
    public AudioSource audioSource;
    public AudioClip sfxBisingGlitch;
    public AudioClip sfxKamuiHisap;

    [Header("Tujuan Selanjutnya")]
    public string namaSceneBoss = "SceneBoss";

    private bool wasKembaliDariBattle = false;
    private bool wasPlayerMenang = false;
    private PlayerOverworld playerMC;
    private Vector3 camOriginalPos;

    private void Awake()
    {
        wasKembaliDariBattle = GlobalBattleState.kembaliDariBattle;
        wasPlayerMenang = GlobalBattleState.playerMenang;
        playerMC = FindAnyObjectByType<PlayerOverworld>();
    }

    private void Start()
    {
        if (panelKamui != null) panelKamui.SetActive(false);

        // Hanya jalankan jika baru kembali dari pertarungan mengalahkan Velvet
        if (wasKembaliDariBattle && GlobalBattleState.dataMusuhAktif == dataVelvet && wasPlayerMenang)
        {
            // Jangan konsumsi kembaliDariBattle di sini agar PostBattleManager bisa mengembalikan posisi player dan memicu dialog menang
            StartCoroutine(SequenceKematianVelvet());
        }
    }

    private IEnumerator SequenceKematianVelvet()
    {
        // 1. Kunci Player
        if (playerMC != null) playerMC.enabled = false;

        // 2. Tunggu dialog pasca-battle bawaan selesai (Velvet: "Kau pikir...")
        // Beri jeda singkat agar PostBattleManager sempat memulai dialognya
        yield return new WaitForSeconds(0.3f);
        if (DialogManager.Instance != null)
        {
            while (DialogManager.Instance.sedangBicara)
            {
                yield return null;
            }
        }

        // Kunci ulang player (karena PostBattleManager mungkin membukanya)
        if (playerMC != null) playerMC.enabled = false;

        // 3. JALANKAN EFEK GLITCH VISUAL & SUARA SECARA LOOPING (JIKA DIAKTIFKAN)
        Coroutine glitchCoroutine = null;
        if (aktifkanGlitchPreKamui)
        {
            glitchCoroutine = StartCoroutine(KeepGlitchingDuringDialogue());
        }

        // 4. Putar dialog cemas Zero ("Apa yang sedang...") di saat layar sedang ngeglitch
        if (dialogZeroGlitch != null && DialogManager.Instance != null)
        {
            bool dialogSelesai = false;
            DialogManager.Instance.MulaiDialog(dialogZeroGlitch, () => dialogSelesai = true);
            while (!dialogSelesai) yield return null;
        }

        // 5. HENTIKAN GLITCH & RESET POSISI LAYAR KEMBALI NORMAL (JIKA DIAKTIFKAN)
        if (aktifkanGlitchPreKamui && glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
            ResetGlitchState();
        }

        // 6. DISTORSI DUNIA (Dinding menggelap, lampu jadi merah secara permanen)
        foreach (var dinding in dindingStudio)
        {
            if (dinding != null) dinding.color = warnaDindingGelap;
        }
        SetLampuMerah();

        // 7. Putar monolog Zero ("Tiba-tiba sesosok...") di lingkungan yang sudah gelap & merah
        if (dialogZeroKegelapan != null && DialogManager.Instance != null)
        {
            bool dialogSelesai = false;
            DialogManager.Instance.MulaiDialog(dialogZeroKegelapan, () => dialogSelesai = true);
            while (!dialogSelesai) yield return null;
        }

        // 8. MULAI EFEK KAMUI HISAP SECARA MULUS (Memutar layar masuk ke dalam + Zoom In Kamera)
        if (panelKamui != null && kamuiMaterial != null)
        {
            panelKamui.SetActive(true);
            if (audioSource != null && sfxKamuiHisap != null)
            {
                audioSource.clip = sfxKamuiHisap;
                audioSource.loop = false;
                audioSource.Play();
            }

            Camera mainCam = Camera.main;
            float originalOrthoSize = 5f;
            if (mainCam != null) originalOrthoSize = mainCam.orthographicSize;

            float elapsed = 0f;
            while (elapsed < durasiKamui)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / durasiKamui;

                // Transisi memutar dan menghisap layar secara bertahap
                kamuiMaterial.SetFloat("_Angle", Mathf.Lerp(0f, 38f, t));
                kamuiMaterial.SetFloat("_Progress", Mathf.Lerp(0f, 1f, t));

                // Zoom in kamera secara perlahan dengan mengecilkan orthographic size (semakin kecil size = semakin zoom)
                if (mainCam != null)
                {
                    mainCam.orthographicSize = Mathf.Lerp(originalOrthoSize, originalOrthoSize * 0.35f, t);
                }

                yield return null;
            }

            // Setelah hisapan selesai, matikan material Kamui dan jadikan panel hitam pekat (menutup semua sudut layar)
            UnityEngine.UI.RawImage rawImg = panelKamui.GetComponent<UnityEngine.UI.RawImage>();
            if (rawImg != null)
            {
                rawImg.material = null;
                rawImg.color = Color.black;
            }
            else
            {
                UnityEngine.UI.Image img = panelKamui.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    img.material = null;
                    img.color = Color.black;
                }
            }
        }

        // 9. MONOLOG TERAKHIR LAYAR HITAM ("Semua yang kulihat...")
        if (dialogLayarHitam != null && DialogManager.Instance != null)
        {
            bool dialogSelesai = false;
            DialogManager.Instance.MulaiDialog(dialogLayarHitam, () => dialogSelesai = true);
            while (!dialogSelesai) yield return null;
        }

        // 10. LOADING KE SCENE BOSS (Map Neraka)
        SceneManager.LoadScene(namaSceneBoss);
    }

    private IEnumerator KeepGlitchingDuringDialogue()
    {
        if (panelKamui != null && kamuiMaterial != null)
        {
            panelKamui.SetActive(true);
        }

        // Putar audio glitch secara berulang (loop)
        if (audioSource != null && sfxBisingGlitch != null)
        {
            audioSource.clip = sfxBisingGlitch;
            audioSource.loop = true;
            audioSource.Play();
        }

        Camera mainCam = Camera.main;
        if (mainCam != null) camOriginalPos = mainCam.transform.position;

        while (true)
        {
            // Acak nilai putaran dan tarikan shader untuk efek visual glitch
            float randomAngle = Random.Range(-8f, 8f);
            float randomProgress = Random.Range(0.04f, 0.12f);
            if (kamuiMaterial != null)
            {
                kamuiMaterial.SetFloat("_Angle", randomAngle);
                kamuiMaterial.SetFloat("_Progress", randomProgress);
            }

            // Guncang posisi kamera secara acak (Camera Shake)
            if (mainCam != null)
            {
                Vector3 shakeOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f);
                mainCam.transform.position = camOriginalPos + shakeOffset;
            }

            yield return new WaitForSeconds(0.04f);
        }
    }

    private void ResetGlitchState()
    {
        // Matikan suara glitch
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }

        // Kembalikan posisi kamera semula
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = camOriginalPos;
        }

        // Matikan panel Kamui sementara agar kembali normal sebelum dihisap
        if (panelKamui != null)
        {
            panelKamui.SetActive(false);
        }
        
        if (kamuiMaterial != null)
        {
            kamuiMaterial.SetFloat("_Angle", 0f);
            kamuiMaterial.SetFloat("_Progress", 0f);
        }
    }

    private void SetLampuAktif(bool aktif)
    {
        foreach (var lampu in lampuStudio)
        {
            if (lampu != null) lampu.SetActive(aktif);
        }
    }

    private void SetLampuMerah()
    {
        foreach (var lampu in lampuStudio)
        {
            if (lampu != null)
            {
                lampu.SetActive(true);
                SpriteRenderer sr = lampu.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.red;
                }
            }
        }
    }
}
