using UnityEngine;
using UnityEngine.SceneManagement;

public class PintuTransisi : MonoBehaviour
{
    [Header("Identitas Pintu Ini")]
    [Tooltip("Nama unik pintu ini (Contoh: Pintu_Hutan_Selatan)")]
    public string idPintuIni;

    [Header("Tujuan Teleportasi")]
    [Tooltip("Nama file Scene yang akan dituju")]
    public string namaSceneTujuan;
    [Tooltip("Nama unik pintu di Scene tujuan tempat player akan mendarat")]
    public string idPintuTujuan;

    [Header("Titik Mendarat Player")]
    [Tooltip("Taruh objek kosong 1 langkah di luar pintu agar player tidak terjebak")]
    public Transform titikMendarat;

    [Header("Sistem Interaksi (Manual)")]
    [Tooltip("Jarak maksimal player untuk berinteraksi (menekan tombol E)")]
    public float radiusInteraksi = 1.2f;
    [Tooltip("Jarak maksimal player untuk memunculkan ikon penunjuk / panah")]
    public float radiusDeteksi = 3f;
    [Tooltip("Masukkan objek gambar tanda seru/panah ke sini")]
    public GameObject ikonPanah;

    private bool playerDiDekat = false;

    private void Start()
    {
        // Sembunyikan ikon tanda seru saat game baru dimulai
        if (ikonPanah != null) ikonPanah.SetActive(false);

        // --- LOGIKA MENDARAT (DARI SCENE LAIN) ---
        if (DataPindahScene.idPintuTujuan == idPintuIni)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (titikMendarat != null)
                {
                    player.transform.position = titikMendarat.position;
                }
                else
                {
                    player.transform.position = transform.position + new Vector3(0, -1.5f, 0);
                }
            }
            // Hapus memori tujuan agar tidak teleport berulang-ulang
            DataPindahScene.idPintuTujuan = ""; 
        }
    }

    private void Update()
    {
        CekRadiusPlayer();

        // Jika player ada di dekat pintu dan menekan E
        if (playerDiDekat && Input.GetKeyDown(KeyCode.E))
        {
            // Sembunyikan ikon tanda seru saat layar mulai fade/loading
            if (ikonPanah != null) ikonPanah.SetActive(false);
            
            // Simpan tujuan ke memori global
            DataPindahScene.idPintuTujuan = idPintuTujuan;
            
            // Eksekusi perpindahan Scene
            if (SceneFader.Instance != null) 
            {
                SceneFader.Instance.PindahSceneDenganFade(namaSceneTujuan);
            }
            else 
            {
                SceneManager.LoadScene(namaSceneTujuan);
            }
        }
    }

    // --- SISTEM SENSOR RADIUS ---
    private void CekRadiusPlayer()
    {
        bool dalamRadiusInteraksi = false;
        bool dalamRadiusDeteksi = false;
        
        // Cek semua objek di dalam radius terbesar
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

    // --- SISTEM GIZMOS ---
    // Menggambar lingkaran di layar Scene Unity (Hanya terlihat saat objek pintu diklik)
    private void OnDrawGizmosSelected()
    {
        // Jarak interaksi (cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusInteraksi);

        // Jarak deteksi ikon (hijau)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusDeteksi);
    }
}