using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class WilayahDialog : MonoBehaviour
{
    [Header("Dialog Sebelum Battle")]
    [Tooltip("Dialog saat pertama kali bertemu musuh")]
    public DialogData dataDialog;

    [Header("Pengaturan Menuju Combat")]
    public bool pindahSceneSetelahDialog = false;
    public string namaSceneCombat = "SceneCombat";
    public OpponentData dataMusuh;

    [Header("Pengaturan Pasca-Battle (Setelah Combat)")]
    [Tooltip("Dialog yang muncul otomatis jika skormu MENGALAHKAN bot")]
    public DialogData dialogMenang;
    [Tooltip("Isi dengan nama scene tujuan jika menang (kosongkan jika tetap di Overworld)")]
    public string namaSceneSetelahMenang;
    
    [Tooltip("Dialog yang muncul otomatis jika skormu KALAH dari bot")]
    public DialogData dialogKalah;

    [Header("Pengaturan Lainnya")]
    public bool picuHanyaSekali = true;

    private bool sudahDipicu = false;

    private void Start()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (picuHanyaSekali && sudahDipicu) return;
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

                        // --- INJEKSI DATA PASCA-BATTLE KE MEMORI GLOBAL ---
                        GlobalBattleState.dialogMenang = dialogMenang;
                        GlobalBattleState.dialogKalah = dialogKalah;
                        GlobalBattleState.sceneSetelahMenang = namaSceneSetelahMenang;
                        GlobalBattleState.sceneOverworldAsal = SceneManager.GetActiveScene().name;
                        GlobalBattleState.namaSceneCombat = namaSceneCombat;
                        GlobalBattleState.dataMusuhAktif = dataMusuh;

                        if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaSceneCombat);
                        else SceneManager.LoadScene(namaSceneCombat);
                    }
                });
            }

            if (picuHanyaSekali) GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}