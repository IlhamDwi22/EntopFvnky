using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BarangQuest : MonoBehaviour
{
    [Header("Pengaturan Barang")]
    [Tooltip("ID unik barang ini. Harus sama persis dengan yang diinput pada WilayahDialog milik musuh")]
    public string IDBarang = "KunciLevel2";

    [Header("Dialog Saat Diambil (Opsional)")]
    [Tooltip("Masukkan file dialog yang muncul tepat saat player menyentuh benda ini")]
    public DialogData dialogSaatDiambil;

    private void Start()
    {
        // Otomatis memaksa collider menjadi trigger agar bisa ditembus player
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Daftarkan ID barang ini ke dalam kantong memori global
            GlobalBattleState.TambahBarang(IDBarang);
            Debug.Log($"[Quest System] Berhasil mengambil barang: {IDBarang}");

            if (dialogSaatDiambil != null && DialogManager.Instance != null)
            {
                // Sembunyikan fisik benda dan matikan kolidernya terlebih dahulu agar aman selama dialog berjalan
                GetComponent<BoxCollider2D>().enabled = false;
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;

                DialogManager.Instance.MulaiDialog(dialogSaatDiambil, () =>
                {
                    Destroy(gameObject); // Hancurkan total setelah dialog selesai dibaca
                });
            }
            else
            {
                Destroy(gameObject); // Jika tidak pakai dialog, langsung hancurkan seketika
            }
        }
    }
}