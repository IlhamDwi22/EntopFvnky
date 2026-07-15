using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

public class FNF2PConductor : MonoBehaviour
{
    public static FNF2PConductor Instance;

    [Header("Komponen Wajib")]
    public AudioSource musicSource;
    public GameObject notePrefab;
    public Transform[] player1Receptors; 
    public Transform[] player2Receptors;    
    public FNF2PScoring scoringSystem;

    [Header("Visualisasi Karakter")]
    public SpriteRenderer p1Visual;
    public Animator p1Anim;
    public SpriteRenderer p2Visual;
    public Animator p2Anim;
    
    [Tooltip("Masukkan 4 gambar KEPALA panah: 0=Kiri, 1=Bawah, 2=Atas, 3=Kanan")]
    public Sprite[] noteSprites = new Sprite[4];

    [Tooltip("Masukkan 4 gambar EKOR panah (Hold): 0=Kiri, 1=Bawah, 2=Atas, 3=Kanan")]
    public Sprite[] holdSprites = new Sprite[4];

    [Header("Pengaturan Jarak Spawn")]
    public float spawnAheadTime = 3f;

    [HideInInspector] public float speed = 5f; 
    [HideInInspector] public float audioOffsetAdjustment = 0f;

    public float currentSongTime { get; private set; }
    private bool isSongPlaying = false;
    private bool hasStartedPlaying = false;
    private bool battleEnded = false; 
    
    private List<FNF2PNoteData> sequencedNotes = new List<FNF2PNoteData>();
    private int nextNoteIndex = 0;
    private float introTimer = 0f;
    private float songDelay = 1f;

    private struct BPMChange { public float tick; public float timeInSeconds; public float bpm; }
    private List<BPMChange> bpmHistory = new List<BPMChange>();
    private bool wasMusicPlayingBeforePause = false;

    private void Awake() 
    { 
        Instance = this; 
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // 1. Terapkan Karakter P1 dan P2
        if (Global2PState.p1Karakter != null)
        {
            if (p1Visual != null) p1Visual.sprite = Global2PState.p1Karakter.visualMusuh;
            if (p1Anim != null) p1Anim.runtimeAnimatorController = Global2PState.p1Karakter.animasiMusuh;
        }
        if (Global2PState.p2Karakter != null)
        {
            if (p2Visual != null) p2Visual.sprite = Global2PState.p2Karakter.visualMusuh;
            if (p2Anim != null) p2Anim.runtimeAnimatorController = Global2PState.p2Karakter.animasiMusuh;
        }

        // 2. Load Lagu Terpilih
        if (Global2PState.laguTerpilih != null)
        {
            Song2PData lagu = Global2PState.laguTerpilih;
            speed = lagu.scrollSpeed;
            audioOffsetAdjustment = lagu.audioOffset;
            songDelay = lagu.delayMulai;

            if (lagu.isCustom)
            {
                // Muat secara dinamis dari folder eksternal
                if (File.Exists(lagu.chartPath))
                {
                    string chartContent = File.ReadAllText(lagu.chartPath);
                    ParseChartDinamis(chartContent);
                }
                
                StartCoroutine(MuatLaguCustom(lagu.audioPath));
            }
            else if (lagu.songDataBawaan != null)
            {
                // Muat dari aset bawaan game
                musicSource.clip = lagu.songDataBawaan.fileAudio;
                ParseChartDinamis(lagu.songDataBawaan.fileChart.text);
                MulaiLaguTerpilih();
            }
        }
        else
        {
            Debug.LogWarning("[FNF2PConductor] Tidak ada lagu terpilih di Global2PState!");
        }
    }

    private IEnumerator MuatLaguCustom(string audioPath)
    {
        string url = "file:///" + audioPath.Replace("\\", "/");
        
        AudioType type = AudioType.MPEG;
        if (audioPath.EndsWith(".ogg", System.StringComparison.OrdinalIgnoreCase))
        {
            type = AudioType.OGGVORBIS;
        }
        else if (audioPath.EndsWith(".wav", System.StringComparison.OrdinalIgnoreCase))
        {
            type = AudioType.WAV;
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, type))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                musicSource.clip = DownloadHandlerAudioClip.GetContent(www);
                MulaiLaguTerpilih();
            }
            else
            {
                Debug.LogError("[FNF2PConductor] Gagal memuat audio custom: " + www.error);
            }
        }
    }

    private void MulaiLaguTerpilih()
    {
        introTimer = -songDelay;
        currentSongTime = introTimer;
        isSongPlaying = true;
    }

    private void Update()
    {
        // Penanganan Jeda (Pause)
        if (Time.timeScale == 0f)
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Pause();
                wasMusicPlayingBeforePause = true;
            }
            return;
        }
        else
        {
            if (wasMusicPlayingBeforePause && musicSource != null)
            {
                musicSource.UnPause();
                wasMusicPlayingBeforePause = false;
            }
        }

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
                    // Fail-safe: Jika audio tidak berputar (atau clip kosong), jalankan waktu dengan Time.deltaTime
                    currentSongTime += Time.deltaTime;

                    if (musicSource.clip != null)
                    {
                        if (currentSongTime >= musicSource.clip.length - 0.5f)
                        {
                            EndBattle();
                        }
                    }
                    else if (sequencedNotes.Count > 0 && currentSongTime >= sequencedNotes[sequencedNotes.Count - 1].hitTime + 2f)
                    {
                        EndBattle(); // Berakhir 2 detik setelah note terakhir jika tidak ada audio
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
        
        // Pemicu akhir tanding, serahkan hasil ke scoring system untuk menampilkan UI pemenang
        if (scoringSystem != null)
        {
            scoringSystem.TampilkanLayarPemenang();
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

                        FNF2PNoteData newNote = new FNF2PNoteData()
                        {
                            hitTime = noteTimeInSeconds,
                            lane = lane,
                            duration = (durationTicks / chartResolution) * (60f / activeBPM.bpm),
                            isPlayer2 = false
                        };
                        sequencedNotes.Add(newNote);
                    }
                }
                catch { continue; }
            }
        }
        sequencedNotes.Sort((x, y) => x.hitTime.CompareTo(y.hitTime));
    }

    private void SpawnFNFNote(FNF2PNoteData data)
    {
        // Spawn untuk Player 1 (Sisi Kiri)
        CreateNoteInstance(data, player1Receptors[data.lane], false);
        // Spawn untuk Player 2 (Sisi Kanan)
        CreateNoteInstance(data, player2Receptors[data.lane], true);
    }

    private void CreateNoteInstance(FNF2PNoteData data, Transform receptor, bool isP2)
    {
        Vector3 spawnPosition = receptor.position + new Vector3(0, spawnAheadTime * speed, 0);
        GameObject spawnedObj = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        
        FNF2PNoteController noteCtrl = spawnedObj.GetComponent<FNF2PNoteController>();
        if (noteCtrl != null)
        {
            noteCtrl.Setup(new FNF2PNoteData
            {
                hitTime = data.hitTime,
                lane = data.lane,
                duration = data.duration,
                isPlayer2 = isP2
            }, receptor, speed);
        }
    }
}

public class FNF2PNoteData
{
    public float hitTime;
    public int lane;
    public float duration;
    public bool isPlayer2;
}
