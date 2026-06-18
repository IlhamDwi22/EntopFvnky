using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class FNFScoring : MonoBehaviour
{
    [Header("UI Canvas - Player")]
    [Tooltip("Masukkan Text Skor Player ke sini")]
    public Text playerScoreLegacy;
    public TextMeshProUGUI playerScoreTMP;
    
    [Header("UI Canvas - Bot / Musuh")]
    [Tooltip("Masukkan Text Skor Bot ke sini")]
    public Text botScoreLegacy;
    public TextMeshProUGUI botScoreTMP;

    [Header("UI Canvas - Player Feedback")]
    [Tooltip("Masukkan Text untuk tulisan SICK!, GOOD, BAD, MISS ke sini")]
    public Text feedbackLegacy;
    public TextMeshProUGUI feedbackTMP;

    [Header("Scoring Configurations")]
    public float hitWindow = 0.15f; 

    private int playerScore = 0;
    private int playerCombo = 0;
    private int botScore = 0;

    private List<FNFNoteController> activePlayerNotes = new List<FNFNoteController>();

    private void Start()
    {
        UpdatePlayerUI();
        UpdateBotUI();
        ClearFeedback();
    }

    private void Update()
    {
        // Deteksi Input Player (WASD atau Panah)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) TryHitNote(0);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) TryHitNote(1);
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) TryHitNote(2);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) TryHitNote(3);
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
            
            // Sistem Penilaian Feedback berdasarkan selisih waktu (akurasi)
            if (smallestTimeDifference <= 0.05f)
            {
                playerScore += 350;
                ShowFeedback("SICK!!");
            }
            else if (smallestTimeDifference <= 0.1f)
            {
                playerScore += 200;
                ShowFeedback("GOOD!");
            }
            else
            {
                playerScore += 50;
                ShowFeedback("BAD");
            }

            UpdatePlayerUI();
            
            activePlayerNotes.Remove(closestNote);
            Destroy(closestNote.gameObject);
        }
        else
        {
            // Pinalti asal tekan tombol
            playerCombo = 0;
            UpdatePlayerUI();
            ShowFeedback("MISS!");
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

    public void RegisterBotHit()
    {
        // Bot memukul otomatis dan skornya dihitung terpisah
        botScore += 350; 
        UpdateBotUI();
    }

    public void RegisterPlayerMiss()
    {
        playerCombo = 0;
        if (playerScore > 0) playerScore -= 50; 
        
        UpdatePlayerUI();
        ShowFeedback("MISS!");
    }

    // --- FUNGSI UPDATE VISUAL UI ---
    
    private void UpdatePlayerUI()
    {
        string txt = $"Player: {playerScore} | Combo: {playerCombo}";
        if (playerScoreLegacy != null) playerScoreLegacy.text = txt;
        if (playerScoreTMP != null) playerScoreTMP.text = txt;
    }

    private void UpdateBotUI()
    {
        string txt = $"Bot: {botScore}";
        if (botScoreLegacy != null) botScoreLegacy.text = txt;
        if (botScoreTMP != null) botScoreTMP.text = txt;
    }

    private void ShowFeedback(string message)
    {
        if (feedbackLegacy != null) feedbackLegacy.text = message;
        if (feedbackTMP != null) feedbackTMP.text = message;
        
        // Hapus teks feedback setelah 0.5 detik agar tidak menempel terus di layar
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