using UnityEngine;
using UnityEngine.UI; // Wajib untuk mengakses Text UI Legacy
using UnityEngine.EventSystems;

public class StylizedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text buttonText; // Menggunakan Text UI Legacy
    private Vector3 originalScale;

    [Header("Styling")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0f, 1f, 1f); // Warna Cyan terang
    public float hoverScale = 1.2f; // Membesar 20%

    void Awake()
    {
        // Mengambil komponen teks Legacy dari anak objek (Text)
        buttonText = GetComponentInChildren<Text>();
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        // Pastikan tombol selalu kembali ke warna/skala normal saat aktif pertama kali
        ResetButton();
    }

    // Dipanggil saat disentuh Mouse
    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightButton();
    }

    // Dipanggil saat Mouse pergi
    public void OnPointerExit(PointerEventData eventData)
    {
        ResetButton();
    }

    private void HighlightButton()
    {
        transform.localScale = originalScale * hoverScale;
        if (buttonText != null) buttonText.color = hoverColor;
    }

    private void ResetButton()
    {
        // Mengamankan kondisi jika originalScale belum tersimpan (misal di awal program)
        if (originalScale == Vector3.zero) originalScale = transform.localScale;

        transform.localScale = originalScale;
        if (buttonText != null) buttonText.color = normalColor;
    }
}
