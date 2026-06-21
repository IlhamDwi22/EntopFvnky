using UnityEngine;

public class CombatTester : MonoBehaviour
{
    [Header("Data Tes Pertarungan (Khusus Tes di Scene Ini)")]
    [Tooltip("Masukkan Data Musuh ke sini agar bisa langsung Play tanpa lewat kota")]
    public OpponentData musuhTes;

    private void Awake()
    {
        // Mengecek apakah memori GameManager kosong (yang berarti kamu Play langsung dari Scene Combat)
        if (GameManager.musuhPilihanSaatIni == null)
        {
            if (musuhTes != null)
            {
                Debug.LogWarning("[CombatTester] Mode Tes Aktif: Menggunakan data musuh dari CombatTester.");
                GameManager.musuhPilihanSaatIni = musuhTes;
                GameManager.urutanLaguSaatIni = 0;
            }
            else
            {
                Debug.LogError("[CombatTester] GAGAL! Kamu belum memasukkan Data Musuh ke dalam script CombatTester!");
            }
        }
    }
}