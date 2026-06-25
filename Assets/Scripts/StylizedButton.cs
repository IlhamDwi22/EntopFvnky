using UnityEngine;
using UnityEngine.UI; // Wajib untuk mengakses Text UI Legacy
using UnityEngine.EventSystems;

public class StylizedButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
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
        
        if (buttonText != null) buttonText.color = normalColor;
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

    // Dipanggil saat dipilih via Keyboard/Gamepad (Tanda Panah/WASD)
    public void OnSelect(BaseEventData eventData)
    {
        HighlightButton();
    }

    // Dipanggil saat pindah ke tombol lain via Keyboard/Gamepad
    public void OnDeselect(BaseEventData eventData)
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
        transform.localScale = originalScale;
        if (buttonText != null) buttonText.color = normalColor;
    }
}
