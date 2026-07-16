using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Menu2PManager : MonoBehaviour
{
    [Header("List Karakter & Map (Drag & Drop di Inspector)")]
    public List<OpponentData> daftarKarakter = new List<OpponentData>();
    public List<string> daftarMap = new List<string> { "Jalanan Kota (Level 3)", "Panggung KROWN (Phase 1)", "Panggung KROWN (Phase 2)" };
    
    [Header("Pengaturan Scene")]
    public string namaSceneBattle = "Battle2PlayerScene";

    [Header("Sub-Panel Navigasi (Satu Tempat Bergantian)")]
    public GameObject subPanelKarakter;
    public GameObject subPanelLagu;
    public GameObject subPanelMap;

    [Header("Visual Preview Karakter")]
    public Image imagePreviewP1;
    public Image imagePreviewP2;
    public TextMeshProUGUI textNamaP1;
    public TextMeshProUGUI textNamaP2;

    [Header("UI List - Lagu (Scroll View)")]
    public Transform songListContainer;
    public GameObject songButtonPrefab;

    [Header("UI List - Map (Scroll View)")]
    public Transform mapListContainer;
    public GameObject mapButtonPrefab;

    [Header("UI - Detail & Config Panel")]
    public TextMeshProUGUI textDetailLagu;
    public TMP_InputField inputSpeed;
    public TMP_InputField inputOffset;
    public TMP_InputField inputDelay;

    [Header("UI - Folder Custom Songs")]
    public TextMeshProUGUI folderPathText; // Menampilkan path folder aktif saat ini

    private List<Song2PData> semuaLagu = new List<Song2PData>();
    private Song2PData laguAktif;
    private int mapTerpilihIndex = 0;
    private string customSongsDir;

    private int currentP1CharIndex = 0;
    private int currentP2CharIndex = 1;

    private void Start()
    {
        // Muat path custom songs folder dari PlayerPrefs (default ke Application.persistentDataPath/CustomSongs)
        string defaultPath = Path.Combine(Application.persistentDataPath, "CustomSongs");
        customSongsDir = PlayerPrefs.GetString("CustomSongsFolder", defaultPath);

        if (!Directory.Exists(customSongsDir))
        {
            Directory.CreateDirectory(customSongsDir);
        }

        if (folderPathText != null)
        {
            folderPathText.text = customSongsDir;
        }

        // Tampilkan sub-panel karakter sebagai tahap pertama
        if (subPanelKarakter != null) subPanelKarakter.SetActive(true);
        if (subPanelLagu != null) subPanelLagu.SetActive(false);
        if (subPanelMap != null) subPanelMap.SetActive(false);

        // Pasang karakter default di awal agar preview tidak kosong
        if (daftarKarakter.Count > 0) PilihKarakterP1(0);
        if (daftarKarakter.Count > 1) PilihKarakterP2(1);

        SetupMapList();
        RefreshSongList();
    }

    // --- FITUR PILIH KARAKTER DENGAN INDEX CYCLING (PREV / NEXT) ---
    public void GeserKarakterP1(int arah)
    {
        if (daftarKarakter.Count == 0) return;
        currentP1CharIndex += arah;

        // Balik lagi (wrap around) jika mentok
        if (currentP1CharIndex >= daftarKarakter.Count) currentP1CharIndex = 0;
        if (currentP1CharIndex < 0) currentP1CharIndex = daftarKarakter.Count - 1;

        PilihKarakterP1(currentP1CharIndex);
    }

    public void GeserKarakterP2(int arah)
    {
        if (daftarKarakter.Count == 0) return;
        currentP2CharIndex += arah;

        // Balik lagi (wrap around) jika mentok
        if (currentP2CharIndex >= daftarKarakter.Count) currentP2CharIndex = 0;
        if (currentP2CharIndex < 0) currentP2CharIndex = daftarKarakter.Count - 1;

        PilihKarakterP2(currentP2CharIndex);
    }

    public void PilihKarakterP1(int index)
    {
        if (index < 0 || index >= daftarKarakter.Count) return;
        currentP1CharIndex = index;
        Global2PState.p1Karakter = daftarKarakter[index];

        if (imagePreviewP1 != null) imagePreviewP1.sprite = Global2PState.p1Karakter.visualMusuh;
        if (textNamaP1 != null) textNamaP1.text = Global2PState.p1Karakter.namaMusuh;
    }

    public void PilihKarakterP2(int index)
    {
        if (index < 0 || index >= daftarKarakter.Count) return;
        currentP2CharIndex = index;
        Global2PState.p2Karakter = daftarKarakter[index];

        if (imagePreviewP2 != null) imagePreviewP2.sprite = Global2PState.p2Karakter.visualMusuh;
        if (textNamaP2 != null) textNamaP2.text = Global2PState.p2Karakter.namaMusuh;
    }

    // --- SISTEM TAHAPAN PANEL ---
    public void KePanelLagu()
    {
        if (subPanelKarakter != null) subPanelKarakter.SetActive(false);
        if (subPanelLagu != null) subPanelLagu.SetActive(true);
    }

    public void KembaliKeKarakter()
    {
        if (subPanelLagu != null) subPanelLagu.SetActive(false);
        if (subPanelKarakter != null) subPanelKarakter.SetActive(true);
    }

    public void KePanelMap()
    {
        if (subPanelLagu != null) subPanelLagu.SetActive(false);
        if (subPanelMap != null) subPanelMap.SetActive(true);
    }

    public void KembaliKeLagu()
    {
        if (subPanelMap != null) subPanelMap.SetActive(false);
        if (subPanelLagu != null) subPanelLagu.SetActive(true);
    }

    // --- FITUR ATUR FOLDER & TAMBAH LAGU CUSTOM VIA NATIVE EXPLORER ---
    public void SetCustomSongsFolder()
    {
        string selectedPath = FolderBrowserWin32.OpenFolderDialog("Pilih Folder Tempat Menyimpan Lagu Custom");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            customSongsDir = selectedPath;
            PlayerPrefs.SetString("CustomSongsFolder", selectedPath);
            PlayerPrefs.Save();

            if (folderPathText != null)
            {
                folderPathText.text = selectedPath;
            }

            RefreshSongList();
        }
    }

    public void BukaFolderLaguCustom()
    {
        if (Directory.Exists(customSongsDir))
        {
            System.Diagnostics.Process.Start("explorer.exe", customSongsDir.Replace("/", "\\"));
        }
        else
        {
            Debug.LogWarning("[Menu2PManager] Folder tidak ditemukan: " + customSongsDir);
        }
    }

    // --- STRUKTUR LIST MAP (DENGAN TOMBOL) ---
    private void SetupMapList()
    {
        if (mapListContainer == null || mapButtonPrefab == null) return;

        // Bersihkan tombol lama
        foreach (Transform child in mapListContainer)
        {
            Destroy(child.gameObject);
        }

        // Generate tombol map bawaan
        for (int i = 0; i < daftarMap.Count; i++)
        {
            int idx = i;
            GameObject btnObj = Instantiate(mapButtonPrefab, mapListContainer);
            
            TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = daftarMap[i];

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnMapSelected(idx));
            }
        }

        // Tambah tombol Acak (Random Map) di akhir
        int randomIdx = daftarMap.Count;
        GameObject randBtnObj = Instantiate(mapButtonPrefab, mapListContainer);
        
        TextMeshProUGUI randTxt = randBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (randTxt != null) randTxt.text = "Acak (Random Map)";

        Button randBtn = randBtnObj.GetComponent<Button>();
        if (randBtn != null)
        {
            randBtn.onClick.AddListener(() => OnMapSelected(randomIdx));
        }
    }

    public void OnMapSelected(int index)
    {
        mapTerpilihIndex = index;
        
        if (index == daftarMap.Count)
        {
            Debug.Log("[Menu2PManager] Map Terpilih: Acak (Random)");
        }
        else
        {
            Debug.Log($"[Menu2PManager] Map Terpilih: {daftarMap[index]}");
        }
    }

    // --- STRUKTUR LIST LAGU (DENGAN TOMBOL) ---
    public void RefreshSongList()
    {
        semuaLagu.Clear();

        // 1. Ambil Lagu Bawaan
        HashSet<string> laguTercatat = new HashSet<string>();
        foreach (var charData in daftarKarakter)
        {
            if (charData != null && charData.daftarLagu != null)
            {
                foreach (var song in charData.daftarLagu)
                {
                    if (song != null && !laguTercatat.Contains(song.judulLagu))
                    {
                        laguTercatat.Add(song.judulLagu);
                        
                        Song2PData s = new Song2PData();
                        s.judulLagu = song.judulLagu;
                        s.isCustom = false;
                        s.songDataBawaan = song;
                        
                        s.scrollSpeed = PlayerPrefs.GetFloat("SongSpeed_" + song.judulLagu, song.kecepatanScroll);
                        s.audioOffset = PlayerPrefs.GetFloat("SongOffset_" + song.judulLagu, song.penyesuaianOffset);
                        s.delayMulai = PlayerPrefs.GetFloat("SongDelay_" + song.judulLagu, song.delayMulai);

                        semuaLagu.Add(s);
                    }
                }
            }
        }

        // 2. Ambil Lagu Custom
        if (Directory.Exists(customSongsDir))
        {
            string[] dirs = Directory.GetDirectories(customSongsDir);
            foreach (string dir in dirs)
            {
                string folderName = Path.GetFileName(dir);
                
                string audioFile = "";
                string chartFile = "";

                try
                {
                    string[] allFiles = Directory.GetFiles(dir);
                    foreach (string file in allFiles)
                    {
                        string fileNameLower = Path.GetFileName(file).ToLower();
                        
                        // Abaikan file metadata Unity
                        if (fileNameLower.EndsWith(".meta")) continue;

                        Debug.Log($"[Menu2PManager] Memindai file: {fileNameLower} di folder {folderName}");

                        if (fileNameLower.Contains(".mp3") || fileNameLower.Contains(".ogg") || fileNameLower.Contains(".wav"))
                        {
                            if (string.IsNullOrEmpty(audioFile)) audioFile = file;
                        }
                        else if (fileNameLower.Contains(".chart") || fileNameLower.Contains(".txt"))
                        {
                            if (string.IsNullOrEmpty(chartFile)) chartFile = file;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[Menu2PManager] Gagal membaca folder lagu {folderName}: {e.Message}");
                }

                if (!string.IsNullOrEmpty(audioFile) && !string.IsNullOrEmpty(chartFile))
                {
                    Song2PData s = new Song2PData();
                    s.judulLagu = folderName + " [Custom]";
                    s.isCustom = true;
                    s.folderPath = dir;
                    s.audioPath = audioFile;
                    s.chartPath = chartFile;

                    string configPath = Path.Combine(dir, "config.json");
                    if (File.Exists(configPath))
                    {
                        try
                        {
                            string json = File.ReadAllText(configPath);
                            SongConfigData config = JsonUtility.FromJson<SongConfigData>(json);
                            s.scrollSpeed = config.scrollSpeed;
                            s.audioOffset = config.audioOffset;
                            s.delayMulai = config.delayMulai;
                        }
                        catch
                        {
                            s.scrollSpeed = 5f;
                            s.audioOffset = 0f;
                            s.delayMulai = 1f;
                        }
                    }
                    else
                    {
                        s.scrollSpeed = 5f;
                        s.audioOffset = 0f;
                        s.delayMulai = 1f;
                    }

                    semuaLagu.Add(s);
                    Debug.Log($"[Menu2PManager] Berhasil mendeteksi lagu custom: {s.judulLagu}");
                }
                else
                {
                    Debug.LogWarning($"[Menu2PManager] Melewati folder {folderName}. File Audio: {(string.IsNullOrEmpty(audioFile) ? "TIDAK DITEMUKAN" : Path.GetFileName(audioFile))}, File Chart: {(string.IsNullOrEmpty(chartFile) ? "TIDAK DITEMUKAN" : Path.GetFileName(chartFile))}");
                }
            }
        }

        // 3. Buat Tombol untuk Masing-Masing Lagu
        if (songListContainer != null && songButtonPrefab != null)
        {
            foreach (Transform child in songListContainer)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < semuaLagu.Count; i++)
            {
                int idx = i;
                GameObject btnObj = Instantiate(songButtonPrefab, songListContainer);
                
                TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = semuaLagu[i].judulLagu;

                Button btn = btnObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OnSongSelected(idx));
                }
            }

            if (semuaLagu.Count > 0)
            {
                OnSongSelected(0);
            }
        }
    }

    public void OnSongSelected(int index)
    {
        if (index < 0 || index >= semuaLagu.Count) return;
        laguAktif = semuaLagu[index];

        if (textDetailLagu != null)
        {
            if (laguAktif.isCustom)
            {
                textDetailLagu.text = $"Lagu Custom: {laguAktif.judulLagu}\nFormat: {Path.GetExtension(laguAktif.audioPath).ToUpper()}";
            }
            else
            {
                textDetailLagu.text = $"Lagu Bawaan: {laguAktif.judulLagu}\nArtist: {laguAktif.songDataBawaan.namaArtis}";
            }
        }

        if (inputSpeed != null)
        {
            inputSpeed.text = laguAktif.scrollSpeed.ToString("F2");
        }
        if (inputOffset != null)
        {
            inputOffset.text = laguAktif.audioOffset.ToString("F3");
        }
        if (inputDelay != null)
        {
            inputDelay.text = laguAktif.delayMulai.ToString("F2");
        }
    }

    public void OnSpeedEndEdit(string text)
    {
        if (laguAktif == null) return;
        if (float.TryParse(text, out float val))
        {
            laguAktif.scrollSpeed = Mathf.Max(0.1f, val); // Scroll speed tidak boleh <= 0
            SimpanConfigLagu(laguAktif);
        }
        // Tampilkan kembali nilai aktual agar sinkron
        if (inputSpeed != null) inputSpeed.text = laguAktif.scrollSpeed.ToString("F2");
    }

    public void OnOffsetEndEdit(string text)
    {
        if (laguAktif == null) return;
        if (float.TryParse(text, out float val))
        {
            laguAktif.audioOffset = val;
            SimpanConfigLagu(laguAktif);
        }
        // Tampilkan kembali nilai aktual agar sinkron
        if (inputOffset != null) inputOffset.text = laguAktif.audioOffset.ToString("F3");
    }

    public void OnDelayEndEdit(string text)
    {
        if (laguAktif == null) return;
        if (float.TryParse(text, out float val))
        {
            laguAktif.delayMulai = Mathf.Max(0f, val); // Delay tidak boleh negatif
            SimpanConfigLagu(laguAktif);
        }
        // Tampilkan kembali nilai aktual agar sinkron
        if (inputDelay != null) inputDelay.text = laguAktif.delayMulai.ToString("F2");
    }

    private void SimpanConfigLagu(Song2PData song)
    {
        if (song.isCustom)
        {
            string configPath = Path.Combine(song.folderPath, "config.json");
            SongConfigData config = new SongConfigData();
            config.scrollSpeed = song.scrollSpeed;
            config.audioOffset = song.audioOffset;
            config.delayMulai = song.delayMulai;

            try
            {
                string json = JsonUtility.ToJson(config, true);
                File.WriteAllText(configPath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[Menu2PManager] Gagal menulis config.json: " + e.Message);
            }
        }
        else
        {
            PlayerPrefs.SetFloat("SongSpeed_" + song.judulLagu, song.scrollSpeed);
            PlayerPrefs.SetFloat("SongOffset_" + song.judulLagu, song.audioOffset);
            PlayerPrefs.SetFloat("SongDelay_" + song.judulLagu, song.delayMulai);
            PlayerPrefs.Save();
        }
    }

    public void StartBattle2Player()
    {
        if (laguAktif == null) return;

        // 1. Terapkan Map
        if (mapTerpilihIndex == daftarMap.Count) // Opsi Acak
        {
            Global2PState.pilihanMapIndex = -1; // -1 untuk Map Acak/Random
        }
        else
        {
            Global2PState.pilihanMapIndex = mapTerpilihIndex;
        }

        // 2. Terapkan Lagu Terpilih
        Global2PState.laguTerpilih = laguAktif;

        Time.timeScale = 1f;
        SceneManager.LoadScene(namaSceneBattle);
    }

    public void BackToMainMenu()
    {
        MenuManager mm = FindAnyObjectByType<MenuManager>();
        if (mm != null)
        {
            mm.Close2PlayerPanel();
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}

// --- HELPER NATIVE WIN32 FILE BROWSER DI WINDOWS ---
public class FolderBrowserWin32
{
    [DllImport("shell32.dll")]
    private static extern System.IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern bool SHGetPathFromIDList(System.IntPtr pidl, StringBuilder pszPath);

    [DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct BROWSEINFO
    {
        public System.IntPtr hwndOwner;
        public System.IntPtr pidlRoot;
        public string pszDisplayName;
        public string lpszTitle;
        public uint ulFlags;
        public System.IntPtr lpfn;
        public System.IntPtr lParam;
        public int iImage;
    }

    private const uint BIF_RETURNONLYFSDIRS = 0x00000001;
    private const uint BIF_NEWDIALOGSTYLE = 0x00000040;

    public static string OpenFolderDialog(string title = "Pilih Folder")
    {
        BROWSEINFO bi = new BROWSEINFO();
        bi.hwndOwner = GetActiveWindow();
        bi.pidlRoot = System.IntPtr.Zero;
        bi.pszDisplayName = new string('\0', 260);
        bi.lpszTitle = title;
        bi.ulFlags = BIF_RETURNONLYFSDIRS | BIF_NEWDIALOGSTYLE;
        bi.lpfn = System.IntPtr.Zero;
        bi.lParam = System.IntPtr.Zero;
        bi.iImage = 0;

        System.IntPtr pidl = SHBrowseForFolder(ref bi);
        if (pidl != System.IntPtr.Zero)
        {
            StringBuilder sb = new StringBuilder(260);
            if (SHGetPathFromIDList(pidl, sb))
            {
                return sb.ToString();
            }
        }
        return null;
    }
}
