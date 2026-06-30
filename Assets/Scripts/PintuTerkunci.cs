using UnityEngine;
using UnityEngine.SceneManagement;

public class PintuTerkunci : MonoBehaviour
{
    [Header("Identitas Pintu Global")]
    public string idPintuGlobal;
    
    [Header("Pengaturan Puzzle Kunci")]
    public string idBarangDibutuhkan;
    public string namaScenePuzzle;

    [Header("Tujuan Teleportasi (Setelah Terbuka)")]
    public string namaSceneTujuan;
    public string idPintuTujuan;
    public Transform titikMendarat;

    [Header("Sistem Interaksi")]
    public float radiusInteraksi = 1.5f;
    public GameObject ikonPanah;

    private bool playerDiDekat = false;
    private string idPintuIni;

    private void Start()
    {
        idPintuIni = idPintuGlobal;
        if (ikonPanah != null) ikonPanah.SetActive(false);

        // MENDARAT SETELAH MENYELESAIKAN PUZZLE ATAU DARI SCENE LAIN
        if (DataPindahScene.idPintuTujuan == idPintuIni)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (titikMendarat != null) player.transform.position = titikMendarat.position;
                else player.transform.position = transform.position + new Vector3(0, -1.5f, 0);
            }
            DataPindahScene.idPintuTujuan = ""; 
        }
    }

    private void Update()
    {
        CekRadiusPlayer();

        if (playerDiDekat && Input.GetKeyDown(KeyCode.E))
        {
            if (ikonPanah != null) ikonPanah.SetActive(false);
            
            if (GlobalBattleState.daftarPintuTerbuka.Contains(idPintuGlobal))
            {
                // SUDAH TERBUKA -> Berfungsi normal seperti pintu teleportasi bolak-balik
                DataPindahScene.idPintuTujuan = idPintuTujuan;
                if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaSceneTujuan);
                else SceneManager.LoadScene(namaSceneTujuan);
            }
            else
            {
                // MASIH TERKUNCI -> Simpan semua data asal, lalu lempar ke Scene Puzzle
                GlobalBattleState.puzzle_sceneAsal = SceneManager.GetActiveScene().name; 
                GlobalBattleState.puzzle_idBarangWajib = idBarangDibutuhkan;
                GlobalBattleState.puzzle_idPintuGlobal = idPintuGlobal;
                GlobalBattleState.puzzle_sceneTujuan = namaSceneTujuan;
                GlobalBattleState.puzzle_idPintuTujuan = idPintuTujuan;

                if (SceneFader.Instance != null) SceneFader.Instance.PindahSceneDenganFade(namaScenePuzzle);
                else SceneManager.LoadScene(namaScenePuzzle);
            }
        }
    }

    private void CekRadiusPlayer()
    {
        bool terdeteksi = false;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radiusInteraksi);
        foreach (Collider2D c in cols) { if (c.CompareTag("Player")) { terdeteksi = true; break; } }

        if (terdeteksi && !playerDiDekat) { playerDiDekat = true; if (ikonPanah != null) ikonPanah.SetActive(true); }
        else if (!terdeteksi && playerDiDekat) { playerDiDekat = false; if (ikonPanah != null) ikonPanah.SetActive(false); }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radiusInteraksi);
    }
}