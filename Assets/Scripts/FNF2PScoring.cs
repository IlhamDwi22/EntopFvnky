using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FNF2PScoring : MonoBehaviour
{
    [Header("UI Player 1 (TMP)")]
    public TextMeshProUGUI scoreP1Text;
    public TextMeshProUGUI comboP1Text;
    public TextMeshProUGUI feedbackP1Text;

    [Header("UI Player 1 (Legacy)")]
    public Text scoreP1TextLegacy;
    public Text comboP1TextLegacy;
    public Text feedbackP1TextLegacy;

    [Header("UI Player 2 (TMP)")]
    public TextMeshProUGUI scoreP2Text;
    public TextMeshProUGUI comboP2Text;
    public TextMeshProUGUI feedbackP2Text;

    [Header("UI Player 2 (Legacy)")]
    public Text scoreP2TextLegacy;
    public Text comboP2TextLegacy;
    public Text feedbackP2TextLegacy;

    [Header("Layar Pemenang (Winner Screen)")]
    public GameObject panelPemenang;
    public TextMeshProUGUI textNamaPemenang;
    public Text textNamaPemenangLegacy;
    public TextMeshProUGUI textDetailSkor;
    public Text textDetailSkorLegacy;

    [Header("Pengaturan Hit Window")]
    public float hitWindow = 0.15f; 
    public float sickWindow = 0.05f; 
    public float goodWindow = 0.10f; 

    [Header("Skor & Penalti")]
    public int missPenalty = 50;

    private int scoreP1 = 0;
    private int comboP1 = 0;
    private int maxComboP1 = 0;
    private int scoreP2 = 0;
    private int comboP2 = 0;
    private int maxComboP2 = 0;

    private bool gameSelesai = false;

    // List panah aktif di layar
    private List<FNF2PNoteController> activeNotesP1 = new List<FNF2PNoteController>();
    private List<FNF2PNoteController> activeNotesP2 = new List<FNF2PNoteController>();

    private FNF2PNoteController[] currentlyHeldNotesP1 = new FNF2PNoteController[4];
    private FNF2PNoteController[] currentlyHeldNotesP2 = new FNF2PNoteController[4];

    // Animasi Receptor
    private float animDuration = 0.1f;
    private float[] p1ReceptorAnimTimers = new float[4];
    private float[] p2ReceptorAnimTimers = new float[4];
    private Vector3[] baseReceptorScalesP1 = new Vector3[4];
    private Vector3[] baseReceptorScalesP2 = new Vector3[4];
    private Color[] baseReceptorColorsP1 = new Color[4];
    private Color[] baseReceptorColorsP2 = new Color[4];
    private Color[] currentTargetColorsP1 = new Color[4];
    private Color[] currentTargetColorsP2 = new Color[4];
    private Vector3[] currentTargetScalesP1 = new Vector3[4];
    private Vector3[] currentTargetScalesP2 = new Vector3[4];

    private string currentAnimStateP1 = "";
    private string currentAnimStateP2 = "";

    private void Start()
    {
        if (panelPemenang != null) panelPemenang.SetActive(false);

        // Simpan ukuran dan warna dasar receptor
        if (FNF2PConductor.Instance != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (FNF2PConductor.Instance.player1Receptors[i] != null)
                {
                    baseReceptorScalesP1[i] = FNF2PConductor.Instance.player1Receptors[i].localScale;
                    SpriteRenderer sr = FNF2PConductor.Instance.player1Receptors[i].GetComponent<SpriteRenderer>();
                    baseReceptorColorsP1[i] = sr != null ? sr.color : Color.white;
                }
                if (FNF2PConductor.Instance.player2Receptors[i] != null)
                {
                    baseReceptorScalesP2[i] = FNF2PConductor.Instance.player2Receptors[i].localScale;
                    SpriteRenderer sr = FNF2PConductor.Instance.player2Receptors[i].GetComponent<SpriteRenderer>();
                    baseReceptorColorsP2[i] = sr != null ? sr.color : Color.white;
                }
            }
        }

        // Kosongkan teks feedback di awal agar teks placeholder editor (New Text) hilang
        if (feedbackP1Text != null) feedbackP1Text.text = "";
        if (feedbackP1TextLegacy != null) feedbackP1TextLegacy.text = "";
        if (feedbackP2Text != null) feedbackP2Text.text = "";
        if (feedbackP2TextLegacy != null) feedbackP2TextLegacy.text = "";

        UpdateUI();
    }

    private void Update()
    {
        if (gameSelesai) return;
        if (Time.timeScale == 0f) return;

        // --- 1. DETEKSI INPUT PLAYER 1 (WASD atau Custom) ---
        KeyCode keyP1Left = (KeyMappingManager.Instance != null) ? KeyMappingManager.Instance.keyLeft : KeyCode.A;
        KeyCode keyP1Down = (KeyMappingManager.Instance != null) ? KeyMappingManager.Instance.keyDown : KeyCode.S;
        KeyCode keyP1Up = (KeyMappingManager.Instance != null) ? KeyMappingManager.Instance.keyUp : KeyCode.W;
        KeyCode keyP1Right = (KeyMappingManager.Instance != null) ? KeyMappingManager.Instance.keyRight : KeyCode.D;

        if (Input.GetKeyDown(keyP1Left)) TryHitNote(0, false);
        if (Input.GetKeyDown(keyP1Down)) TryHitNote(1, false);
        if (Input.GetKeyDown(keyP1Up)) TryHitNote(2, false);
        if (Input.GetKeyDown(keyP1Right)) TryHitNote(3, false);

        if (Input.GetKeyUp(keyP1Left)) TryReleaseNote(0, false);
        if (Input.GetKeyUp(keyP1Down)) TryReleaseNote(1, false);
        if (Input.GetKeyUp(keyP1Up)) TryReleaseNote(2, false);
        if (Input.GetKeyUp(keyP1Right)) TryReleaseNote(3, false);

        // --- 2. DETEKSI INPUT PLAYER 2 (Arrows - Non-Customizable) ---
        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryHitNote(0, true);
        if (Input.GetKeyDown(KeyCode.DownArrow)) TryHitNote(1, true);
        if (Input.GetKeyDown(KeyCode.UpArrow)) TryHitNote(2, true);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryHitNote(3, true);

        if (Input.GetKeyUp(KeyCode.LeftArrow)) TryReleaseNote(0, true);
        if (Input.GetKeyUp(KeyCode.DownArrow)) TryReleaseNote(1, true);
        if (Input.GetKeyUp(KeyCode.UpArrow)) TryReleaseNote(2, true);
        if (Input.GetKeyUp(KeyCode.RightArrow)) TryReleaseNote(3, true);

        // --- 3. ANIMASI DANCE/DENGAN TOMBOL ---
        // Player 1 Anim
        if (Input.GetKey(keyP1Left)) MainkanAnimasi(false, "Left");
        else if (Input.GetKey(keyP1Down)) MainkanAnimasi(false, "Down");
        else if (Input.GetKey(keyP1Up)) MainkanAnimasi(false, "Up");
        else if (Input.GetKey(keyP1Right)) MainkanAnimasi(false, "Right");
        else MainkanAnimasi(false, "Idle");

        // Player 2 Anim
        if (Input.GetKey(KeyCode.LeftArrow)) MainkanAnimasi(true, "Left");
        else if (Input.GetKey(KeyCode.DownArrow)) MainkanAnimasi(true, "Down");
        else if (Input.GetKey(KeyCode.UpArrow)) MainkanAnimasi(true, "Up");
        else if (Input.GetKey(KeyCode.RightArrow)) MainkanAnimasi(true, "Right");
        else MainkanAnimasi(true, "Idle");

        // --- 4. HILANGKAN NYAWA JIKA KELAMAAN HOLD DIBATALKAN ---
        UpdateHoldNotesProgress();

        // --- 5. PERBARUI ANIMASI RECEPTOR ---
        UpdateReceptorAnimations();
    }

    private void TryHitNote(int lane, bool isP2)
    {
        List<FNF2PNoteController> activeNotes = isP2 ? activeNotesP2 : activeNotesP1;
        FNF2PNoteController targetNote = null;
        float minimumDifference = float.MaxValue;

        foreach (var note in activeNotes)
        {
            if (note.GetDetails().lane == lane)
            {
                float diff = Mathf.Abs(note.GetDetails().hitTime - FNF2PConductor.Instance.currentSongTime);
                if (diff < minimumDifference && diff <= hitWindow)
                {
                    minimumDifference = diff;
                    targetNote = note;
                }
            }
        }

        if (targetNote != null)
        {
            float diff = Mathf.Abs(targetNote.GetDetails().hitTime - FNF2PConductor.Instance.currentSongTime);
            string rating = "GOOD";

            if (diff <= sickWindow) rating = "SICK";
            else if (diff <= goodWindow) rating = "GOOD";
            else rating = "BAD";

            // Tambah skor & combo
            int bonusSkor = (rating == "SICK") ? 350 : (rating == "GOOD") ? 200 : 50;
            
            if (isP2)
            {
                scoreP2 += bonusSkor;
                comboP2++;
                ShowFeedback(true, rating);
            }
            else
            {
                scoreP1 += bonusSkor;
                comboP1++;
                ShowFeedback(false, rating);
            }

            TriggerReceptorEffect(lane, isP2, rating);
            UpdateUI();

            if (targetNote.GetDetails().duration > 0)
            {
                if (isP2) currentlyHeldNotesP2[lane] = targetNote;
                else currentlyHeldNotesP1[lane] = targetNote;

                targetNote.isBeingHeld = true;
            }
            else
            {
                targetNote.DestroyNoteAndRemove();
            }
        }
        else
        {
            // PUKULAN KOSONG / SALAH (MISS)
            RegisterMiss(lane, isP2);
        }
    }

    private void TryReleaseNote(int lane, bool isP2)
    {
        FNF2PNoteController[] currentlyHeldNotes = isP2 ? currentlyHeldNotesP2 : currentlyHeldNotesP1;

        if (currentlyHeldNotes[lane] != null)
        {
            FNF2PNoteController heldNote = currentlyHeldNotes[lane];
            float remainingHold = (heldNote.GetDetails().hitTime + heldNote.GetDetails().duration) - FNF2PConductor.Instance.currentSongTime;

            if (remainingHold > 0.1f)
            {
                RegisterMiss(lane, isP2);
            }

            heldNote.DestroyNoteAndRemove();
            currentlyHeldNotes[lane] = null;
        }
    }

    private void UpdateHoldNotesProgress()
    {
        for (int i = 0; i < 4; i++)
        {
            // Player 1 Hold Note Progress
            if (currentlyHeldNotesP1[i] != null)
            {
                FNF2PNoteController note = currentlyHeldNotesP1[i];
                float endTime = note.GetDetails().hitTime + note.GetDetails().duration;

                if (FNF2PConductor.Instance.currentSongTime >= endTime - 0.05f)
                {
                    scoreP1 += 100;
                    comboP1++;
                    ShowFeedback(false, "NICE HOLD!");
                    currentlyHeldNotesP1[i] = null;
                }
                else
                {
                    scoreP1 += 1;
                }
            }

            // Player 2 Hold Note Progress
            if (currentlyHeldNotesP2[i] != null)
            {
                FNF2PNoteController note = currentlyHeldNotesP2[i];
                float endTime = note.GetDetails().hitTime + note.GetDetails().duration;

                if (FNF2PConductor.Instance.currentSongTime >= endTime - 0.05f)
                {
                    scoreP2 += 100;
                    comboP2++;
                    ShowFeedback(true, "NICE HOLD!");
                    currentlyHeldNotesP2[i] = null;
                }
                else
                {
                    scoreP2 += 1;
                }
            }
        }

        UpdateUI();
    }

    public void TriggerNoteMiss(FNF2PNoteController note)
    {
        RegisterMiss(note.GetDetails().lane, note.GetDetails().isPlayer2);
    }

    private void RegisterMiss(int lane, bool isP2)
    {
        if (isP2)
        {
            comboP2 = 0;
            scoreP2 = Mathf.Max(0, scoreP2 - missPenalty);
            ShowFeedback(true, "MISS!");
        }
        else
        {
            comboP1 = 0;
            scoreP1 = Mathf.Max(0, scoreP1 - missPenalty);
            ShowFeedback(false, "MISS!");
        }

        UpdateUI();
    }

    public void TampilkanLayarPemenang(string pemenangManual = "")
    {
        gameSelesai = true;
        
        string pemenangText = pemenangManual;
        if (string.IsNullOrEmpty(pemenangText))
        {
            if (scoreP1 > scoreP2) pemenangText = "PLAYER 1 MENANG!";
            else if (scoreP2 > scoreP1) pemenangText = "PLAYER 2 MENANG!";
            else pemenangText = "DRAW / SERI!";
        }

        if (textNamaPemenang != null) textNamaPemenang.text = pemenangText;
        if (textNamaPemenangLegacy != null) textNamaPemenangLegacy.text = pemenangText;

        string detailText = $"PLAYER 1:\nScore: {scoreP1}\nMax Combo: {maxComboP1}\n\nPLAYER 2:\nScore: {scoreP2}\nMax Combo: {maxComboP2}";
        if (textDetailSkor != null) textDetailSkor.text = detailText;
        if (textDetailSkorLegacy != null) textDetailSkorLegacy.text = detailText;

        if (panelPemenang != null) panelPemenang.SetActive(true);
    }

    private void MainkanAnimasi(bool isP2, string namaAnimasi)
    {
        if (isP2)
        {
            if (FNF2PConductor.Instance != null && FNF2PConductor.Instance.p2Anim != null && currentAnimStateP2 != namaAnimasi)
            {
                FNF2PConductor.Instance.p2Anim.Play(namaAnimasi);
                currentAnimStateP2 = namaAnimasi;
            }
        }
        else
        {
            if (FNF2PConductor.Instance != null && FNF2PConductor.Instance.p1Anim != null && currentAnimStateP1 != namaAnimasi)
            {
                FNF2PConductor.Instance.p1Anim.Play(namaAnimasi);
                currentAnimStateP1 = namaAnimasi;
            }
        }
    }

    private void TriggerReceptorEffect(int lane, bool isP2, string rating)
    {
        Transform[] receptors = isP2 ? FNF2PConductor.Instance.player2Receptors : FNF2PConductor.Instance.player1Receptors;
        if (receptors == null || lane < 0 || lane >= 4 || receptors[lane] == null) return;

        Transform receptor = receptors[lane];
        SpriteRenderer sr = receptor.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Color targetColor = Color.white;
        Vector3 baseScale = isP2 ? baseReceptorScalesP2[lane] : baseReceptorScalesP1[lane];
        Vector3 targetScale = baseScale;

        if (rating == "SICK" || rating == "GOOD")
        {
            targetColor = new Color(0.5f, 1f, 1f, 1f); 
            targetScale = baseScale * 1.3f;          
        }
        else if (rating == "BAD")
        {
            targetColor = new Color(1f, 0.8f, 0.2f, 1f); 
            targetScale = baseScale * 1.15f;           
        }
        else if (rating == "MISS")
        {
            targetColor = new Color(1f, 0.3f, 0.3f, 1f); 
            targetScale = baseScale * 0.8f;            
        }

        if (isP2)
        {
            currentTargetColorsP2[lane] = targetColor;
            currentTargetScalesP2[lane] = targetScale;
            p2ReceptorAnimTimers[lane] = animDuration;
        }
        else
        {
            currentTargetColorsP1[lane] = targetColor;
            currentTargetScalesP1[lane] = targetScale;
            p1ReceptorAnimTimers[lane] = animDuration;
        }

        sr.color = targetColor;
        receptor.localScale = targetScale;
    }

    private void UpdateReceptorAnimations()
    {
        if (FNF2PConductor.Instance == null) return;

        KeyCode keyP1Left = (KeyMappingManager.Instance != null) ? KeyMappingManager.Instance.keyLeft : KeyCode.A;
        KeyCode keyP1Down = (KeyMappingManager.Instance != null) ? KeyMappingManager.Instance.keyDown : KeyCode.S;
        KeyCode keyP1Up = (KeyMappingManager.Instance != null) ? KeyMappingManager.Instance.keyUp : KeyCode.W;
        KeyCode keyP1Right = (KeyMappingManager.Instance != null) ? KeyMappingManager.Instance.keyRight : KeyCode.D;

        for (int i = 0; i < 4; i++)
        {
            // --- 1. RECEPTOR PLAYER 1 ---
            bool isP1Pressed = false;
            if (i == 0 && Input.GetKey(keyP1Left)) isP1Pressed = true;
            if (i == 1 && Input.GetKey(keyP1Down)) isP1Pressed = true;
            if (i == 2 && Input.GetKey(keyP1Up)) isP1Pressed = true;
            if (i == 3 && Input.GetKey(keyP1Right)) isP1Pressed = true;

            Transform recP1 = FNF2PConductor.Instance.player1Receptors[i];
            if (recP1 != null)
            {
                SpriteRenderer srP1 = recP1.GetComponent<SpriteRenderer>();
                if (srP1 != null)
                {
                    if (isP1Pressed)
                    {
                        // Jika ada rating hit tersimpan, gunakan itu. Jika tidak, nyalakan putih terang
                        srP1.color = currentTargetColorsP1[i] != Color.clear ? currentTargetColorsP1[i] : Color.white;
                        // Skala membesar 15% jika ditekan kosong, atau gunakan target rating scale jika ada note hit
                        recP1.localScale = currentTargetScalesP1[i] != Vector3.zero ? currentTargetScalesP1[i] : baseReceptorScalesP1[i] * 1.15f;
                    }
                    else if (p1ReceptorAnimTimers[i] > 0)
                    {
                        p1ReceptorAnimTimers[i] -= Time.deltaTime;
                        if (p1ReceptorAnimTimers[i] <= 0)
                        {
                            srP1.color = baseReceptorColorsP1[i];
                            recP1.localScale = baseReceptorScalesP1[i];
                        }
                        else
                        {
                            float t = 1f - (p1ReceptorAnimTimers[i] / animDuration);
                            srP1.color = Color.Lerp(currentTargetColorsP1[i], baseReceptorColorsP1[i], t);
                            recP1.localScale = Vector3.Lerp(currentTargetScalesP1[i], baseReceptorScalesP1[i], t);
                        }
                    }
                    else
                    {
                        // Kembali ke warna dasar redup/transparan dan skala dasar
                        srP1.color = baseReceptorColorsP1[i];
                        recP1.localScale = baseReceptorScalesP1[i];
                    }
                }
            }

            // --- 2. RECEPTOR PLAYER 2 ---
            bool isP2Pressed = false;
            if (i == 0 && Input.GetKey(KeyCode.LeftArrow)) isP2Pressed = true;
            if (i == 1 && Input.GetKey(KeyCode.DownArrow)) isP2Pressed = true;
            if (i == 2 && Input.GetKey(KeyCode.UpArrow)) isP2Pressed = true;
            if (i == 3 && Input.GetKey(KeyCode.RightArrow)) isP2Pressed = true;

            Transform recP2 = FNF2PConductor.Instance.player2Receptors[i];
            if (recP2 != null)
            {
                SpriteRenderer srP2 = recP2.GetComponent<SpriteRenderer>();
                if (srP2 != null)
                {
                    if (isP2Pressed)
                    {
                        // Jika ada rating hit tersimpan, gunakan itu. Jika tidak, nyalakan putih terang
                        srP2.color = currentTargetColorsP2[i] != Color.clear ? currentTargetColorsP2[i] : Color.white;
                        // Skala membesar 15% jika ditekan kosong, atau gunakan target rating scale jika ada note hit
                        recP2.localScale = currentTargetScalesP2[i] != Vector3.zero ? currentTargetScalesP2[i] : baseReceptorScalesP2[i] * 1.15f;
                    }
                    else if (p2ReceptorAnimTimers[i] > 0)
                    {
                        p2ReceptorAnimTimers[i] -= Time.deltaTime;
                        if (p2ReceptorAnimTimers[i] <= 0)
                        {
                            srP2.color = baseReceptorColorsP2[i];
                            recP2.localScale = baseReceptorScalesP2[i];
                        }
                        else
                        {
                            float t = 1f - (p2ReceptorAnimTimers[i] / animDuration);
                            srP2.color = Color.Lerp(currentTargetColorsP2[i], baseReceptorColorsP2[i], t);
                            recP2.localScale = Vector3.Lerp(currentTargetScalesP2[i], baseReceptorScalesP2[i], t);
                        }
                    }
                    else
                    {
                        // Kembali ke warna dasar redup/transparan dan skala dasar
                        srP2.color = baseReceptorColorsP2[i];
                        recP2.localScale = baseReceptorScalesP2[i];
                    }
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (comboP1 > maxComboP1) maxComboP1 = comboP1;
        if (comboP2 > maxComboP2) maxComboP2 = comboP2;

        string p1Score = $"P1: {scoreP1}";
        if (scoreP1Text != null) scoreP1Text.text = p1Score;
        if (scoreP1TextLegacy != null) scoreP1TextLegacy.text = p1Score;

        string p1Combo = comboP1 > 0 ? $"Combo x{comboP1}" : "";
        if (comboP1Text != null) comboP1Text.text = p1Combo;
        if (comboP1TextLegacy != null) comboP1TextLegacy.text = p1Combo;

        string p2Score = $"P2: {scoreP2}";
        if (scoreP2Text != null) scoreP2Text.text = p2Score;
        if (scoreP2TextLegacy != null) scoreP2TextLegacy.text = p2Score;

        string p2Combo = comboP2 > 0 ? $"Combo x{comboP2}" : "";
        if (comboP2Text != null) comboP2Text.text = p2Combo;
        if (comboP2TextLegacy != null) comboP2TextLegacy.text = p2Combo;
    }

    private void ShowFeedback(bool isP2, string rating)
    {
        TextMeshProUGUI tmp = isP2 ? feedbackP2Text : feedbackP1Text;
        Text legacy = isP2 ? feedbackP2TextLegacy : feedbackP1TextLegacy;

        if (tmp != null) tmp.text = rating;
        if (legacy != null) legacy.text = rating;

        StartCoroutine(ClearFeedbackRoutine(tmp, legacy));
    }

    private System.Collections.IEnumerator ClearFeedbackRoutine(TextMeshProUGUI targetTMP, Text targetLegacy)
    {
        yield return new WaitForSeconds(0.5f);
        if (targetTMP != null) targetTMP.text = "";
        if (targetLegacy != null) targetLegacy.text = "";
    }

    public void RegisterNoteInGame(FNF2PNoteController note)
    {
        if (note.GetDetails().isPlayer2) activeNotesP2.Add(note);
        else activeNotesP1.Add(note);
    }

    public void RemoveNoteFromActiveList(FNF2PNoteController note)
    {
        if (note.GetDetails().isPlayer2) activeNotesP2.Remove(note);
        else activeNotesP1.Remove(note);
    }

    // --- BUTTON ACTIONS ---
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToSelectionMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu2PlayerScene"); // Pastikan nama scene menu pilihan 2P cocok
    }
}
