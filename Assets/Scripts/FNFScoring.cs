using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class FNFScoring : MonoBehaviour
{
    [Header("UI Canvas - Player Score")]
    public Text playerScoreLegacy;
    public TextMeshProUGUI playerScoreTMP;

    [Header("UI Canvas - Player Combo (BARU)")]
    [Tooltip("Tarik teks khusus Combo ke sini agar posisinya bisa dipisah dari Skor")]
    public Text playerComboLegacy;
    public TextMeshProUGUI playerComboTMP;
    
    [Header("UI Canvas - Player Feedback")]
    public Text feedbackLegacy;
    public TextMeshProUGUI feedbackTMP;

    [Header("Animasi Player")]
    public Animator playerAnim;

    private float hitWindow = 0.15f; 
    private float sickWindow = 0.05f;
    private float goodWindow = 0.1f;
    private int missPenalty = 50;

    public int playerScore = 0; 
    private int playerCombo = 0;
    
    private List<FNFNoteController> activePlayerNotes = new List<FNFNoteController>();
    private FNFNoteController[] currentlyHeldNotes = new FNFNoteController[4];

    private Vector3[] baseReceptorScales = new Vector3[4];
    private float[] receptorAnimTimers = new float[4];
    private float animDuration = 0.15f;
    private Color[] currentTargetColors = new Color[4];
    private Vector3[] currentTargetScales = new Vector3[4];
    
    private string currentAnimState = "";

    private void Start()
    {
        UpdatePlayerUI();
        ClearFeedback();
        
        if (KeyMappingManager.Instance == null)
        {
            GameObject managerObj = new GameObject("KeyMappingManager");
            managerObj.AddComponent<KeyMappingManager>();
        }
        
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
        if (KeyMappingManager.Instance == null) return;

        // 1. DETEKSI PUKULAN AWAL
        if (Input.GetKeyDown(KeyMappingManager.Instance.keyLeft) || Input.GetKeyDown(KeyCode.LeftArrow)) TryHitNote(0);
        if (Input.GetKeyDown(KeyMappingManager.Instance.keyDown) || Input.GetKeyDown(KeyCode.DownArrow)) TryHitNote(1);
        if (Input.GetKeyDown(KeyMappingManager.Instance.keyUp) || Input.GetKeyDown(KeyCode.UpArrow)) TryHitNote(2);
        if (Input.GetKeyDown(KeyMappingManager.Instance.keyRight) || Input.GetKeyDown(KeyCode.RightArrow)) TryHitNote(3);

        // 2. DETEKSI PELEPASAN TOMBOL
        if (Input.GetKeyUp(KeyMappingManager.Instance.keyLeft) || Input.GetKeyUp(KeyCode.LeftArrow)) TryReleaseNote(0);
        if (Input.GetKeyUp(KeyMappingManager.Instance.keyDown) || Input.GetKeyUp(KeyCode.DownArrow)) TryReleaseNote(1);
        if (Input.GetKeyUp(KeyMappingManager.Instance.keyUp) || Input.GetKeyUp(KeyCode.UpArrow)) TryReleaseNote(2);
        if (Input.GetKeyUp(KeyMappingManager.Instance.keyRight) || Input.GetKeyUp(KeyCode.RightArrow)) TryReleaseNote(3);

        UpdateReceptorAnimations();

        // 3. SISTEM ANIMASI TAHAN TOMBOL
        if (Input.GetKey(KeyMappingManager.Instance.keyLeft) || Input.GetKey(KeyCode.LeftArrow)) MainkanAnimasiPlayer("Left");
        else if (Input.GetKey(KeyMappingManager.Instance.keyDown) || Input.GetKey(KeyCode.DownArrow)) MainkanAnimasiPlayer("Down");
        else if (Input.GetKey(KeyMappingManager.Instance.keyUp) || Input.GetKey(KeyCode.UpArrow)) MainkanAnimasiPlayer("Up");
        else if (Input.GetKey(KeyMappingManager.Instance.keyRight) || Input.GetKey(KeyCode.RightArrow)) MainkanAnimasiPlayer("Right");
        else MainkanAnimasiPlayer("Idle"); 

        // 4. PENYELESAIAN HOLD NOTE UNTUK SKOR
        for (int i = 0; i < 4; i++)
        {
            if (currentlyHeldNotes[i] != null)
            {
                FNFNoteController heldNote = currentlyHeldNotes[i];
                float waktuSelesai = heldNote.GetDetails().hitTime + heldNote.GetDetails().duration;

                if (FNFConductor.Instance.currentSongTime >= waktuSelesai - 0.05f)
                {
                    playerScore += 100;
                    playerCombo++;
                    ShowFeedback("NICE HOLD!");
                    UpdatePlayerUI();

                    heldNote.DestroyNoteAndRemove();
                    currentlyHeldNotes[i] = null;
                }
                else
                {
                    playerScore += 1;
                    UpdatePlayerUI();
                }
            }
        }
    }

    private void MainkanAnimasiPlayer(string namaAnimasi)
    {
        if (playerAnim != null && currentAnimState != namaAnimasi)
        {
            playerAnim.Play(namaAnimasi);
            currentAnimState = namaAnimasi; 
        }
    }

    private void TryHitNote(int inputLane)
    {
        FNFNoteController closestNote = null;
        float smallestTimeDifference = float.MaxValue;

        foreach (FNFNoteController note in activePlayerNotes)
        {
            if (note == null || note.isBeingHeld) continue; 
            
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
            
            if (smallestTimeDifference <= sickWindow) { playerScore += 350; ShowFeedback("SICK!!"); TriggerReceptorEffect(inputLane, "SICK"); }
            else if (smallestTimeDifference <= goodWindow) { playerScore += 200; ShowFeedback("GOOD!"); TriggerReceptorEffect(inputLane, "GOOD"); }
            else { playerScore += 50; ShowFeedback("BAD"); TriggerReceptorEffect(inputLane, "BAD"); }

            UpdatePlayerUI();
            
            if (closestNote.GetDetails().duration > 0.05f)
            {
                closestNote.isBeingHeld = true;
                currentlyHeldNotes[inputLane] = closestNote;
            }
            else
            {
                closestNote.DestroyNoteAndRemove(); 
            }
        }
        else
        {
            RegisterPlayerMiss();
            TriggerReceptorEffect(inputLane, "MISS");
        }
    }

    private void TryReleaseNote(int lane)
    {
        if (currentlyHeldNotes[lane] != null)
        {
            FNFNoteController heldNote = currentlyHeldNotes[lane];
            float sisaWaktuBuntut = (heldNote.GetDetails().hitTime + heldNote.GetDetails().duration) - FNFConductor.Instance.currentSongTime;
            
            if (sisaWaktuBuntut > 0.1f) 
            {
                RegisterPlayerMiss();
            }
            
            heldNote.DestroyNoteAndRemove();
            currentlyHeldNotes[lane] = null;
        }
    }

    public void RegisterNoteInGame(FNFNoteController note) { if (!note.GetDetails().isBot) activePlayerNotes.Add(note); }
    public void RegisterNoteLeaveGame(FNFNoteController note) { if (activePlayerNotes.Contains(note)) activePlayerNotes.Remove(note); }

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

        sr.color = currentTargetColors[lane];
        receptor.localScale = currentTargetScales[lane];
        receptorAnimTimers[lane] = animDuration;
    }

    private void UpdateReceptorAnimations()
    {
        if (FNFConductor.Instance == null || FNFConductor.Instance.playerReceptors == null) return;

        for (int i = 0; i < 4; i++)
        {
            bool isKeyPressed = false;
            
            if (i == 0 && (Input.GetKey(KeyMappingManager.Instance.keyLeft) || Input.GetKey(KeyCode.LeftArrow))) isKeyPressed = true;
            if (i == 1 && (Input.GetKey(KeyMappingManager.Instance.keyDown) || Input.GetKey(KeyCode.DownArrow))) isKeyPressed = true;
            if (i == 2 && (Input.GetKey(KeyMappingManager.Instance.keyUp) || Input.GetKey(KeyCode.UpArrow))) isKeyPressed = true;
            if (i == 3 && (Input.GetKey(KeyMappingManager.Instance.keyRight) || Input.GetKey(KeyCode.RightArrow))) isKeyPressed = true;

            Transform receptor = FNFConductor.Instance.playerReceptors[i];
            if (receptor == null) continue;
            
            SpriteRenderer sr = receptor.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            if (isKeyPressed)
            {
                sr.color = currentTargetColors[i] != Color.clear ? currentTargetColors[i] : Color.white;
                receptor.localScale = currentTargetScales[i] != Vector3.zero ? currentTargetScales[i] : baseReceptorScales[i];
                continue; 
            }

            if (receptorAnimTimers[i] > 0)
            {
                receptorAnimTimers[i] -= Time.deltaTime;
                if (receptorAnimTimers[i] <= 0)
                {
                    sr.color = Color.white;
                    receptor.localScale = baseReceptorScales[i];
                }
                else
                {
                    float t = 1f - (receptorAnimTimers[i] / animDuration); 
                    sr.color = Color.Lerp(currentTargetColors[i], Color.white, t);
                    receptor.localScale = Vector3.Lerp(currentTargetScales[i], baseReceptorScales[i], t);
                }
            }
        }
    }

    // --- REFORMASI LOGIKA UPDATE UI (DIPISAH TOTAL) ---
    private void UpdatePlayerUI()
    {
        // 1. Mengurus Teks Skor Player
        string formatSkor = $" {playerScore}";
        if (playerScoreLegacy != null) playerScoreLegacy.text = formatSkor;
        if (playerScoreTMP != null) playerScoreTMP.text = formatSkor;

        // 2. Mengurus Teks Combo Player (Sekarang punya jalurnya sendiri)
        string formatCombo = playerCombo > 0 ? $"Combo x{playerCombo}" : ""; // Hilangkan tulisan jika combo 0
        if (playerComboLegacy != null) playerComboLegacy.text = formatCombo;
        if (playerComboTMP != null) playerComboTMP.text = formatCombo;
    }

    private void ShowFeedback(string message)
    {
        if (feedbackLegacy != null) feedbackLegacy.text = message;
        if (feedbackTMP != null) feedbackTMP.text = message;
        StopAllCoroutines();
        StartCoroutine(ClearFeedbackRoutine());
    }

    private void ClearFeedback() { if (feedbackLegacy != null) feedbackLegacy.text = ""; if (feedbackTMP != null) feedbackTMP.text = ""; }
    private IEnumerator ClearFeedbackRoutine() { yield return new WaitForSeconds(0.5f); ClearFeedback(); }
}