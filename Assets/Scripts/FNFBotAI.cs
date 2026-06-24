using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FNFBotAI : MonoBehaviour
{
    [Header("UI Canvas - Bot / Musuh")]
    public Text botScoreLegacy;
    public TextMeshProUGUI botScoreTMP;

    [Header("Animasi Bot")]
    public Animator botAnim;

    private float botHitChance = 100f; 
    public int botScore = 0;
    private int botCombo = 0; 
    
    private float botIdleTimer = 0f;

    private void Start()
    {
        UpdateBotUI();
    }

    private void Update()
    {
        if (botIdleTimer > 0)
        {
            botIdleTimer -= Time.deltaTime;
            if (botIdleTimer <= 0 && botAnim != null)
            {
                botAnim.Play("Idle");
            }
        }
    }

    public void TerapkanDifficulty(int tingkatKesulitan)
    {
        float factor = tingkatKesulitan / 100f; 
        botHitChance = Mathf.Lerp(40f, 100f, factor);
    }

    public bool EvaluateBotNote(FNFNoteController note)
    {
        float randomRoll = Random.Range(0f, 100f);

        if (randomRoll <= botHitChance)
        {
            botCombo++;
            botScore += 350;
            UpdateBotUI();
            
            // Bot akan menahan posenya selama durasi panah tersebut (atau 0.4 detik jika panah biasa)
            float tahanAnimasi = note.GetDetails().duration > 0 ? note.GetDetails().duration : 0.4f;
            MainkanAnimasiBot(note.GetDetails().lane, tahanAnimasi);
            
            return true; 
        }
        else
        {
            botCombo = 0;
            if (botScore > 0) 
            {
                botScore -= 50; 
                if (botScore < 0) botScore = 0;
            }
            UpdateBotUI();
            return false; 
        }
    }

    private void MainkanAnimasiBot(int lane, float durasiHold)
    {
        if (botAnim == null) return;
        
        string namaAnimasi = "Idle";
        if (lane == 0) namaAnimasi = "Left";
        if (lane == 1) namaAnimasi = "Down";
        if (lane == 2) namaAnimasi = "Up";
        if (lane == 3) namaAnimasi = "Right";

        botAnim.Play(namaAnimasi, 0, 0f); 
        
        // Pose animasi di-hold sepanjang buntut panah tersebut!
        botIdleTimer = durasiHold; 
    }

    private void UpdateBotUI()
    {
        string txt = $"Bot: {botScore} | Combo: {botCombo}";
        if (botScoreLegacy != null) botScoreLegacy.text = txt;
        if (botScoreTMP != null) botScoreTMP.text = txt;
    }
}