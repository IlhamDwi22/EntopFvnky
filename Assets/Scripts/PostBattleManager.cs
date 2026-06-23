using UnityEngine;
using UnityEngine.SceneManagement;

public class PostBattleManager : MonoBehaviour
{
    [Header("UI Pilihan Kalah")]
    [Tooltip("Tarik GameObject Panel yang berisi tombol 'Ulangi' dan 'Nyerah/Batal' ke sini")]
    public GameObject panelPilihanRetry;

    private PlayerOverworld playerMC;

    private void Start()
    {
        if (panelPilihanRetry != null) panelPilihanRetry.SetActive(false);
        playerMC = FindObjectOfType<PlayerOverworld>();

        // Cek apakah pemain baru saja pulang dari Scene Combat
        if (GlobalBattleState.kembaliDariBattle)
        {
            GlobalBattleState.kembaliDariBattle = false; // Matikan status agar tidak looping

            if (GlobalBattleState.playerMenang)
            {
                // JIKA MENANG: Panggil Dialog Menang, lalu pindah ke Scene Selanjutnya
                if (GlobalBattleState.dialogMenang != null)
                {
                    DialogManager.Instance.MulaiDialog(GlobalBattleState.dialogMenang, () => { PindahKeSceneSelanjutnya(); });
                }
                else
                {
                    PindahKeSceneSelanjutnya(); // Jika tak ada dialog, langsung pindah
                }
            }
            else
            {
                // JIKA KALAH: Panggil Dialog Kalah, lalu munculkan Panel Pilihan
                if (GlobalBattleState.dialogKalah != null)
                {
                    DialogManager.Instance.MulaiDialog(GlobalBattleState.dialogKalah, () => { TampilkanPilihanRetry(); });
                }
                else
                {
                    TampilkanPilihanRetry();
                }
            }
        }
    }

    private void PindahKeSceneSelanjutnya()
    {
        if (!string.IsNullOrEmpty(GlobalBattleState.sceneSetelahMenang))
        {
            if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(GlobalBattleState.sceneSetelahMenang);
            else SceneManager.LoadScene(GlobalBattleState.sceneSetelahMenang);
        }
    }

    private void TampilkanPilihanRetry()
    {
        if (panelPilihanRetry != null)
        {
            panelPilihanRetry.SetActive(true);
            
            // Kunci total pergerakan MC saat panel terbuka
            if (playerMC != null)
            {
                playerMC.enabled = false;
                Rigidbody2D rb = playerMC.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
                Animator anim = playerMC.GetComponent<Animator>();
                if (anim != null) anim.SetFloat("Speed", 0f);
            }
        }
    }

    // Fungsi ini dipanggil dari Tombol UI "Ulangi"
    public void TombolUlangiBattle()
    {
        if (panelPilihanRetry != null) panelPilihanRetry.SetActive(false);
        
        GameManager.musuhPilihanSaatIni = GlobalBattleState.dataMusuhAktif;
        GameManager.urutanLaguSaatIni = 0;

        if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(GlobalBattleState.namaSceneCombat);
        else SceneManager.LoadScene(GlobalBattleState.namaSceneCombat);
    }

    // Fungsi ini dipanggil dari Tombol UI "Batal"
    public void TombolBatal()
    {
        if (panelPilihanRetry != null) panelPilihanRetry.SetActive(false);
        if (playerMC != null) playerMC.enabled = true; // Buka kunci gerakan MC
    }
}