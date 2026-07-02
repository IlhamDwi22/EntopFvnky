using UnityEngine;

public class PetiPassword : MonoBehaviour
{
    public static string passwordRahasiaSaatIni;

    [Header("Identitas Peti (Unique)")]
    [Tooltip("ID unik peti. Harus berbeda untuk setiap peti di game! Jika kosong, otomatis menggunakan nama GameObject.")]
    public string idPeti;

    [Header("Deteksi Player (Gizmos)")]
    [Tooltip("Jarak maksimal player untuk melakukan interaksi (menekan tombol E)")]
    public float radiusInteraksi = 1.5f;
    [Tooltip("Jarak maksimal player untuk memunculkan ikon penunjuk / panah")]
    public float radiusDeteksi = 3.5f;

    [Header("Pengaturan Hadiah Peti")]
    public string idBarangHadiah = "DokumenPenting";
    
    [Header("Dialog Feedback")]
    public DialogData dialogBerhasil;
    public DialogData dialogGagal;

    [Header("Visual Indikator")]
    public GameObject ikonPanah;

    private bool playerDiDekat = false;
    private bool sudahTerbuka = false;

    private string GetIDPeti()
    {
        return string.IsNullOrEmpty(idPeti) ? gameObject.name : idPeti;
    }

    private void Awake()
    {
        int angkaAcak = Random.Range(0, 10000);
        passwordRahasiaSaatIni = angkaAcak.ToString("D4"); 
    }

    private void Start()
    {
        // --- KODE BARU: CEK MEMORI ---
        // Cek apakah peti ini sudah pernah dibuka sebelumnya
        if (GlobalBattleState.CekPetiTerbuka(GetIDPeti()))
        {
            Destroy(gameObject);
            return; // Hentikan proses
        }

        if (ikonPanah != null) ikonPanah.SetActive(false);
    }

    private void Update()
    {
        CekRadiusPlayer();

        if (playerDiDekat && !sudahTerbuka && Input.GetKeyDown(KeyCode.E))
        {
            if (ikonPanah != null) ikonPanah.SetActive(false); 

            if (UIInputPassword.Instance != null)
            {
                UIInputPassword.Instance.BukaLayarPassword(this);
            }
        }
    }

    // --- SISTEM SENSOR RADIUS ---
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

        // Logika menyalakan/mematikan ikon
        if (dalamRadiusDeteksi && !sudahTerbuka)
        {
            if (ikonPanah != null) ikonPanah.SetActive(true);
        }
        else
        {
            if (ikonPanah != null) ikonPanah.SetActive(false);
        }
    }

    // --- SISTEM GIZMOS ---
    private void OnDrawGizmosSelected()
    {
        // Jarak interaksi (kuning)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusInteraksi);

        // Jarak deteksi ikon (hijau)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusDeteksi);
    }

    public void VerifikasiPassword(string inputUser)
    {
        if (inputUser == passwordRahasiaSaatIni)
        {
            sudahTerbuka = true;
            if (ikonPanah != null) ikonPanah.SetActive(false); 
            
            GlobalBattleState.BukaPeti(GetIDPeti()); // Simpan status peti terbuka
            GlobalBattleState.TambahBarang(idBarangHadiah);
            
            if (DialogManager.Instance != null && dialogBerhasil != null)
            {
                DialogManager.Instance.MulaiDialog(dialogBerhasil, () => { Destroy(gameObject); });
            }
            else Destroy(gameObject);
        }
        else
        {
            if (playerDiDekat && ikonPanah != null) ikonPanah.SetActive(true);

            if (DialogManager.Instance != null && dialogGagal != null)
            {
                DialogManager.Instance.MulaiDialog(dialogGagal, null);
            }
        }
    }
}