using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BossNPCSequence : MonoBehaviour
{
    public static int statusFaseBoss = 0; // 0 = default, 1 = sedang bertarung Phase 1, 2 = sedang bertarung Phase 2

    [Header("Data Pertarungan Musuh")]
    public OpponentData dataBossPhase1;
    public OpponentData dataBossPhase2;
    public string namaSceneRhythm = "Gameplay";

    [Header("UI Visual (Interaksi)")]
    public GameObject tombolInteraksiUI;

    [Header("Dialog Alur Boss")]
    public DialogData dialogSebelumBattle;
    [Tooltip("Dialog setelah menang Phase 1 (Boss B muncul / Boss A kalah)")]
    public DialogData dialogTransisiPhase;
    [Tooltip("Dialog setelah menang Phase 2 (Peti/Boss kalah permanen)")]
    public DialogData dialogSelesaiBoss;

    [Header("Tujuan Selanjutnya")]
    public string namaSceneSetelahMenang;

    private bool playerDiDekat = false;
    private bool wasKembaliDariBattle = false;
    private bool wasPlayerMenang = false;
    private PlayerOverworld playerMC;

    private void Awake()
    {
        wasKembaliDariBattle = GlobalBattleState.kembaliDariBattle;
        wasPlayerMenang = GlobalBattleState.playerMenang;
        playerMC = FindFirstObjectByType<PlayerOverworld>();
    }

    private void Start()
    {
        if (tombolInteraksiUI != null) tombolInteraksiUI.SetActive(false);

        // Memori Penyimpanan Status Pembukaan Peti/Hancurnya Bos
        // Jika Boss sudah dikalahkan sepenuhnya (Phase 2 menang), hancurkan objek bos ini dari scene
        if (GlobalBattleState.CekPetiTerbuka(gameObject.name + "_Kalah"))
        {
            Destroy(gameObject);
            return;
        }

        if (wasKembaliDariBattle)
        {
            if (statusFaseBoss == 1 && wasPlayerMenang)
            {
                MulaiTransisiPhase2();
            }
            else if (statusFaseBoss == 2 && wasPlayerMenang)
            {
                MulaiVictorySequence();
            }
            else
            {
                // Jika kalah di salah satu phase, statusFaseBoss tetap bertahan sehingga jika mereka menekan tombol retry
                // di menu kalah, statusnya tidak kacau.
            }
        }
    }

    private void Update()
    {
        // Interaksi awal hanya aktif jika bos belum dilawan (statusFaseBoss == 0)
        if (statusFaseBoss == 0 && playerDiDekat && Input.GetKeyDown(KeyCode.E))
        {
            MulaiSequenceBoss();
        }
    }

    private void MulaiSequenceBoss()
    {
        if (tombolInteraksiUI != null) tombolInteraksiUI.SetActive(false);

        if (dialogSebelumBattle != null && DialogManager.Instance != null)
        {
            if (playerMC != null) playerMC.enabled = false; // Kunci pergerakan player
            DialogManager.Instance.MulaiDialog(dialogSebelumBattle, () =>
            {
                MulaiBattlePhase1();
            });
        }
        else
        {
            MulaiBattlePhase1();
        }
    }

    private void MulaiBattlePhase1()
    {
        statusFaseBoss = 1;

        // Siapkan memori battle untuk Phase 1
        GameManager.musuhPilihanSaatIni = dataBossPhase1;
        GameManager.urutanLaguSaatIni = 0;

        GlobalBattleState.dialogMenang = null; // Kita yang handle dialog menangnya secara kustom
        GlobalBattleState.dialogKalah = null; // Menyerahkan UI Retry bawaan ke PostBattleManager
        GlobalBattleState.sceneSetelahMenang = ""; // Biar kembali ke overworld scene asal
        GlobalBattleState.sceneOverworldAsal = SceneManager.GetActiveScene().name;
        GlobalBattleState.namaSceneCombat = namaSceneRhythm;
        GlobalBattleState.dataMusuhAktif = dataBossPhase1;

        if (playerMC != null)
        {
            GlobalBattleState.posisiPlayerTerakhir = playerMC.transform.position;
            GlobalBattleState.adaPosisiTersimpan = true;
        }

        // Loading scene battle
        if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaSceneRhythm);
        else SceneManager.LoadScene(namaSceneRhythm);
    }

    private void MulaiTransisiPhase2()
    {
        if (playerMC != null) playerMC.enabled = false; // Kunci player saat dialog

        if (dialogTransisiPhase != null && DialogManager.Instance != null)
        {
            DialogManager.Instance.MulaiDialog(dialogTransisiPhase, () =>
            {
                MulaiBattlePhase2();
            });
        }
        else
        {
            MulaiBattlePhase2();
        }
    }

    private void MulaiBattlePhase2()
    {
        statusFaseBoss = 2;

        // Siapkan memori battle untuk Phase 2
        GameManager.musuhPilihanSaatIni = dataBossPhase2;
        GameManager.urutanLaguSaatIni = 0;

        GlobalBattleState.dialogMenang = null;
        GlobalBattleState.dialogKalah = null;
        GlobalBattleState.sceneSetelahMenang = "";
        GlobalBattleState.sceneOverworldAsal = SceneManager.GetActiveScene().name;
        GlobalBattleState.namaSceneCombat = namaSceneRhythm;
        GlobalBattleState.dataMusuhAktif = dataBossPhase2;

        if (playerMC != null)
        {
            GlobalBattleState.posisiPlayerTerakhir = playerMC.transform.position;
            GlobalBattleState.adaPosisiTersimpan = true;
        }

        // Loading scene battle untuk phase 2
        if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaSceneRhythm);
        else SceneManager.LoadScene(namaSceneRhythm);
    }

    private void MulaiVictorySequence()
    {
        if (playerMC != null) playerMC.enabled = false;

        // Tandai bos ini sudah dikalahkan permanen agar tidak muncul lagi jika scene dimuat ulang
        GlobalBattleState.BukaPeti(gameObject.name + "_Kalah");
        statusFaseBoss = 0; // Reset status

        if (dialogSelesaiBoss != null && DialogManager.Instance != null)
        {
            DialogManager.Instance.MulaiDialog(dialogSelesaiBoss, () =>
            {
                PindahKeSceneTujuanAkhir();
            });
        }
        else
        {
            PindahKeSceneTujuanAkhir();
        }
    }

    private void PindahKeSceneTujuanAkhir()
    {
        if (!string.IsNullOrEmpty(namaSceneSetelahMenang))
        {
            if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaSceneSetelahMenang);
            else SceneManager.LoadScene(namaSceneSetelahMenang);
        }
        else
        {
            if (playerMC != null) playerMC.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (statusFaseBoss == 0 && collision.CompareTag("Player"))
        {
            playerDiDekat = true;
            if (tombolInteraksiUI != null) tombolInteraksiUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDiDekat = false;
            if (tombolInteraksiUI != null) tombolInteraksiUI.SetActive(false);
        }
    }
}
