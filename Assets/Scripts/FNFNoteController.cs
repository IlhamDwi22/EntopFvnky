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
    
    [Tooltip("Karena Prefabmu ukurannya 0.2, naikkan angka ini (misal 5 atau 10) agar buntutnya panjang dan terlihat!")]
    public float panjangBuntutMultiplier = 5f;
    
    [Tooltip("Ubah angka ini untuk mengatur seberapa tebal/lebar ukuran buntutnya")]
    public float tebalBuntut = 1.5f;
    
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

        if (noteData.duration >= 0.1f && holdTail != null)
        {
            holdTail.gameObject.SetActive(true);
            
            // LOGIKA BARU: Panjang asli dikali dengan Multiplier agar terlihat jelas!
            float tailLength = noteData.duration * moveSpeed * panjangBuntutMultiplier;
            
            // Atur ketebalan dan panjang
            holdTail.localScale = new Vector3(tebalBuntut, tailLength, 1);
            holdTail.localPosition = new Vector3(0, tailLength / 2f, 0); 
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
                DestroyNoteAndRemove();
            }
            else if (holdTail != null)
            {
                // LOGIKA BARU: Saat menyusut juga menggunakan Multiplier
                float tailLength = timeRemaining * moveSpeed * panjangBuntutMultiplier;
                holdTail.localScale = new Vector3(tebalBuntut, tailLength, 1);
                holdTail.localPosition = new Vector3(0, tailLength / 2f, 0);
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
                if (botHit && noteData.duration >= 0.1f)
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