using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FNFBotAI : MonoBehaviour
{
    [Header("UI Canvas - Bot / Musuh")]
    public Text botScoreLegacy;
    public TextMeshProUGUI botScoreTMP;

    // Logika Probabilitas Bot (Dalam Persen %)
    private float botHitChance = 100f; 

    private int botScore = 0;
    private int botCombo = 0; 

    private void Start()
    {
        UpdateBotUI();
    }

    public void TerapkanDifficulty(int tingkatKesulitan)
    {
        // Kesulitan 1 = 40% sukses (Sangat bodoh). Kesulitan 100 = 100% sukses (Sempurna).
        float factor = tingkatKesulitan / 100f; 
        botHitChance = Mathf.Lerp(40f, 100f, factor);
        
        Debug.Log($"[BotAI] Musuh Level {tingkatKesulitan} | Akurasi Musuh: {botHitChance}%");
    }

    public void EvaluateBotNote(FNFNoteController note)
    {
        float randomRoll = Random.Range(0f, 100f);

        if (randomRoll <= botHitChance)
        {
            botCombo++;
            botScore += 350;
        }
        else
        {
            // Musuh melakukan MISS
            botCombo = 0;
            if (botScore > 0) 
            {
                botScore -= 50; 
                if (botScore < 0) botScore = 0;
            }
            Debug.Log("[BotAI] Meleset! Musuh melakukan MISS!");
        }

        UpdateBotUI();
    }

    private void UpdateBotUI()
    {
        string txt = $"Bot: {botScore} | Combo: {botCombo}";
        if (botScoreLegacy != null) botScoreLegacy.text = txt;
        if (botScoreTMP != null) botScoreTMP.text = txt;
    }
}