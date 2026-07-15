using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string namaScene;
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public bool adaPosisiTersimpan;
    
    public List<string> daftarBarangQuest;
    public List<RuntimeItemSaveData> databaseRuntime;
    public List<string> daftarPetiTerbuka;
    public List<string> daftarPintuTerbuka;

    public string waktuSimpan;
    public string passwordPetiRahasia;

    // Memori Puzzle
    public string puzzle_sceneAsal;
    public string puzzle_idBarangWajib;
    public string puzzle_idPintuGlobal;
    public string puzzle_sceneTujuan;
    public string puzzle_idPintuTujuan;
}

[System.Serializable]
public class RuntimeItemSaveData
{
    public string id;
    public string isiTeks;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private bool isPendingLoad = false;
    private Vector3 savedPlayerPosition;
    private List<RuntimeItemSaveData> loadedRuntimeItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Tetap hidup lintas scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // Quick Save (F5) & Quick Load (F9) menggunakan Slot 0
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame(0);
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadGame(0);
        }
    }

    public string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"savegame_slot_{slot}.json");
    }

    public bool HasSaveFile(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    private Transform DapatkanPlayerTransform(out Rigidbody2D rb)
    {
        rb = null;

        // Coba cari PlayerOverworld
        PlayerOverworld playerOverworld = FindAnyObjectByType<PlayerOverworld>();
        if (playerOverworld != null)
        {
            rb = playerOverworld.GetComponent<Rigidbody2D>();
            return playerOverworld.transform;
        }

        // Coba cari PlayerTopDown jika PlayerOverworld tidak ada
        PlayerTopDown playerTopDown = FindAnyObjectByType<PlayerTopDown>();
        if (playerTopDown != null)
        {
            rb = playerTopDown.GetComponent<Rigidbody2D>();
            return playerTopDown.transform;
        }

        return null;
    }

    public void SaveGame(int slot)
    {
        Rigidbody2D rb;
        Transform playerTr = DapatkanPlayerTransform(out rb);
        if (playerTr == null)
        {
            Debug.LogWarning("[SaveManager] Gagal Menyimpan! PlayerOverworld maupun PlayerTopDown tidak ditemukan.");
            return;
        }

        SaveData data = new SaveData();
        data.namaScene = SceneManager.GetActiveScene().name;
        data.playerPosX = playerTr.position.x;
        data.playerPosY = playerTr.position.y;
        data.playerPosZ = playerTr.position.z;
        data.adaPosisiTersimpan = GlobalBattleState.adaPosisiTersimpan;
        data.waktuSimpan = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        data.daftarBarangQuest = new List<string>(GlobalBattleState.daftarBarangQuest);
        data.daftarPetiTerbuka = new List<string>(GlobalBattleState.daftarPetiTerbuka);
        data.daftarPintuTerbuka = new List<string>(GlobalBattleState.daftarPintuTerbuka);

        data.databaseRuntime = new List<RuntimeItemSaveData>();
        foreach (var item in GlobalBattleState.databaseRuntime)
        {
            RuntimeItemSaveData runtimeSave = new RuntimeItemSaveData();
            runtimeSave.id = item.id;
            runtimeSave.isiTeks = item.isiTeks;
            data.databaseRuntime.Add(runtimeSave);
        }

        data.puzzle_sceneAsal = GlobalBattleState.puzzle_sceneAsal;
        data.puzzle_idBarangWajib = GlobalBattleState.puzzle_idBarangWajib;
        data.puzzle_idPintuGlobal = GlobalBattleState.puzzle_idPintuGlobal;
        data.puzzle_sceneTujuan = GlobalBattleState.puzzle_sceneTujuan;
        data.puzzle_idPintuTujuan = GlobalBattleState.puzzle_idPintuTujuan;
        data.passwordPetiRahasia = PetiPassword.passwordRahasiaSaatIni;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);

        Debug.Log($"[SaveManager] Slot {slot} Berhasil Disimpan ke: {GetSavePath(slot)}");
    }

    public void LoadGame(int slot)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveManager] Gagal Memuat! File save slot {slot} tidak ada.");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        GlobalBattleState.adaPosisiTersimpan = data.adaPosisiTersimpan;
        GlobalBattleState.posisiPlayerTerakhir = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
        GlobalBattleState.daftarBarangQuest = new List<string>(data.daftarBarangQuest);
        GlobalBattleState.daftarPetiTerbuka = new List<string>(data.daftarPetiTerbuka);
        GlobalBattleState.daftarPintuTerbuka = new List<string>(data.daftarPintuTerbuka);

        GlobalBattleState.puzzle_sceneAsal = data.puzzle_sceneAsal;
        GlobalBattleState.puzzle_idBarangWajib = data.puzzle_idBarangWajib;
        GlobalBattleState.puzzle_idPintuGlobal = data.puzzle_idPintuGlobal;
        GlobalBattleState.puzzle_sceneTujuan = data.puzzle_sceneTujuan;
        GlobalBattleState.puzzle_idPintuTujuan = data.puzzle_idPintuTujuan;
        PetiPassword.passwordRahasiaSaatIni = data.passwordPetiRahasia;

        loadedRuntimeItems = data.databaseRuntime;
        savedPlayerPosition = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
        isPendingLoad = true;

        SceneManager.LoadScene(data.namaScene);

        Debug.Log($"[SaveManager] Slot {slot} Berhasil Dimuat!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isPendingLoad)
        {
            isPendingLoad = false;

            // Reset waktu agar game berjalan kembali setelah di-load dari kondisi pause
            Time.timeScale = 1f;

            // Rekonstruksi database runtime item (memulihkan sprite dari ItemDatabase di scene baru)
            if (loadedRuntimeItems != null)
            {
                GlobalBattleState.databaseRuntime.Clear();
                ItemDatabase db = (InventoryManager.Instance != null) ? InventoryManager.Instance.itemDatabase : null;

                foreach (var itemSave in loadedRuntimeItems)
                {
                    Sprite spriteIkon = null;
                    if (db != null && db.allItems != null)
                    {
                        foreach (var itemData in db.allItems)
                        {
                            if (itemData != null && itemData.itemID == itemSave.id)
                            {
                                spriteIkon = itemData.ikonBarang;
                                break;
                            }
                        }
                    }

                    // Fallback: Jika tidak ditemukan di ItemDatabase, cari dari KertasPetunjuk yang ada di scene
                    if (spriteIkon == null)
                    {
                        KertasPetunjuk[] semuaKertas = FindObjectsByType<KertasPetunjuk>();
                        foreach (var kertas in semuaKertas)
                        {
                            if (kertas != null)
                            {
                                string kertasId = string.IsNullOrEmpty(kertas.idKertas) ? kertas.gameObject.name : kertas.idKertas;
                                if (kertasId == itemSave.id)
                                {
                                    SpriteRenderer sr = kertas.GetComponent<SpriteRenderer>();
                                    if (sr != null) spriteIkon = sr.sprite;
                                    break;
                                }
                            }
                        }
                    }

                    GlobalBattleState.TambahItemRuntime(itemSave.id, spriteIkon, itemSave.isiTeks);
                }
                loadedRuntimeItems = null; // Bersihkan memori temp setelah berhasil dimuat
            }

            Rigidbody2D rb;
            Transform playerTr = DapatkanPlayerTransform(out rb);
            if (playerTr != null)
            {
                playerTr.position = savedPlayerPosition;
                if (rb != null) rb.linearVelocity = Vector2.zero;

                // Pastikan script input player dinyalakan kembali
                PlayerOverworld po = playerTr.GetComponent<PlayerOverworld>();
                if (po != null) po.enabled = true;

                PlayerTopDown ptd = playerTr.GetComponent<PlayerTopDown>();
                if (ptd != null) ptd.enabled = true;
            }

            // Reset status PauseManager jika terpasang di scene baru
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.isPaused = false;
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.RefreshInventoryUI();
            }
        }
    }
}
