using UnityEngine;
using UnityEngine.UI; // Wajib untuk mengakses Text UI Legacy

public class TextBlink : MonoBehaviour
{
    [Header("Blink Settings")]
    public float blinkSpeed = 2f; // Kecepatan kedip (semakin besar semakin cepat)
    
    private Text textComponent; // Menggunakan Text UI Legacy

    void Start()
    {
        // Mengambil komponen Text Legacy yang menempel di objek ini
        textComponent = GetComponent<Text>();
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
