using UnityEngine;
using TMPro;

public class UIInputPassword : MonoBehaviour
{
    public static UIInputPassword Instance;

    [Header("Komponen Layar UI")]
    public GameObject panelInput;
    public TMP_InputField kotakKetik;

    private PetiPassword petiYangSedangDibuka;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (panelInput != null) panelInput.SetActive(false);
    }

    public void BukaLayarPassword(PetiPassword petiTarget)
    {
        petiYangSedangDibuka = petiTarget;
        kotakKetik.text = ""; // Kosongkan ketikan sebelumnya
        panelInput.SetActive(true);
        Time.timeScale = 0f; // Hentikan waktu game agar player tidak jalan-jalan
    }

    // Sambungkan fungsi ini ke tombol "OK / Submit" di Unity
    public void TombolKonfirmasi()
    {
        TutupLayar();
        if (petiYangSedangDibuka != null)
        {
            petiYangSedangDibuka.VerifikasiPassword(kotakKetik.text);
        }
    }

    // Sambungkan fungsi ini ke tombol "Batal / Close" di Unity
    public void TutupLayar()
    {
        panelInput.SetActive(false);
        Time.timeScale = 1f; // Jalankan waktu game kembali
    }
}