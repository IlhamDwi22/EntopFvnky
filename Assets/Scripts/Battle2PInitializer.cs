using UnityEngine;

public class Battle2PInitializer : MonoBehaviour
{
    [Header("List GameObject Map")]
    [Tooltip("Urutkan objek map sesuai dengan urutan daftarMap di Menu2PManager (0 = Map 1, 1 = Map 2, dst)")]
    public GameObject[] mapGameObjects;

    private void Start()
    {
        if (mapGameObjects == null || mapGameObjects.Length == 0)
        {
            Debug.LogWarning("[Battle2PInitializer] Daftar GameObject Map masih kosong!");
            return;
        }

        int mapIndex = Global2PState.pilihanMapIndex;

        // Jika bernilai -1 (Acak/Random), pilih acak berdasarkan jumlah map fisik yang terpasang di scene
        if (mapIndex == -1)
        {
            mapIndex = Random.Range(0, mapGameObjects.Length);
            Debug.Log($"[Battle2PInitializer] Memilih map acak secara dinamis: Index {mapIndex} ({mapGameObjects[mapIndex].name})");
        }

        // Amankan index agar tidak out of bounds
        if (mapIndex < 0 || mapIndex >= mapGameObjects.Length)
        {
            mapIndex = 0;
        }

        // Aktifkan map terpilih dan matikan yang lain
        for (int i = 0; i < mapGameObjects.Length; i++)
        {
            if (mapGameObjects[i] != null)
            {
                mapGameObjects[i].SetActive(i == mapIndex);
            }
        }
    }
}
