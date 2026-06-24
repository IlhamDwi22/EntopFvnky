using UnityEngine;

public class FNFNoteController : MonoBehaviour
{
    private FNFNoteData noteData;
    private Transform targetReceptor;
    private float moveSpeed;
    private bool isReady = false;
    private bool hasMissed = false;

    [Header("Hold Note Settings")]
    public Transform holdTail; 
    
    public float panjangBuntutMultiplier = 1f;
    public float tebalBuntut = 1.5f;

    [Tooltip("Geser angka ini untuk menaikkan posisi awal buntut ke pucuk panah (misal: 0.5 atau 1)")]
    public float yOffsetBuntut = 0.5f; // <--- VARIABEL BARU UNTUK MENGGESER EKOR
    
    public bool isBeingHeld = false;
    private bool isBotHolding = false;
    private SpriteRenderer headRenderer;

    public void Setup(FNFNoteData data, Transform receptor, float speed)
    {
        noteData = data;
        targetReceptor = receptor;
        moveSpeed = speed;
        isReady = true;

        headRenderer = GetComponent<SpriteRenderer>();

        if (noteData.duration >= 0.05f && holdTail != null)
        {
            holdTail.gameObject.SetActive(true);
            
            float tailLength = noteData.duration * moveSpeed * panjangBuntutMultiplier;
            
            holdTail.localScale = new Vector3(tebalBuntut, tailLength, 1);
            
            // LOGIKA BARU: Posisi Y ditambah dengan yOffsetBuntut agar naik ke pucuk panah
            holdTail.localPosition = new Vector3(0, (tailLength / 2f) + yOffsetBuntut, 0); 
        }
        else if (holdTail != null)
        {
            holdTail.gameObject.SetActive(false); 
        }

        if (!noteData.isBot && FNFConductor.Instance.scoringSystem != null)
        {
            FNFConductor.Instance.scoringSystem.RegisterNoteInGame(this);
        }
    }

    private void Update()
    {
        if (!isReady) return;

        float timeDifference = noteData.hitTime - FNFConductor.Instance.currentSongTime;

        if (isBeingHeld || isBotHolding)
        {
            transform.position = new Vector3(targetReceptor.position.x, targetReceptor.position.y, transform.position.z);
            
            if (headRenderer != null) headRenderer.enabled = false;

            float timeRemaining = (noteData.hitTime + noteData.duration) - FNFConductor.Instance.currentSongTime;

            if (timeRemaining <= 0)
            {
                if (noteData.isBot) DestroyNoteAndRemove();
            }
            else if (holdTail != null)
            {
                float tailLength = timeRemaining * moveSpeed * panjangBuntutMultiplier;
                holdTail.localScale = new Vector3(tebalBuntut, tailLength, 1);
                
                // LOGIKA BARU: Pastikan ekor tetap berada di pucuk panah saat sedang menyusut
                holdTail.localPosition = new Vector3(0, (tailLength / 2f) + yOffsetBuntut, 0);
            }
            return; 
        }

        float targetY = targetReceptor.position.y + (timeDifference * moveSpeed);
        transform.position = new Vector3(targetReceptor.position.x, targetY, transform.position.z);

        if (noteData.isBot && timeDifference <= 0)
        {
            if (FNFConductor.Instance.botAI != null) 
            {
                bool botHit = FNFConductor.Instance.botAI.EvaluateBotNote(this);
                if (botHit && noteData.duration >= 0.05f)
                {
                    isBotHolding = true; 
                }
                else
                {
                    DestroyNoteAndRemove(); 
                }
            }
        }

        if (!noteData.isBot && timeDifference < -0.2f && !hasMissed && !isBeingHeld)
        {
            hasMissed = true;
            if (FNFConductor.Instance.scoringSystem != null)
            {
                FNFConductor.Instance.scoringSystem.RegisterPlayerMiss();
            }
            DestroyNoteAndRemove();
        }
    }

    public void DestroyNoteAndRemove()
    {
        if (!noteData.isBot && FNFConductor.Instance != null && FNFConductor.Instance.scoringSystem != null)
        {
            FNFConductor.Instance.scoringSystem.RegisterNoteLeaveGame(this);
        }
        Destroy(gameObject);
    }

    public FNFNoteData GetDetails() => noteData;
}