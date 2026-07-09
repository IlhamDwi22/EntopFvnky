using UnityEngine;
using UnityEngine.SceneManagement;

public class PostBattleManager : MonoBehaviour
{
    [Header("UI Pilihan Kalah")]
    public GameObject panelPilihanRetry;

    private PlayerOverworld playerMC;

    private void Start()
    {
        if (panelPilihanRetry != null) panelPilihanRetry.SetActive(false);
        playerMC = FindFirstObjectByType<PlayerOverworld>();

        if (GlobalBattleState.kembaliDariBattle)
        {
            GlobalBattleState.kembaliDariBattle = false; 

            if (GlobalBattleState.adaPosisiTersimpan && playerMC != null)
            {
                playerMC.transform.position = GlobalBattleState.posisiPlayerTerakhir;
            }

            // Beri jeda 0.1 detik agar sistem DialogManager bersiap sepenuhnya
            Invoke("ProsesDialogPascaBattle", 0.1f);
        }
    }

    private void ProsesDialogPascaBattle()
    {
        if (GlobalBattleState.playerMenang)
        {
            if (GlobalBattleState.dialogMenang != null)
            {
                DialogManager.Instance.MulaiDialog(GlobalBattleState.dialogMenang, () => { PindahKeSceneSelanjutnya(); });
            }
            else
            {
                PindahKeSceneSelanjutnya(); 
            }
        }
        else
        {
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

    private void PindahKeSceneSelanjutnya()
    {
        if (!string.IsNullOrEmpty(GlobalBattleState.sceneSetelahMenang))
        {
            if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(GlobalBattleState.sceneSetelahMenang);
            else SceneManager.LoadScene(GlobalBattleState.sceneSetelahMenang);
        }
        else
        {
            if (playerMC != null) playerMC.enabled = true;
        }
    }

    private void TampilkanPilihanRetry()
    {
        if (panelPilihanRetry != null)
        {
            panelPilihanRetry.SetActive(true);
            
            if (playerMC != null)
            {
                playerMC.enabled = false;
                Rigidbody2D rb = playerMC.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
                Animator anim = playerMC.GetComponent<Animator>();
                if (anim != null) anim.SetFloat("Speed", 0f);
            }
        }
        else
        {
            Debug.LogError("[PostBattleManager] GAGAL MEMUNCULKAN MENU! Kamu belum memasukkan UI Panel Kalah ke Inspector PostBattleManager!");
        }
    }

    public void TombolUlangiBattle()
    {
        if (panelPilihanRetry != null) panelPilihanRetry.SetActive(false);
        
        GameManager.musuhPilihanSaatIni = GlobalBattleState.dataMusuhAktif;
        GameManager.urutanLaguSaatIni = 0;

        if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(GlobalBattleState.namaSceneCombat);
        else SceneManager.LoadScene(GlobalBattleState.namaSceneCombat);
    }

    public void TombolBatal()
    {
        if (panelPilihanRetry != null) panelPilihanRetry.SetActive(false);
        
        // Reset status fase boss
        BossSceneCutsceneManager.bossFightState = 0;

        // Buka kunci pergerakan MC
        if (playerMC != null) playerMC.enabled = true; 
    }
}