using UnityEngine;
using System.Collections;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Prefab/Sprite Mobil")]
    [Tooltip("Masukkan prefab-prefab mobil Anda ke sini (bisa lebih dari satu jenis)")]
    public GameObject[] prefabsMobil;

    [Header("Titik Spawn & Tujuan")]
    [Tooltip("Ujung jalan sebelah kiri (titik spawn / tujuan akhir)")]
    public Transform titikKiri;
    [Tooltip("Ujung jalan sebelah kanan (titik spawn / tujuan akhir)")]
    public Transform titikKanan;

    [Header("Pengaturan Jeda Muncul (Detik)")]
    public float jedaMin = 3f;
    public float jedaMax = 8f;

    [Header("Pengaturan Kecepatan")]
    public float kecepatanMin = 4f;
    public float kecepatanMax = 8f;

    [Header("Opsi Balik Gambar")]
    [Tooltip("Centang jika gambar mobil bawaan Anda di project menghadap ke KIRI (agar saat jalan ke kanan otomatis di-flip X)")]
    public bool mobilBawaanHadapKiri = true;

    private void Start()
    {
        if (prefabsMobil == null || prefabsMobil.Length == 0 || titikKiri == null || titikKanan == null)
        {
            Debug.LogWarning("[TrafficSpawner] Lengkapi konfigurasi variabel di Inspector terlebih dahulu!");
            return;
        }

        StartCoroutine(RoutineSpawnMobil());
    }

    private IEnumerator RoutineSpawnMobil()
    {
        while (true)
        {
            // Menunggu jeda waktu acak sebelum memunculkan mobil berikutnya
            float jedaAcak = Random.Range(jedaMin, jedaMax);
            yield return new WaitForSeconds(jedaAcak);

            SpawnMobilAcak();
        }
    }

    private void SpawnMobilAcak()
    {
        // 1. Pilih jenis mobil secara acak dari array
        int indeksMobil = Random.Range(0, prefabsMobil.Length);
        GameObject prefabPilihan = prefabsMobil[indeksMobil];

        if (prefabPilihan == null) return;

        // 2. Tentukan arah acak (0 = ke Kanan, 1 = ke Kiri)
        int arah = Random.Range(0, 2);

        Vector3 posisiSpawn;
        Vector3 posisiTujuan;
        bool harusFlip = false;

        if (arah == 0) // Jalur Ke Kanan
        {
            posisiSpawn = titikKiri.position;
            posisiTujuan = titikKanan.position;
            // Jika mobil default hadap kiri, jalan ke kanan harus di-flip X
            harusFlip = mobilBawaanHadapKiri; 
        }
        else // Jalur Ke Kiri
        {
            posisiSpawn = titikKanan.position;
            posisiTujuan = titikKiri.position;
            // Jika mobil default hadap kiri, jalan ke kiri TIDAK perlu di-flip X
            harusFlip = !mobilBawaanHadapKiri;
        }

        // 3. Spawn instansi mobil
        GameObject mobil = Instantiate(prefabPilihan, posisiSpawn, Quaternion.identity);

        // 4. Pasang skrip penggerak dinamis ke mobil yang baru disub-spawn
        MovingCar penggerak = mobil.AddComponent<MovingCar>();
        float kecepatanAcak = Random.Range(kecepatanMin, kecepatanMax);
        penggerak.Setup(posisiTujuan, kecepatanAcak, harusFlip);
    }
}
