using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PengaturanTombolUI : MonoBehaviour
{
    [Header("Teks Tampilan Tombol (TMP)")]
    public TextMeshProUGUI txtLeft;
    public TextMeshProUGUI txtDown;
    public TextMeshProUGUI txtUp;
    public TextMeshProUGUI txtRight;

    [Header("Teks Instruksi / Pesan")]
    public TextMeshProUGUI txtStatus; // Contoh: "Tekan tombol baru..."

    private string keyToRebind = null;
    private bool isWaitingForInput = false;

    private void Start()
    {
        if (KeyMappingManager.Instance == null)
        {
            // Buat otomatis jika belum ada di scene
            GameObject managerObj = new GameObject("KeyMappingManager");
            managerObj.AddComponent<KeyMappingManager>();
        }
        UpdateUI();
        if (txtStatus != null) txtStatus.text = "Pilih tombol untuk diubah.";
    }

    // Fungsi ini dipanggil saat pemain mengklik tombol Kiri, Bawah, dll di UI
    public void StartRebind(string actionName)
    {
        keyToRebind = actionName;
        isWaitingForInput = true;
        if (txtStatus != null) txtStatus.text = "Mendengarkan... Tekan tombol apapun (Keyboard/Gamepad)!";
    }

    private void Update()
    {
        if (isWaitingForInput)
        {
            // Deteksi SELURUH tombol yang ada di dunia ini (Keyboard + Joystick)
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(vKey))
                {
                    // Cegah tombol Esc atau Mouse Click terdaftar secara tidak sengaja
                    if (vKey != KeyCode.Escape && vKey != KeyCode.Mouse0 && vKey != KeyCode.Mouse1)
                    {
                        KeyMappingManager.Instance.SaveKey(keyToRebind, vKey);
                        isWaitingForInput = false;
                        UpdateUI();
                        if (txtStatus != null) txtStatus.text = "Tombol berhasil diubah!";
                        break;
                    }
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (KeyMappingManager.Instance == null) return;
        
        // Menampilkan nama tombol ke layar UI (misal: "A" atau "JoystickButton2")
        if (txtLeft != null) txtLeft.text = KeyMappingManager.Instance.keyLeft.ToString();
        if (txtDown != null) txtDown.text = KeyMappingManager.Instance.keyDown.ToString();
        if (txtUp != null) txtUp.text = KeyMappingManager.Instance.keyUp.ToString();
        if (txtRight != null) txtRight.text = KeyMappingManager.Instance.keyRight.ToString();
    }
}