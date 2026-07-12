using UnityEngine;
using System.Collections.Generic;

public class FNFConductor : MonoBehaviour
{
    public static FNFConductor Instance;

    [Header("Komponen Wajib")]
    public AudioSource musicSource;
    public GameObject notePrefab;
    public Transform[] playerReceptors; 
    public Transform[] botReceptors;    
    public FNFScoring scoringSystem;
    public FNFBotAI botAI; 

    [Header("Visualisasi Gameplay")]
    public SpriteRenderer visualKarakterBot;
    
    [Tooltip("Masukkan 4 gambar KEPALA panah: 0=Kiri, 1=Bawah, 2=Atas, 3=Kanan")]
    public Sprite[] noteSprites = new Sprite[4];

    [Tooltip("Masukkan 4 gambar EKOR panah (Hold): 0=Kiri, 1=Bawah, 2=Atas, 3=Kanan")]
    public Sprite[] holdSprites = new Sprite[4]; // <-- WADAH BARU UNTUK GAMBAR EKOR

    [Header("Pengaturan Jarak Spawn")]
    public float spawnAheadTime = 3f;

    [HideInInspector] public float speed; 
    [HideInInspector] public float audioOffsetAdjustment;

    public float currentSongTime { get; private set; }
    private bool isSongPlaying = false;
    private bool hasStartedPlaying = false;
    private bool battleEnded = false; 
    
    private List<FNFNoteData> sequencedNotes = new List<FNFNoteData>();
    private int nextNoteIndex = 0;
    private float introTimer = 0f;
    private float songDelay = 1f;

    private struct BPMChange { public float tick; public float timeInSeconds; public float bpm; }
    private List<BPMChange> bpmHistory = new List<BPMChange>();

    private void Awake() { Instance = this; }

    private void Start()
    {
        Time.timeScale = 1f;

        if (GameManager.musuhPilihanSaatIni == null || GameManager.musuhPilihanSaatIni.daftarLagu.Length == 0)
        {
            Debug.LogError("[FNFConductor] Gagal! Tidak ada data musuh atau lagunya kosong.");
            return;
        }

        OpponentData dataMusuh = GameManager.musuhPilihanSaatIni;
        SongData dataLagu = dataMusuh.daftarLagu[GameManager.urutanLaguSaatIni];

        if (visualKarakterBot != null)
        {
            if (dataMusuh.visualMusuh != null)
            {
                visualKarakterBot.sprite = dataMusuh.visualMusuh;
            }
            
            Animator botAnimController = visualKarakterBot.GetComponent<Animator>();
            if (botAnimController != null && dataMusuh.animasiMusuh != null)
            {
                botAnimController.runtimeAnimatorController = dataMusuh.animasiMusuh;
            }
        }

        // Pastikan Animator yang dipegang oleh botAI juga memakai controller baru
        if (botAI != null && botAI.botAnim != null && dataMusuh.animasiMusuh != null)
        {
            botAI.botAnim.runtimeAnimatorController = dataMusuh.animasiMusuh;
        }

        int kesulitan = dataMusuh.tingkatKesulitan;
        float speedMultiplier = 1f + ((kesulitan - 50) / 100f); 
        
        musicSource.clip = dataLagu.fileAudio;
        speed = dataLagu.kecepatanScroll * speedMultiplier;
        songDelay = dataLagu.delayMulai;
        audioOffsetAdjustment = dataLagu.penyesuaianOffset;

        if (scoringSystem != null) scoringSystem.TerapkanDifficulty(kesulitan);
        if (botAI != null) botAI.TerapkanDifficulty(kesulitan);

        ParseChartDinamis(dataLagu.fileChart.text);

        introTimer = -songDelay;
        currentSongTime = introTimer;
        isSongPlaying = true;
    }

    private void Update()
    {
        if (!isSongPlaying || battleEnded) return;

        if (musicSource != null)
        {
            if (!hasStartedPlaying)
            {
                introTimer += Time.deltaTime;
                currentSongTime = introTimer;

                if (introTimer >= 0)
                {
                    if (musicSource.clip != null && !musicSource.isPlaying)
                    {
                        musicSource.Play();
                    }
                    hasStartedPlaying = true; 
                }
            }
            else
            {
                if (musicSource.isPlaying)
                {
                    currentSongTime = musicSource.time + audioOffsetAdjustment;
                }
                else
                {
                    // Hanya selesaikan pertarungan jika lagu benar-benar sudah mencapai bagian akhir (mencegah bug Alt-Tab)
                    if (musicSource.clip != null && currentSongTime >= musicSource.clip.length - 0.5f)
                    {
                        EndBattle();
                    }
                }
            }
        }

        while (nextNoteIndex < sequencedNotes.Count && sequencedNotes[nextNoteIndex].hitTime - currentSongTime <= spawnAheadTime)
        {
            SpawnFNFNote(sequencedNotes[nextNoteIndex]);
            nextNoteIndex++;
        }
    }

