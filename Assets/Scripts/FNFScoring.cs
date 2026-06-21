using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class FNFScoring : MonoBehaviour
{
    [Header("UI Canvas - Player")]
    public Text playerScoreLegacy;
    public TextMeshProUGUI playerScoreTMP;
    
    [Header("UI Canvas - Player Feedback")]
    public Text feedbackLegacy;
    public TextMeshProUGUI feedbackTMP;

    // Nilai toleransi Player
    private float hitWindow = 0.15f; 
    private float sickWindow = 0.05f;
    private float goodWindow = 0.1f;
    private int missPenalty = 50;

    private int playerScore = 0;
    private int playerCombo = 0;
    
    private List<FNFNoteController> activePlayerNotes = new List<FNFNoteController>();

    // --- SISTEM ANIMASI BARU YANG ANTI-NYANGKUT ---
    private Vector3[] baseReceptorScales = new Vector3[4];
    private float[] receptorAnimTimers = new float[4];
    private float animDuration = 0.15f;
    private Color[] currentTargetColors = new Color[4];
    private Vector3[] currentTargetScales = new Vector3[4];

    private void Start()
    {
        UpdatePlayerUI();
        ClearFeedback();
        
        // Mengunci ukuran asli panah (contoh: 0.2) tepat saat game dimulai.
        // Tidak akan bisa tertimpa oleh animasi apapun.
        for (int i = 0; i < 4; i++)
        {
            if (FNFConductor.Instance != null && FNFConductor.Instance.playerReceptors[i] != null)
            {
                baseReceptorScales[i] = FNFConductor.Instance.playerReceptors[i].localScale;
            }
            else 
            {
                baseReceptorScales[i] = Vector3.one; 
            }
        }
    }

    public void TerapkanDifficulty(int tingkatKesulitan)
    {
        float factor = tingkatKesulitan / 100f; 

        hitWindow = Mathf.Lerp(0.25f, 0.10f, factor); 
        sickWindow = Mathf.Lerp(0.08f, 0.03f, factor);
        goodWindow = Mathf.Lerp(0.15f, 0.07f, factor);
        missPenalty = (int)Mathf.Lerp(10, 150, factor);
    }

    private void Update()
    {
        // 1. Deteksi Input Pemain
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) TryHitNote(0);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) TryHitNote(1);
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) TryHitNote(2);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) TryHitNote(3);

        // 2. Jalankan pemulihan animasi setiap frame
        UpdateReceptorAnimations();
    }

    private void TryHitNote(int inputLane)
    {
        FNFNoteController closestNote = null;
        float smallestTimeDifference = float.MaxValue;

        foreach (FNFNoteController note in activePlayerNotes)
        {
            if (note == null) continue;
            
            FNFNoteData data = note.GetDetails();
            
            if (!data.isBot && data.lane == inputLane)
            {
                float timeDifference = Mathf.Abs(data.hitTime - FNFConductor.Instance.currentSongTime);
                if (timeDifference < smallestTimeDifference)
                {
                    smallestTimeDifference = timeDifference;
                    closestNote = note;
                }
            }
        }

        if (closestNote != null && smallestTimeDifference <= hitWindow)
        {
            playerCombo++;
            
            if (smallestTimeDifference <= sickWindow)
            {
                playerScore += 350;
                ShowFeedback("SICK!!");
                TriggerReceptorEffect(inputLane, "SICK");
            }
            else if (smallestTimeDifference <= goodWindow)
            {
                playerScore += 200;
                ShowFeedback("GOOD!");
                TriggerReceptorEffect(inputLane, "GOOD");
            }
            else
            {
                playerScore += 50;
                ShowFeedback("BAD");
                TriggerReceptorEffect(inputLane, "BAD");
            }

            UpdatePlayerUI();
            
            activePlayerNotes.Remove(closestNote);
            Destroy(closestNote.gameObject);
        }
        else
        {
            // Miss karena memencet saat kosong (Ghost Tapping)
            playerCombo = 0;
            UpdatePlayerUI();
            ShowFeedback("MISS!");
            TriggerReceptorEffect(inputLane, "MISS");
        }
    }

    public void RegisterNoteInGame(FNFNoteController note)
    {
        if (!note.GetDetails().isBot) activePlayerNotes.Add(note);
    }

    public void RegisterNoteLeaveGame(FNFNoteController note)
    {
        if (activePlayerNotes.Contains(note)) activePlayerNotes.Remove(note);
    }

    public void RegisterPlayerMiss()
    {
        playerCombo = 0;
        if (playerScore > 0) 
        {
            playerScore -= missPenalty; 
            if (playerScore < 0) playerScore = 0; 
        }
        
        UpdatePlayerUI();
        ShowFeedback("MISS!");
    }

    private void TriggerReceptorEffect(int lane, string rating)
    {
        if (FNFConductor.Instance == null || FNFConductor.Instance.playerReceptors == null) return;
        Transform receptor = FNFConductor.Instance.playerReceptors[lane];
        if (receptor == null) return;

        SpriteRenderer sr = receptor.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // Tentukan target warna dan puncakan ukuran yang baru dikalikan dengan ukuran asli
        if (rating == "SICK" || rating == "GOOD")
        {
            currentTargetColors[lane] = new Color(0.5f, 1f, 1f, 1f); 
            currentTargetScales[lane] = baseReceptorScales[lane] * 1.3f;          
        }
        else if (rating == "BAD")
        {
            currentTargetColors[lane] = new Color(1f, 0.8f, 0.2f, 1f); 
            currentTargetScales[lane] = baseReceptorScales[lane] * 1.15f;           
        }
        else if (rating == "MISS")
        {
            currentTargetColors[lane] = new Color(1f, 0.3f, 0.3f, 1f); 
            currentTargetScales[lane] = baseReceptorScales[lane] * 0.8f;            
        }

        // Terapkan lompatan animasi seketika di frame ini
        sr.color = currentTargetColors[lane];
        receptor.localScale = currentTargetScales[lane];

        // Isi penuh bensin waktu animasinya
        receptorAnimTimers[lane] = animDuration;
    }

    private void UpdateReceptorAnimations()
    {
        if (FNFConductor.Instance == null || FNFConductor.Instance.playerReceptors == null) return;

        // Tiap frame, paksa panah mengecil kembali ke titik asalnya secara perlahan
        for (int i = 0; i < 4; i++)
        {
            if (receptorAnimTimers[i] > 0)
            {
                receptorAnimTimers[i] -= Time.deltaTime;
                
                Transform receptor = FNFConductor.Instance.playerReceptors[i];
                if (receptor == null) continue;
                
                SpriteRenderer sr = receptor.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                if (receptorAnimTimers[i] <= 0)
                {
                    // Waktu habis, kunci absolut ke kondisi normal
                    sr.color = Color.white;
                    receptor.localScale = baseReceptorScales[i];
                }
                else
                {
                    // Menghitung persentase dari 0 hingga 1
                    float t = 1f - (receptorAnimTimers[i] / animDuration); 
                    
                    // Transisi mulus menyusut kembali ke skala aslinya
                    sr.color = Color.Lerp(currentTargetColors[i], Color.white, t);
                    receptor.localScale = Vector3.Lerp(currentTargetScales[i], baseReceptorScales[i], t);
                }
            }
        }
    }

    private void UpdatePlayerUI()
    {
        string txt = $"Player: {playerScore} | Combo: {playerCombo}";
        if (playerScoreLegacy != null) playerScoreLegacy.text = txt;
        if (playerScoreTMP != null) playerScoreTMP.text = txt;
    }

    private void ShowFeedback(string message)
    {
        if (feedbackLegacy != null) feedbackLegacy.text = message;
        if (feedbackTMP != null) feedbackTMP.text = message;
        
        StopAllCoroutines();
        StartCoroutine(ClearFeedbackRoutine());
    }

    private void ClearFeedback()
    {
        if (feedbackLegacy != null) feedbackLegacy.text = "";
        if (feedbackTMP != null) feedbackTMP.text = "";
    }

    private IEnumerator ClearFeedbackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        ClearFeedback();
    }
}