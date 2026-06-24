using UnityEngine;

[CreateAssetMenu(fileName = "DataMusuhBaru", menuName = "FNF/Data Musuh")]
public class OpponentData : ScriptableObject
{
    public string namaMusuh = "Nama Musuh";
    public Sprite visualMusuh;
    
    [Header("Animasi Karakter")]
    [Tooltip("Masukkan Animator Controller khusus musuh ini ke sini")]
    public RuntimeAnimatorController animasiMusuh;
    
    [Range(0, 100)] public int tingkatKesulitan = 50;
    public SongData[] daftarLagu;
}