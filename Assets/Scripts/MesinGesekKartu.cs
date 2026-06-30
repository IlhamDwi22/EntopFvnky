using UnityEngine;
using UnityEngine.SceneManagement;

public class MesinGesekKartu : MonoBehaviour
{
    public static MesinGesekKartu Instance;

    [Header("Pengaturan Akses Kartu")]
    public string idKartuYangBenar;

    [Header("Pengaturan Drag & Drop")]
    public GameObject prefabKartuGesek;
    public Transform titikMunculKartu;
    public Collider2D areaScanner;

    [Header("Visual Pintu")]
    public SpriteRenderer backgroundPintu;
    public Sprite gambarPintuTerbuka;

    [Header("Dialog Feedback")]
    public DialogData dialogBerhasil;
    public DialogData dialogGagal;

    private GameObject kartuAktif;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void MunculkanBarangDiLayar(string idBarang, Sprite ikonBarang)
    {
        if (kartuAktif != null) Destroy(kartuAktif);

        if (prefabKartuGesek != null && titikMunculKartu != null)
        {
            kartuAktif = Instantiate(prefabKartuGesek, titikMunculKartu.position, Quaternion.identity);
            KartuBisaDigeser scriptKartu = kartuAktif.GetComponent<KartuBisaDigeser>();
            
            if (scriptKartu != null)
            {
                scriptKartu.SetupKartu(idBarang, ikonBarang, areaScanner);
            }
        }
    }

    public void VerifikasiGesekan(string idBarangYangDigesek)
    {
        Destroy(kartuAktif); 

        if (idBarangYangDigesek == idKartuYangBenar || idBarangYangDigesek == GlobalBattleState.puzzle_idBarangWajib)
        {
            // KARTU BENAR
            if (backgroundPintu != null && gambarPintuTerbuka != null)
            {
                backgroundPintu.sprite = gambarPintuTerbuka; 
            }

            if (!string.IsNullOrEmpty(GlobalBattleState.puzzle_idPintuGlobal))
            {
                if (!GlobalBattleState.daftarPintuTerbuka.Contains(GlobalBattleState.puzzle_idPintuGlobal))
                {
                    GlobalBattleState.daftarPintuTerbuka.Add(GlobalBattleState.puzzle_idPintuGlobal);
                }
            }

            if (DialogManager.Instance != null && dialogBerhasil != null)
            {
                DialogManager.Instance.MulaiDialog(dialogBerhasil, () => 
                {
                    KembaliKePintuAsal(); // Memanggil fungsi kembali setelah dialog sukses
                });
            }
            else
            {
                KembaliKePintuAsal(); 
            }
        }
        else
        {
            // KARTU SALAH
            if (DialogManager.Instance != null && dialogGagal != null)
            {
                DialogManager.Instance.MulaiDialog(dialogGagal, null);
            }
        }
    }

    // --- FUNGSI BARU UNTUK TOMBOL BACK ---
    public void KembaliKePintuAsal()
    {
        // Mengecek apakah ada data asal tempat player berasal
        if (!string.IsNullOrEmpty(GlobalBattleState.puzzle_sceneAsal))
        {
            // Mengatur titik mendarat player tepat di depan pintu yang diklik sebelumnya
            DataPindahScene.idPintuTujuan = GlobalBattleState.puzzle_idPintuGlobal; 
            
            if (SceneFader.Instance != null) 
            {
                SceneFader.Instance.PindahSceneDenganFade(GlobalBattleState.puzzle_sceneAsal);
            }
            else 
            {
                SceneManager.LoadScene(GlobalBattleState.puzzle_sceneAsal);
            }
        }
        else
        {
            Debug.LogWarning("Data scene asal kosong! Pastikan player masuk ke sini melalui interaksi pintu.");
        }
    }
}