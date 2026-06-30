using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class KertasPetunjuk : MonoBehaviour
{
    [Header("Deteksi Player (Gizmos)")]
    [Tooltip("Atur seberapa jauh player bisa berinteraksi dengan benda ini")]
    public float radiusInteraksi = 1.5f;

    [Header("UI Kertas")]
    public GameObject panelKertas;
    public TextMeshProUGUI teksCatatan;

    [Header("Visual Indikator")]
    public GameObject ikonPanah;

    private bool playerDiDekat = false;
    private bool sedangMembaca = false;
    private bool sudahDiambil = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (panelKertas != null) panelKertas.SetActive(false);
        if (ikonPanah != null) ikonPanah.SetActive(false); 
    }

    private void Update()
    {
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
                    teksCatatan.text = 
                    "Sebuah catatan bernoda darah...\n\n" +
                    "\"Kombinasi brankas hari ini adalah: <color=red>" +
                    PetiPassword.passwordRahasiaSaatIni +
                    "</color>\"";
                }
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
                if (!sudahDiambil)
                {
                    sudahDiambil = true;
                    string idItem = gameObject.name;
                    GlobalBattleState.TambahItemRuntime(idItem, spriteRenderer.sprite);
                    GlobalBattleState.TambahBarang(idItem);
                    Destroy(gameObject);
                }
            }
        }
    }

    // --- SISTEM SENSOR RADIUS ---
    private void CekRadiusPlayer()
    {
        bool terdeteksi = false;
        
        // Membaca semua objek yang masuk ke dalam lingkaran radius
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radiusInteraksi);
        foreach (Collider2D c in cols)
        {
            if (c.CompareTag("Player")) 
            { 
                terdeteksi = true; 
                break; 
            }
        }

        // Jika player baru saja masuk ke radius lingkaran
        if (terdeteksi && !playerDiDekat)
        {
            playerDiDekat = true;
            if (!sedangMembaca && ikonPanah != null) ikonPanah.SetActive(true);
        }
        // Jika player baru saja keluar dari radius lingkaran
        else if (!terdeteksi && playerDiDekat)
        {
            playerDiDekat = false;
            sedangMembaca = false;
            if (panelKertas != null) panelKertas.SetActive(false);
            if (ikonPanah != null) ikonPanah.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // --- SISTEM GIZMOS ---
    // Menggambar lingkaran kuning di layar Scene Unity (Hanya terlihat saat objek diklik)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusInteraksi);
    }
}