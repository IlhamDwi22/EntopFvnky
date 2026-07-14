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
    [Tooltip("Jarak maksimal player untuk berinteraksi (menekan tombol E)")]
    public float radiusInteraksi = 1.2f;
    [Tooltip("Jarak maksimal player untuk memunculkan ikon penunjuk / panah")]
    public float radiusDeteksi = 3f;
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
        if (Time.timeScale == 0f) return;

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
        bool dalamRadiusInteraksi = false;
        bool dalamRadiusDeteksi = false;
        
        float radiusTerbesar = Mathf.Max(radiusInteraksi, radiusDeteksi);
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radiusTerbesar);
        foreach (Collider2D c in cols) 
        { 
            if (c.CompareTag("Player")) 
            { 
                float jarak = Vector2.Distance(transform.position, c.transform.position);
                if (jarak <= radiusInteraksi) dalamRadiusInteraksi = true;
                if (jarak <= radiusDeteksi) dalamRadiusDeteksi = true;
                break; 
            } 
        }

        playerDiDekat = dalamRadiusInteraksi;

        if (dalamRadiusDeteksi)
        {
            if (ikonPanah != null) ikonPanah.SetActive(true);
        }
        else
        {
            if (ikonPanah != null) ikonPanah.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Jarak interaksi (magenta)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radiusInteraksi);

        // Jarak deteksi ikon (hijau)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusDeteksi);
    }
}