using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI Inventory")]
    public GameObject inventoryPanel;
    public Transform slotContainer;
    public GameObject slotPrefab;

    [Header("Database Item")]
    [Tooltip("Masukkan Asset database ScriptableObject item Anda ke sini")]
    public ItemDatabase itemDatabase;

    [Header("UI Dokumen")]
    public GameObject panelKertas;
    public TextMeshProUGUI teksCatatan;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (panelKertas != null)
            panelKertas.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if(inventoryPanel == null)
            return;

        bool buka = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(buka);

        if(buka)
        {
            Time.timeScale = 0f;
            RefreshInventoryUI();
        }
        else
        {
            Time.timeScale = 1f;

            if(panelKertas != null)
                panelKertas.SetActive(false);
        }
    }

    public void RefreshInventoryUI()
    {
        foreach(Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if(GlobalBattleState.daftarBarangQuest == null)
            return;

        foreach(string id in GlobalBattleState.daftarBarangQuest)
        {
            ItemData data = CariDataBarang(id);

            if(data == null)
                continue;

            GameObject slot = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();

            if(slotUI != null)
            {
                slotUI.SetupSlot(
                    data.ikonBarang,
                    data.namaBarang,
                    data.itemID
                );
            }
        }
    }

    private ItemData CariDataBarang(string id)
    {
        // 1. Cari di database ScriptableObject
        if (itemDatabase != null && itemDatabase.allItems != null)
        {
            foreach(ItemData data in itemDatabase.allItems)
            {
                if(data != null && data.itemID == id)
                {
                    return data;
                }
            }
        }

        // 2. Cari di database runtime (kertas/clue yang diambil selama permainan)
        RuntimeItemData runtime = GlobalBattleState.CariRuntimeItem(id);
        if(runtime != null)
        {
            // Buat instance ItemData sementara untuk representasi di UI
            ItemData tempItem = ScriptableObject.CreateInstance<ItemData>();
            tempItem.itemID = runtime.id;
            tempItem.namaBarang = runtime.nama;
            tempItem.ikonBarang = runtime.sprite;
            tempItem.isDokumen = true;
            tempItem.isiTeks = runtime.isiTeks;
            return tempItem;
        }

        return null;
    }

    public void GunakanBarang(string id)
    {
        ItemData data = CariDataBarang(id);
        if (data == null) return;

        // 1. CEK DOKUMEN (Kertas): Dokumen selalu dibaca, tidak digesek!
        if (data.isDokumen)
        {
            BukaDokumen(data);
            return;
        }

        // 2. JIKA ADA MESIN GESEK DI SCENE INI (Untuk barang non-dokumen seperti Kartu Akses)
        if (MesinGesekKartu.Instance != null)
        {
            ToggleInventory(); // Ini yang membuat tas auto-tertutup!
            Sprite ikon = data.ikonBarang; 
            MesinGesekKartu.Instance.MunculkanBarangDiLayar(id, ikon);
            return; // Hentikan kode di sini agar tidak baca ke bawah
        }
    }

    // Fungsi pembantu untuk mengambil Sprite gambar dari tas
    private Sprite CariIkonBarang(string id)
    {
        ItemData data = CariDataBarang(id);
        if (data != null) return data.ikonBarang;
        return null;
    }

    private void BukaDokumen(ItemData data)
    {
        if(panelKertas == null || teksCatatan == null)
            return;

        panelKertas.SetActive(true);

        // Jika diset menampilkan password brankas secara dinamis
        if (data.menampilkanPasswordBrankas)
        {
            teksCatatan.text = "Sebuah catatan bernoda darah...\n\n\"Kombinasi brankas hari ini adalah: <color=red>" + PetiPassword.passwordRahasiaSaatIni + "</color>\"";
        }
        // Jika data item memiliki isi teks kustom, gunakan itu
        else if (!string.IsNullOrEmpty(data.isiTeks))
        {
            teksCatatan.text = data.isiTeks;
        }
        else
        {
            teksCatatan.text = "Dokumen : " + data.namaBarang + "\n\nTidak ada tulisan.";
        }
    }

    public void TutupPanelKertas()
    {
        if(panelKertas != null)
        {
            panelKertas.SetActive(false);
        }
    }
}
