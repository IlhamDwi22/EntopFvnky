[System.Serializable]
public class FNFNoteData
{
    public float hitTime;      // Kapan note harus di-hit (dalam detik)
    public int lane;           // Lajur panah (0: Kiri, 1: Bawah, 2: Atas, 3: Kanan)
    public bool isBot;         // Apakah note ini milik musuh/bot
    public float duration;     // Durasi untuk hold/sustain note
}