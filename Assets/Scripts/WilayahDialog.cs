using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class WilayahDialog : MonoBehaviour
{
    [Header("Dialog Sebelum Battle (Jika Barang Sudah Ada / Tanpa Syarat)")]
    public DialogData dataDialog;

    [Header("Pengaturan Menuju Combat")]
    public bool pindahSceneSetelahDialog = false;
    public string namaSceneCombat = "SceneCombat";
    public OpponentData dataMusuh;

    [Header("Pengaturan Pasca-Battle (Setelah Combat)")]
    public DialogData dialogMenang;
    public string namaSceneSetelahMenang;
    public DialogData dialogKalah;

    [Header("SISTEM PENGUNCIAN BARANG QUEST (BARU)")]
    [Tooltip("Centang ini jika musuh ini mewajibkan player mencari barang terlebih dahulu")]
    public bool butuhBarangQuest = false;
    [Tooltip("Isi dengan ID Teks barang yang wajib dibawa player (Misal: KunciLevel2)")]
    public string IDBarangYangDibutuhkan = "KunciLevel2";
    [Tooltip("Dialog penolakan yang muncul jika player nekat menabrak musuh tapi belum bawa barangnya")]
    public DialogData dialogJikaBarangBelumAda;

    [Header("Pengaturan Lainnya")]
    public bool picuHanyaSekali = true;

    private bool sudahDipicu = false;
    private float timerAman = 0f; 

    private void Start()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        if (timerAman < 2f) timerAman += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (timerAman < 0.5f) return; 
            if (picuHanyaSekali && sudahDipicu) return;

            // --- SISTEM PEMERIKSAAN KANTONG BARANG ---
            if (butuhBarangQuest)
            {
                // Minta bantuan GlobalBattleState untuk mengecek isi list
                bool memilikiBarang = GlobalBattleState.CekBarang(IDBarangYangDibutuhkan);

                if (!memilikiBarang)
                {
                    // Jika player tidak punya barangnya, putar dialog penolakan
                    if (dialogJikaBarangBelumAda != null && DialogManager.Instance != null)
                    {
                        DialogManager.Instance.MulaiDialog(dialogJikaBarangBelumAda, () => 
                        {
                            // Kosong. Player dibebaskan bergerak kembali untuk mencari barang di map
                        });
                    }
                    else
                    {
                        Debug.LogWarning("[WilayahDialog] Player dihadang quest, tapi file 'dialogJikaBarangBelumAda' belum diisi!");
                    }
                    
                    return; // BLOKIR JALUR: Menghentikan paksa kodingan di sini agar tidak masuk ke area combat!
                }
            }

            // --- JALUR NORMAL (JIKA SYARAT QUEST SUDAH TERPENOJI ATAU TIDAK DIKUNCI) ---
            if (dataDialog == null) return;
            sudahDipicu = true;

            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.MulaiDialog(dataDialog, () => 
                {
                    if (pindahSceneSetelahDialog)
                    {
                        if (dataMusuh != null)
                        {
                            GameManager.musuhPilihanSaatIni = dataMusuh;
                            GameManager.urutanLaguSaatIni = 0; 
                        }

                        GlobalBattleState.dialogMenang = dialogMenang;
                        GlobalBattleState.dialogKalah = dialogKalah;
                        GlobalBattleState.sceneSetelahMenang = namaSceneSetelahMenang;
                        GlobalBattleState.sceneOverworldAsal = SceneManager.GetActiveScene().name;
                        GlobalBattleState.namaSceneCombat = namaSceneCombat;
                        GlobalBattleState.dataMusuhAktif = dataMusuh;
                        
                        GlobalBattleState.posisiPlayerTerakhir = other.transform.position;
                        GlobalBattleState.adaPosisiTersimpan = true;

                        if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaSceneCombat);
                        else SceneManager.LoadScene(namaSceneCombat);
                    }
                });
            }

            if (picuHanyaSekali) GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}