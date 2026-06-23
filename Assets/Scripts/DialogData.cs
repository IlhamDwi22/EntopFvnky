using UnityEngine;

// Struktur kustom untuk menggabungkan Nama dan Kalimat dalam satu paket
[System.Serializable]
public struct Percakapan
{
    [Tooltip("Nama karakter yang sedang berbicara")]
    public string namaPembicara;
    
    [Tooltip("Isi kalimat yang diucapkan")]
    [TextArea(3, 5)]
    public string kalimat;
}

[CreateAssetMenu(fileName = "Data_Dialog_Baru", menuName = "Dialog/Data Dialog")]
public class DialogData : ScriptableObject
{
    [Header("Daftar Percakapan")]
    [Tooltip("Tambahkan jumlah percakapan, lalu isi nama dan kalimatnya masing-masing")]
    public Percakapan[] daftarPercakapan;
}