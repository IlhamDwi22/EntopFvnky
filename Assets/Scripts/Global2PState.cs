using UnityEngine;

public static class Global2PState
{
    // Karakter Pilihan P1 dan P2
    public static OpponentData p1Karakter;
    public static OpponentData p2Karakter;

    // Map Pilihan (0 = Map 1, 1 = Map 2, dll, -1 = Random)
    public static int pilihanMapIndex = 0;

    // Lagu Pilihan
    public static Song2PData laguTerpilih;
}

[System.Serializable]
public class Song2PData
{
    public string judulLagu;
    public bool isCustom;

    // Untuk Lagu Built-in
    public SongData songDataBawaan;

    // Untuk Lagu Custom
    public string folderPath;
    public string audioPath;
    public string chartPath;

    // Pengaturan Lagu yang Bisa Disesuaikan
    public float scrollSpeed = 5f;
    public float audioOffset = 0f;
    public float delayMulai = 1f;
}

[System.Serializable]
public class SongConfigData
{
    public float scrollSpeed = 5f;
    public float audioOffset = 0f;
    public float delayMulai = 1f;
}
