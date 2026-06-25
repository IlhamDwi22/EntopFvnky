using UnityEngine;
using System;

public class KeyMappingManager : MonoBehaviour
{
    public static KeyMappingManager Instance;

    // Variabel tombol yang bisa diakses dari script mana saja
    public KeyCode keyLeft { get; private set; }
    public KeyCode keyDown { get; private set; }
    public KeyCode keyUp { get; private set; }
    public KeyCode keyRight { get; private set; }

    private void Awake()
    {
        // Pastikan hanya ada 1 Manager di seluruh game
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Jangan hancurkan saat pindah scene
            LoadKeys(); // Muat tombol yang tersimpan
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadKeys()
    {
        // Defaultnya menggunakan WASD, tapi jika sudah pernah diubah, ambil dari memori (PlayerPrefs)
        keyLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyLeft", "A"));
        keyDown = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyDown", "S"));
        keyUp = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyUp", "W"));
        keyRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyRight", "D"));
    }

    public void SaveKey(string action, KeyCode newKey)
    {
        PlayerPrefs.SetString(action, newKey.ToString());
        LoadKeys(); // Perbarui memori setelah disimpan
    }
}