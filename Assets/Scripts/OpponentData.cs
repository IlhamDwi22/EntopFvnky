using UnityEngine;

[CreateAssetMenu(fileName = "MusuhBaru", menuName = "FNF/Data Musuh")]
public class OpponentData : ScriptableObject
{
    [Header("Profil Musuh")]
    public string namaMusuh;
    public Sprite visualMusuh; 

    [Header("Statistik Pertarungan")]
    [Range(1, 100)]
    [Tooltip("Tingkat kesulitan 1 (Sangat Mudah) hingga 100 (Sangat Sulit)")]
    public int tingkatKesulitan = 50;

    [Header("Daftar Lagu (Setlist)")]
    public SongData[] daftarLagu;
}