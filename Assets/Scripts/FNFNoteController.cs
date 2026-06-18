using UnityEngine;

public class FNFNoteController : MonoBehaviour
{
    private FNFNoteData noteData;
    private Transform targetReceptor;
    private float moveSpeed;
    private bool isReady = false;
    private bool hasMissed = false;

public void Setup(FNFNoteData data, Transform receptor, float speed)
{
    noteData = data;
    targetReceptor = receptor;
    moveSpeed = speed;
    isReady = true;

    // DAFTARKAN note player ke dalam sistem hitung scoring begitu dia lahir
    if (!noteData.isBot && FNFConductor.Instance.scoringSystem != null)
    {
        FNFConductor.Instance.scoringSystem.RegisterNoteInGame(this);
    }
}

    private void Update()
    {
        if (!isReady) return;

        // Hitung sisa waktu menuju target receptor (Detik lagu dikunci dari AudioSource)
        float timeDifference = noteData.hitTime - FNFConductor.Instance.currentSongTime;

        // RUMUS SINKRONISASI AUDIO: Posisi Y dipaksa mengikuti sisa waktu detik lagu secara real-time
        float targetY = targetReceptor.position.y + (timeDifference * moveSpeed);
        transform.position = new Vector3(targetReceptor.position.x, targetY, transform.position.z);

        // AI Bot Auto Hit ketika note menyentuh target receptor (waktu <= 0)
        if (noteData.isBot && timeDifference <= 0)
        {
            if (FNFConductor.Instance.scoringSystem != null) FNFConductor.Instance.scoringSystem.RegisterBotHit();
            Destroy(gameObject);
        }

// Player dinyatakan Miss jika note dibiarkan lolos ke bawah receptor
if (!noteData.isBot && timeDifference < -0.2f && !hasMissed)
{
    hasMissed = true;
    if (FNFConductor.Instance.scoringSystem != null)
    {
        // HAPUS note dari daftar antrean hit karena sudah kadaluwarsa
        FNFConductor.Instance.scoringSystem.RegisterNoteLeaveGame(this);
        FNFConductor.Instance.scoringSystem.RegisterPlayerMiss();
    }
    Destroy(gameObject, 0.05f);
}
    }

    public FNFNoteData GetDetails() => noteData;
}