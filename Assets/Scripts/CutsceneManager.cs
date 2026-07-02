using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Wajib ditambahkan untuk menggunakan sistem jeda waktu

public class CutsceneManager : MonoBehaviour
{
    [Header("Pengaturan Cerita")]
    [Tooltip("Masukkan data dialog/narasi yang ingin ditampilkan")]
    public DialogData dialogNarasi;
    
    [Header("Tujuan Selanjutnya")]
    [Tooltip("Nama Scene yang akan dimuat setelah narasi selesai (Contoh: Scene_Level1)")]
    public string namaSceneBerikutnya;
    
    [Tooltip("OPSIONAL: Jika ingin mendarat di pintu tertentu setelah narasi (Kosongkan jika tidak perlu)")]
    public string idPintuTujuan;

    // Mengubah "void Start" menjadi "IEnumerator Start" agar bisa menggunakan delay
    private IEnumerator Start()
    {
        // Jeda selama 0.1 detik agar seluruh sistem UI dan DialogManager selesai di-load oleh Unity
        yield return new WaitForSeconds(0.1f);

        // Setelah jeda, baru jalankan dialognya
        if (DialogManager.Instance != null && dialogNarasi != null)
        {
            DialogManager.Instance.MulaiDialog(dialogNarasi, SelesaiCerita);
        }
        else
        {
            Debug.LogWarning("[Cutscene] Peringatan: DialogManager tidak ditemukan di scene ini!");
            SelesaiCerita(); 
        }
    }

    // Fungsi ini akan dipanggil otomatis oleh DialogManager ketika pemain mengklik teks terakhir
    private void SelesaiCerita()
    {
        if (!string.IsNullOrEmpty(idPintuTujuan))
        {
            DataPindahScene.idPintuTujuan = idPintuTujuan;
        }

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.PindahSceneDenganFade(namaSceneBerikutnya);
        }
        else
        {
            SceneManager.LoadScene(namaSceneBerikutnya);
        }
    }
}