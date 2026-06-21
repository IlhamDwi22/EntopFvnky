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

        if (!noteData.isBot && FNFConductor.Instance.scoringSystem != null)
        {
            FNFConductor.Instance.scoringSystem.RegisterNoteInGame(this);
        }
    }

    private void Update()
    {
        if (!isReady) return;

        float timeDifference = noteData.hitTime - FNFConductor.Instance.currentSongTime;
        float targetY = targetReceptor.position.y + (timeDifference * moveSpeed);
        transform.position = new Vector3(targetReceptor.position.x, targetY, transform.position.z);

        // PENYESUAIAN: Panah Musuh melapor ke script FNFBotAI
        if (noteData.isBot && timeDifference <= 0)
        {
            if (FNFConductor.Instance.botAI != null) 
            {
                FNFConductor.Instance.botAI.EvaluateBotNote(this);
            }
            Destroy(gameObject);
        }

        // Panah Player
        if (!noteData.isBot && timeDifference < -0.2f && !hasMissed)
        {
            hasMissed = true;
            if (FNFConductor.Instance.scoringSystem != null)
            {
                FNFConductor.Instance.scoringSystem.RegisterNoteLeaveGame(this);
                FNFConductor.Instance.scoringSystem.RegisterPlayerMiss();
            }
            Destroy(gameObject, 0.05f);
        }
    }

    public FNFNoteData GetDetails() => noteData;
}