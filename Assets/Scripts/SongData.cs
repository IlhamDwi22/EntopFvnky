using UnityEngine;

[CreateAssetMenu(fileName = "LaguBaru", menuName = "FNF/Data Lagu")]
public class SongData : ScriptableObject
{
    [Header("Identitas Lagu")]
    public string judulLagu;
    public string namaArtis;

    [Header("File Mentah")]
    public AudioClip fileAudio;
    public TextAsset fileChart;

    [Header("Pengaturan Khusus Lagu")]
    public float delayMulai = 1f;
    public float kecepatanScroll = 5f;
    public float penyesuaianOffset = 0f;
}