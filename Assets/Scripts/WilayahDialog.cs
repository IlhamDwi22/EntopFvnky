using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class WilayahDialog : MonoBehaviour
{
    [Header("Dialog Sebelum Battle")]
    public DialogData dataDialog;

    [Header("Pengaturan Menuju Combat")]
    public bool pindahSceneSetelahDialog = false;
    public string namaSceneCombat = "SceneCombat";
    public OpponentData dataMusuh;

    [Header("Pengaturan Pasca-Battle (Setelah Combat)")]
    public DialogData dialogMenang;
    public string namaSceneSetelahMenang;
    public DialogData dialogKalah;

    [Header("Pengaturan Lainnya")]
    public bool picuHanyaSekali = true;

    private bool sudahDipicu = false;
    
    // PENYELAMAT: Timer agar musuh tidak memicu dialog instan saat kita baru spawn di depannya
    private float timerAman = 0f; 

    private void Start()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        // Menghitung waktu sejak scene dimuat (berhenti di angka 2 detik agar tidak memberatkan memori)
        if (timerAman < 2f) timerAman += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // JIKA SCENE BARU SAJA DIMUAT, ABAIKAN TABRAKAN!
            if (timerAman < 0.5f) return; 

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