    private void EndBattle()
    {
        battleEnded = true;
        isSongPlaying = false;
        
        GlobalBattleState.kembaliDariBattle = true;

        int skorPlayer = scoringSystem != null ? scoringSystem.playerScore : 0;
        int skorBot = botAI != null ? botAI.botScore : 0;

        GlobalBattleState.playerMenang = (skorPlayer >= skorBot);

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.PindahSceneDenganFade(GlobalBattleState.sceneOverworldAsal);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GlobalBattleState.sceneOverworldAsal);
        }
    }

    private void ParseChartDinamis(string chartText)
    {
        sequencedNotes.Clear();
        bpmHistory.Clear();
        string[] lines = chartText.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        
        float chartResolution = 192f;
        List<string> syncTrackLines = new List<string>();
        List<string> expertNotesLines = new List<string>();
        bool inSong = false, inSync = false, inExpert = false;

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (trimmed.StartsWith("["))
            {
                inSong = trimmed.Equals("[Song]");
                inSync = trimmed.Equals("[SyncTrack]");
                inExpert = trimmed.Equals("[ExpertSingle]");
                continue;
            }
            if (trimmed.Equals("}"))
            {
                inSong = inSync = inExpert = false; continue;
            }

            if (inSong && trimmed.Contains("Resolution"))
            {
                string[] parts = trimmed.Split('=');
                if (parts.Length > 1) float.TryParse(parts[1].Trim(), out chartResolution);
            }
            if (inSync) syncTrackLines.Add(trimmed);
            if (inExpert) expertNotesLines.Add(trimmed);
        }

        if (chartResolution == 0) chartResolution = 192f;

        float currentBPM = 120f;
        float lastTick = 0f;
        float accumulatedTime = 0f;
        bpmHistory.Add(new BPMChange { tick = 0, timeInSeconds = 0, bpm = currentBPM });

        foreach (string line in syncTrackLines)
        {
            if (line.Contains("= B"))
            {
                try
                {
                    string[] mainParts = line.Split('=');
                    float tick = float.Parse(mainParts[0].Trim());
                    string[] bpmParts = mainParts[1].Trim().Split(' ');
                    float rawBpm = float.Parse(bpmParts[1].Trim());
                    float newBPM = rawBpm / 1000f;

                    float deltaTicks = tick - lastTick;
                    accumulatedTime += (deltaTicks / chartResolution) * (60f / currentBPM);

                    bpmHistory.Add(new BPMChange { tick = tick, timeInSeconds = accumulatedTime, bpm = newBPM });
                    currentBPM = newBPM; lastTick = tick;
                }
                catch { continue; }
            }
        }

        foreach (string line in expertNotesLines)
        {
            if (line.Contains("= N"))
            {
                try
                {
                    string[] mainParts = line.Split('=');
                    float noteTick = float.Parse(mainParts[0].Trim());
                    string[] noteParts = mainParts[1].Trim().Split(' ');
                    int lane = int.Parse(noteParts[1].Trim());
                    float durationTicks = float.Parse(noteParts[2].Trim());

                    if (lane >= 0 && lane <= 3)
                    {
                        BPMChange activeBPM = bpmHistory[0];
                        for (int i = bpmHistory.Count - 1; i >= 0; i--)
                        {
                            if (bpmHistory[i].tick <= noteTick)
                            {
                                activeBPM = bpmHistory[i]; break;
                            }
                        }

                        float ticksFromBPMChange = noteTick - activeBPM.tick;
                        float noteTimeInSeconds = activeBPM.timeInSeconds + ((ticksFromBPMChange / chartResolution) * (60f / activeBPM.bpm));

                        FNFNoteData newNote = new FNFNoteData()
                        {
                            hitTime = noteTimeInSeconds,
                            lane = lane,
                            duration = (durationTicks / chartResolution) * (60f / activeBPM.bpm),
                            isBot = false
                        };
                        sequencedNotes.Add(newNote);
                    }
                }
                catch { continue; }
            }
        }
        sequencedNotes.Sort((x, y) => x.hitTime.CompareTo(y.hitTime));
    }

    private void SpawnFNFNote(FNFNoteData data)
    {
        CreateNoteInstance(data, playerReceptors[data.lane], false);
        CreateNoteInstance(data, botReceptors[data.lane], true);
    }

    private void CreateNoteInstance(FNFNoteData data, Transform receptor, bool assignmentToBot)
    {
        Vector3 spawnPosition = receptor.position + new Vector3(0, spawnAheadTime * speed, 0);
        GameObject spawnedObj = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        
        // Atur Gambar KEPALA Panah
        SpriteRenderer sr = spawnedObj.GetComponent<SpriteRenderer>();
        if (sr != null && noteSprites.Length == 4)
        {
            sr.sprite = noteSprites[data.lane];
        }

        FNFNoteController controller = spawnedObj.GetComponent<FNFNoteController>();
        if (controller != null)
        {
            // --- LOGIKA BARU: Atur Gambar EKOR Panah (Jika Ada) ---
            if (controller.holdTail != null && holdSprites.Length == 4)
            {
                SpriteRenderer tailSR = controller.holdTail.GetComponent<SpriteRenderer>();
                if (tailSR != null && holdSprites[data.lane] != null)
                {
                    tailSR.sprite = holdSprites[data.lane];
                }
            }

            FNFNoteData runtimeData = new FNFNoteData() { hitTime = data.hitTime, lane = data.lane, duration = data.duration, isBot = assignmentToBot };
            controller.Setup(runtimeData, receptor, speed);
        }
    }
}