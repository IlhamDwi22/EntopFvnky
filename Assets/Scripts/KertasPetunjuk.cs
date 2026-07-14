using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class KertasPetunjuk : MonoBehaviour
{
    [Header("Identitas Item (Unique)")]
    [Tooltip("ID unik kertas. Harus beda antara kertas satu dengan yang lain! Jika kosong, akan otomatis menggunakan nama GameObject.")]
    public string idKertas;

    [Header("Deteksi Player (Gizmos)")]
    [Tooltip("Jarak maksimal player untuk melakukan interaksi (menekan tombol E)")]
    public float radiusInteraksi = 1.2f;
    [Tooltip("Jarak maksimal player untuk memunculkan ikon penunjuk / panah")]
    public float radiusDeteksi = 3f;

    [Header("UI Kertas")]
    public GameObject panelKertas;
    public TextMeshProUGUI teksCatatan;

    [Header("Visual Indikator")]
    public GameObject ikonPanah;

    [Header("Isi Tulisan Kertas")]
    [Tooltip("Centang ini jika kertas ini khusus untuk menampilkan password acak brankas darah secara dinamis.")]
    public bool menampilkanPasswordBrankas = false;

    [Tooltip("Isi tulisan jika tidak menampilkan password brankas.")]
    [TextArea(5, 10)]
    public string isiTulisan = "Tulis pesan kertas di sini...";

    private bool playerDiDekat = false;
    private bool sedangMembaca = false;
    private bool sudahDiambil = false;
    private SpriteRenderer spriteRenderer;

    private string GetItemID()
    {
        return string.IsNullOrEmpty(idKertas) ? gameObject.name : idKertas;
    }

    private string DapatkanIsiTeks()
    {
        if (menampilkanPasswordBrankas)
        {
            return "Sebuah catatan bernoda darah...\n\n\"Kombinasi brankas hari ini adalah: <color=red>" + PetiPassword.passwordRahasiaSaatIni + "</color>\"";
        }
        return isiTulisan;
    }

    private void Start()
    {
        // --- KODE BARU: CEK MEMORI ---
        // Jika id kertas ini sudah tercatat ada di dalam tas, langsung hancurkan!
        if (GlobalBattleState.CekBarang(GetItemID()))
        {
            Destroy(gameObject);
            return; // Hentikan proses agar kode di bawahnya tidak error
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (panelKertas != null) panelKertas.SetActive(false);
        if (ikonPanah != null) ikonPanah.SetActive(false); 
    }

    private void Update()
    {
        if (Time.timeScale == 0f && !sedangMembaca) return;

        CekRadiusPlayer();

        // Logika saat tombol E ditekan
        if (playerDiDekat && Input.GetKeyDown(KeyCode.E))
        {
            sedangMembaca = !sedangMembaca;
            if (panelKertas != null) panelKertas.SetActive(sedangMembaca);

            if (sedangMembaca)
            {
                if (ikonPanah != null) ikonPanah.SetActive(false);

                if (teksCatatan != null)
                {
                    teksCatatan.text = DapatkanIsiTeks();
                }
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
                if (!sudahDiambil)
                {
                    sudahDiambil = true;
                    string idItem = GetItemID();
                    GlobalBattleState.TambahItemRuntime(idItem, spriteRenderer.sprite, DapatkanIsiTeks());
                    GlobalBattleState.TambahBarang(idItem);
                    Destroy(gameObject);
                }
            }
        }
    }

    // --- SISTEM SENSOR RADIUS ---
    private void CekRadiusPlayer()
    {
        bool dalamRadiusInteraksi = false;
        bool dalamRadiusDeteksi = false;
        
        // Membaca semua objek yang masuk ke dalam radius terbesar
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
        if (dalamRadiusDeteksi)
        {
            if (!sedangMembaca && ikonPanah != null) ikonPanah.SetActive(true);
        }
        else
        {
            if (ikonPanah != null) ikonPanah.SetActive(false);
        }

        // Jika player menjauh melebihi jarak interaksi saat sedang membaca, tutup kertas secara otomatis
        if (!dalamRadiusInteraksi && sedangMembaca)
        {
            sedangMembaca = false;
            if (panelKertas != null) panelKertas.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // --- SISTEM GIZMOS ---
    // Menggambar lingkaran di layar Scene Unity (Hanya terlihat saat objek diklik)
    private void OnDrawGizmosSelected()
    {
        // Jarak interaksi (kuning)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusInteraksi);

        // Jarak deteksi ikon (hijau)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusDeteksi);
    }
}