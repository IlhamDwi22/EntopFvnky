using UnityEngine;

public class FNF2PNoteController : MonoBehaviour
{
    private FNF2PNoteData noteData;
    private Transform targetReceptor;
    private float moveSpeed;
    private bool isReady = false;
    private bool hasMissed = false;

    [Header("Hold Note Settings")]
    public Transform holdTail; 
    
    public float panjangBuntutMultiplier = 1f;
    public float tebalBuntut = 1.5f;

    [Tooltip("Geser angka ini untuk menaikkan posisi awal buntut ke pucuk panah (misal: 0.5 atau 1)")]
    public float yOffsetBuntut = 0.5f; 
    
    public bool isBeingHeld = false;
    private SpriteRenderer headRenderer;

    public void Setup(FNF2PNoteData data, Transform receptor, float speed)
    {
        noteData = data;
        targetReceptor = receptor;
        moveSpeed = speed;
        isReady = true;

        headRenderer = GetComponent<SpriteRenderer>();

        // Set gambar panah sesuai arah lane (0 = Kiri, 1 = Bawah, 2 = Atas, 3 = Kanan)
        if (FNF2PConductor.Instance != null)
        {
            if (data.lane >= 0 && data.lane < FNF2PConductor.Instance.noteSprites.Length)
            {
                if (headRenderer != null) headRenderer.sprite = FNF2PConductor.Instance.noteSprites[data.lane];
            }
        }

        // Set gambar ekor (Hold Note)
        if (noteData.duration >= 0.05f && holdTail != null)
        {
            holdTail.gameObject.SetActive(true);
            
            SpriteRenderer tailRenderer = holdTail.GetComponent<SpriteRenderer>();
            if (tailRenderer != null && FNF2PConductor.Instance != null)
            {
                if (data.lane >= 0 && data.lane < FNF2PConductor.Instance.holdSprites.Length)
                {
                    tailRenderer.sprite = FNF2PConductor.Instance.holdSprites[data.lane];
                }
            }

            float tailLength = noteData.duration * moveSpeed * panjangBuntutMultiplier;
            holdTail.localScale = new Vector3(tebalBuntut, tailLength, 1);
            holdTail.localPosition = new Vector3(0, (tailLength / 2f) + yOffsetBuntut, 0); 
        }
        else if (holdTail != null)
        {
            holdTail.gameObject.SetActive(false); 
        }

        if (FNF2PConductor.Instance != null && FNF2PConductor.Instance.scoringSystem != null)
        {
            FNF2PConductor.Instance.scoringSystem.RegisterNoteInGame(this);
        }
    }

    private void Update()
    {
        if (!isReady || FNF2PConductor.Instance == null || targetReceptor == null) return;
        if (Time.timeScale == 0f) return;

        float timeDifference = noteData.hitTime - FNF2PConductor.Instance.currentSongTime;

        if (isBeingHeld)
        {
            transform.position = new Vector3(targetReceptor.position.x, targetReceptor.position.y, transform.position.z);
            
            if (headRenderer != null) headRenderer.enabled = false;

            float timeRemaining = (noteData.hitTime + noteData.duration) - FNF2PConductor.Instance.currentSongTime;

            if (timeRemaining <= 0)
            {
                DestroyNoteAndRemove();
            }
            else if (holdTail != null)
            {
                float tailLength = timeRemaining * moveSpeed * panjangBuntutMultiplier;
                holdTail.localScale = new Vector3(tebalBuntut, tailLength, 1);
                holdTail.localPosition = new Vector3(0, (tailLength / 2f) + yOffsetBuntut, 0);
            }
            return; 
        }

        float targetY = targetReceptor.position.y + (timeDifference * moveSpeed);
        transform.position = new Vector3(targetReceptor.position.x, targetY, transform.position.z);

        // Deteksi Miss (Tombol terlewat)
        if (timeDifference < -0.2f && !hasMissed && !isBeingHeld)
        {
            hasMissed = true;
            if (FNF2PConductor.Instance.scoringSystem != null)
            {
                FNF2PConductor.Instance.scoringSystem.TriggerNoteMiss(this);
            }
            DestroyNoteAndRemove();
        }
    }

    public void DestroyNoteAndRemove()
    {
        if (FNF2PConductor.Instance != null && FNF2PConductor.Instance.scoringSystem != null)
        {
            FNF2PConductor.Instance.scoringSystem.RemoveNoteFromActiveList(this);
        }
        Destroy(gameObject);
    }

    public FNF2PNoteData GetDetails() => noteData;
}
