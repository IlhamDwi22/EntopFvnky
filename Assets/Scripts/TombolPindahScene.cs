using UnityEngine;
using UnityEngine.SceneManagement;

public class TombolPindahScene : MonoBehaviour
{
    [Tooltip("Ketik nama scene tujuan dengan persis (misal: SceneOverworld)")]
    public string sceneTujuan;

    // Fungsi ini bisa dipanggil dari UI Button OnClick()
    public void PindahDenganFader()
    {
        if (SceneFader.Instance != null)
        {
            // Gunakan fader jika tersedia
            SceneFader.Instance.PindahSceneDenganFade(sceneTujuan);
        }
        else
        {
            // Cadangan jika lupa menaruh prefab fader
            Debug.LogWarning("Prefab Fader tidak ditemukan! Pindah secara instan.");
            SceneManager.LoadScene(sceneTujuan);
        }
    }
}