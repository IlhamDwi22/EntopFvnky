using UnityEngine;
using System.Collections.Generic;

public class FNFConductor : MonoBehaviour
{
    public static FNFConductor Instance;

    [Header("Audio Components")]
    public AudioSource musicSource;
    public float songDelay = 1f;
    public float speed = 5f; 

    [Header("Spawn Elements")]
    public GameObject notePrefab;
    public Transform[] playerReceptors; 
    public Transform[] botReceptors;    

    [Header("Chart Configuration")]
    public TextAsset chartTextAsset;
    [Tooltip("Gunakan jika lagu kurang pas keseluruhan. Nilai (+) menunda lagu, nilai (-) mempercepat.")]
    public float audioOffsetAdjustment = 0f;

    public float currentSongTime { get; private set; }
    private bool isSongPlaying = false;
    
    private List<FNFNoteData> sequencedNotes = new List<FNFNoteData>();
    private int nextNoteIndex = 0;
    public float spawnAheadTime = 1.5f; 

    public FNFScoring scoringSystem;
    private float introTimer = 0f;

    // Struktur data internal untuk mencatat riwayat perubahan BPM sepanjang lagu
    private struct BPMChange
    {
        public float tick;
        public float timeInSeconds;
        public float bpm;
    }
    private List<BPMChange> bpmHistory = new List<BPMChange>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (chartTextAsset == null)
        {
            Debug.LogError("[FNFConductor] File Chart Txt kosong di Inspector!");
            return;
        }

        // Ekstraksi data chart menggunakan algoritma multi-BPM tracking
        ParseChartDinamis(chartTextAsset.text);

        introTimer = -songDelay;
        currentSongTime = introTimer;
        isSongPlaying = true;
    }

    private void Update()
    {
        if (!isSongPlaying) return;

        if (musicSource != null && musicSource.isPlaying)
        {
            currentSongTime = musicSource.time + audioOffsetAdjustment;
        }
        else
        {
            introTimer += Time.deltaTime;
            currentSongTime = introTimer;

            if (introTimer >= 0 && musicSource != null && musicSource.clip != null && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }

        while (nextNoteIndex < sequencedNotes.Count && sequencedNotes[nextNoteIndex].hitTime - currentSongTime <= spawnAheadTime)
        {
            SpawnFNFNote(sequencedNotes[nextNoteIndex]);
            nextNoteIndex++;
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
        
        bool inSong = false;
        bool inSync = false;
        bool inExpert = false;

        // FASE 1: Memilah baris teks ke dalam kategori bagian masing-masing
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
                inSong = inSync = inExpert = false;
                continue;
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

        // FASE 2: Rekonstruksi garis waktu perubahan nilai BPM (BPM Timeline Mapping)
        float currentBPM = 120f;
        float lastTick = 0f;
        float accumulatedTime = 0f;

        // Masukkan titik awal default di detik ke-0
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

                    // Hitung akumulasi detik dari posisi BPM sebelumnya menuju ke titik perubahan baru ini
                    float deltaTicks = tick - lastTick;
                    accumulatedTime += (deltaTicks / chartResolution) * (60f / currentBPM);

                    bpmHistory.Add(new BPMChange { tick = tick, timeInSeconds = accumulatedTime, bpm = newBPM });
                    
                    currentBPM = newBPM;
                    lastTick = tick;
                }
                catch { continue; }
            }
        }

        // FASE 3: Konversi Koordinat Ticks Note Menjadi Detik Menggunakan Garis Waktu BPM History
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
                        // Cari baris perubahan BPM terakhir yang aktif sebelum atau tepat di koordinat note ini
                        BPMChange activeBPM = bpmHistory[0];
                        for (int i = bpmHistory.Count - 1; i >= 0; i--)
                        {
                            if (bpmHistory[i].tick <= noteTick)
                            {
                                activeBPM = bpmHistory[i];
                                break;
                            }
                        }

                        // Hitung posisi detik riil yang 100% pas dan akurat dengan video preview
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

        // Urutkan ulang daftar antrean
        sequencedNotes.Sort((x, y) => x.hitTime.CompareTo(y.hitTime));
        Debug.Log($"[FNFConductor] Sinkronisasi Sempurna! Sukses memetakan {sequencedNotes.Count} note mengikuti garis waktu perubahan BPM chart.");
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
        
        FNFNoteController controller = spawnedObj.GetComponent<FNFNoteController>();
        if (controller != null)
        {
            FNFNoteData runtimeData = new FNFNoteData()
            {
                hitTime = data.hitTime,
                lane = data.lane,
                duration = data.duration,
                isBot = assignmentToBot
            };
            controller.Setup(runtimeData, receptor, speed);
        }
    }
}