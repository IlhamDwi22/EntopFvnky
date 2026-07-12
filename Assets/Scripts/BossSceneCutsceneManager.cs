using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossSceneCutsceneManager : MonoBehaviour
{
    public static int bossFightState = 0; // 0 = awal, 1 = kembali dari phase 1, 2 = kembali dari phase 2

    [Header("Data Pertarungan Musuh")]
    public OpponentData dataBossPhase1;
    public OpponentData dataBossPhase2;
    public string namaSceneRhythm = "Gameplay";

    [Header("Dialog Alur Boss")]
    [Tooltip("Dialog monolog saat Zero baru mendarat di neraka (sebelum KROWN muncul/jalan)")]
    public DialogData dialogIntroMonolog;
    public DialogData dialogSebelumBattle;
    [Tooltip("Dialog setelah menang Phase 1 (Boss B muncul / Boss A kalah)")]
    public DialogData dialogTransisiPhase;
    [Tooltip("Dialog setelah menang Phase 2 (Peti/Boss kalah permanen)")]
    public DialogData dialogSelesaiBoss;

    [Header("Pengaturan Pergerakan Boss")]
    [Tooltip("Tarik objek Boss Anda ke sini")]
    public GameObject bossObject;
    [Tooltip("Animator milik Bos untuk memutar animasi jalan")]
    public Animator bossAnimator;
    [Tooltip("Nama parameter Boolean di Animator Bos (contoh: IsWalking)")]
    public string namaParameterJalan = "IsWalking";
    [Tooltip("Titik tujuan jalan Bos di depan Player")]
    public Transform titikTujuanJalan;
    [Tooltip("Kecepatan jalan Bos bergeser")]
    public float kecepatanJalan = 2f;

    [Header("Tujuan Selanjutnya")]
    public string namaSceneSetelahMenang;

    private PlayerOverworld playerMC;
    private bool wasKembaliDariBattle = false;
    private bool wasPlayerMenang = false;

    private void Awake()
    {
        wasKembaliDariBattle = GlobalBattleState.kembaliDariBattle;
        wasPlayerMenang = GlobalBattleState.playerMenang;
    }

    private void Start()
    {
        playerMC = FindAnyObjectByType<PlayerOverworld>();

        // Jika Boss sudah kalah permanen, hancurkan objek bos/cutscene ini dari scene
        if (GlobalBattleState.CekPetiTerbuka(gameObject.name + "_Kalah"))
        {
            if (bossObject != null) Destroy(bossObject);
            Destroy(gameObject);
            return;
        }

        if (!wasKembaliDariBattle)
        {
            // Fresh load scene: Mulai dari awal (Phase 0)
            bossFightState = 0;
            StartCoroutine(SequenceIntroBoss());
        }
        else
        {
            // Kembali dari battle
            if (bossFightState == 1 && wasPlayerMenang)
            {
                // Menang Phase 1 -> Posisikan bos langsung di titik tujuan, lalu putar transisi ke Phase 2
                TeleportBossKeTujuan();
                StartCoroutine(SequenceTransisiPhase2());
            }
            else if (bossFightState == 2 && wasPlayerMenang)
            {
                // Menang Phase 2 -> Sukses total
                TeleportBossKeTujuan();
                StartCoroutine(SequenceVictoryBoss());
            }
        }
    }

    private void TeleportBossKeTujuan()
    {
        if (bossObject != null && titikTujuanJalan != null)
        {
            bossObject.transform.position = titikTujuanJalan.position;
        }
    }

    // --- 1. SEQUENCE INTRO (BARU MASUK SCENE) ---
    private IEnumerator SequenceIntroBoss()
    {
        // Tunggu sebentar agar loading scene selesai sempurna
        yield return new WaitForSeconds(0.2f);

        // Kunci pergerakan player
        if (playerMC != null)
        {
            playerMC.enabled = false;
            Rigidbody2D rb = playerMC.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        // 1. Putar monolog awal Zero ("Tempat ini... terasa seperti neraka")
        if (dialogIntroMonolog != null && DialogManager.Instance != null)
        {
            bool monologSelesai = false;
            DialogManager.Instance.MulaiDialog(dialogIntroMonolog, () => monologSelesai = true);
            while (!monologSelesai) yield return null;
        }

        // 2. Jalankan pergerakan Boss menghampiri Player
        if (bossObject != null && titikTujuanJalan != null)
        {
            // Nyalakan parameter animasi jalan di Animator
            if (bossAnimator != null && !string.IsNullOrEmpty(namaParameterJalan))
            {
                bossAnimator.SetBool(namaParameterJalan, true);
            }

            // Gerakkan objek Boss secara bertahap menuju titikTujuanJalan
            Vector3 targetPos = titikTujuanJalan.position;
            while (Vector3.Distance(bossObject.transform.position, targetPos) > 0.05f)
            {
                bossObject.transform.position = Vector3.MoveTowards(
                    bossObject.transform.position,
                    targetPos,
                    kecepatanJalan * Time.deltaTime
                );
                yield return null;
            }

            // Posisikan pas di target
            bossObject.transform.position = targetPos;

            // Matikan parameter animasi jalan
            if (bossAnimator != null && !string.IsNullOrEmpty(namaParameterJalan))
            {
                bossAnimator.SetBool(namaParameterJalan, false);
            }
        }

        // Setelah bos sampai, putar dialog sebelum battle (KROWN menantang player)
        if (dialogSebelumBattle != null && DialogManager.Instance != null)
        {
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
        bossFightState = 1;

        // Siapkan memori pertarungan untuk Phase 1
        GameManager.musuhPilihanSaatIni = dataBossPhase1;
        GameManager.urutanLaguSaatIni = 0;

        GlobalBattleState.dialogMenang = null; // Dialog kita ambil alih secara kustom di skrip ini
        GlobalBattleState.dialogKalah = null; 
        GlobalBattleState.sceneSetelahMenang = ""; 
        GlobalBattleState.sceneOverworldAsal = SceneManager.GetActiveScene().name;
        GlobalBattleState.namaSceneCombat = namaSceneRhythm;
        GlobalBattleState.dataMusuhAktif = dataBossPhase1;

        if (playerMC != null)
        {
            GlobalBattleState.posisiPlayerTerakhir = playerMC.transform.position;
            GlobalBattleState.adaPosisiTersimpan = true;
        }

        // Pindah ke scene pertarungan ritme
        if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaSceneRhythm);
        else SceneManager.LoadScene(namaSceneRhythm);
    }

    // --- 2. SEQUENCE TRANSISI (SETELAH PHASE 1 MENANG) ---
    private IEnumerator SequenceTransisiPhase2()
    {
        yield return new WaitForSeconds(0.2f);

        if (playerMC != null) playerMC.enabled = false;

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
        bossFightState = 2;

        // Siapkan memori pertarungan untuk Phase 2
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

        // Pindah ke scene pertarungan ritme untuk Phase 2
        if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaSceneRhythm);
        else SceneManager.LoadScene(namaSceneRhythm);
    }

    // --- 3. SEQUENCE KEMENANGAN AKHIR (SETELAH PHASE 2 MENANG) ---
    private IEnumerator SequenceVictoryBoss()
    {
        yield return new WaitForSeconds(0.2f);

        if (playerMC != null) playerMC.enabled = false;

        // Kunci agar bos tidak muncul lagi jika scene dimuat ulang
        GlobalBattleState.BukaPeti(gameObject.name + "_Kalah");
        bossFightState = 0; // Reset state

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
}
