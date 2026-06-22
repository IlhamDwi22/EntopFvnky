using UnityEngine;
using TMPro;

public class TextBlink : MonoBehaviour
{
    [Header("Blink Settings")]
    public float blinkSpeed = 2f; // Kecepatan kedip (semakin besar semakin cepat)
    
    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (textComponent != null)
        {
            Color c = textComponent.color;
            // Menggunakan Mathf.PingPong untuk membuat nilai Alpha (transparansi) naik-turun dengan halus
            c.a = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            textComponent.color = c;
        }
    }
}