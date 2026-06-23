using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FNFBotAI : MonoBehaviour
{
    [Header("UI Canvas - Bot / Musuh")]
    public Text botScoreLegacy;
    public TextMeshProUGUI botScoreTMP;

    private float botHitChance = 100f; 
    private int botScore = 0;
    private int botCombo = 0; 

    private void Start()
    {
        UpdateBotUI();
    }

    public void TerapkanDifficulty(int tingkatKesulitan)
    {
        float factor = tingkatKesulitan / 100f; 
        botHitChance = Mathf.Lerp(40f, 100f, factor);
    }

    // Fungsi ini sekarang mengembalikan 'bool' agar panah tahu bahwa Bot sukses menekannya
    public bool EvaluateBotNote(FNFNoteController note)
    {
        float randomRoll = Random.Range(0f, 100f);

        if (randomRoll <= botHitChance)
        {
            botCombo++;
            botScore += 350;
            UpdateBotUI();
            return true; // Berhasil! (Jika hold note, bot akan menahannya)
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
            return false; // Meleset!
        }
    }

    private void UpdateBotUI()
    {
        string txt = $"Bot: {botScore} | Combo: {botCombo}";
        if (botScoreLegacy != null) botScoreLegacy.text = txt;
        if (botScoreTMP != null) botScoreTMP.text = txt;
    }
}