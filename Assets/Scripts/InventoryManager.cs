using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
public class DataBarang
{
    public string itemID;
    public string namaBarang;
    public Sprite ikonBarang;
}

    [Header("UI Inventory")]
    public GameObject inventoryPanel;
    public Transform slotContainer;
    public GameObject slotPrefab;


    [Header("Database Item Manual")]
    public List<DataBarang> databaseBarang;



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



        bool buka =
            !inventoryPanel.activeSelf;

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
            DataBarang data =
                CariDataBarang(id);

            if(data == null)
                continue;

            GameObject slot =
                Instantiate(
                    slotPrefab,
                    slotContainer
                );


            InventorySlotUI slotUI =
                slot.GetComponent<InventorySlotUI>();

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


    private DataBarang CariDataBarang(string id)
    {


        // Cari item manual
        foreach(DataBarang data in databaseBarang)
        {

            if(data.itemID == id)
            {
                return data;
            }

        }


        // Cari item otomatis (kertas)
        RuntimeItemData runtime =
            GlobalBattleState.CariRuntimeItem(id);

        if(runtime != null)
        {

            return new DataBarang()
            {
                itemID = runtime.id,
                namaBarang = runtime.nama,
                ikonBarang = runtime.sprite
            };

        }

        return null;

    }

    public void GunakanBarang(string id)
    {
        // 1. JIKA ADA MESIN GESEK DI SCENE INI
        if (MesinGesekKartu.Instance != null)
        {
            ToggleInventory(); // Ini yang membuat tas auto-tertutup!
            Sprite ikon = CariIkonBarang(id); 
            MesinGesekKartu.Instance.MunculkanBarangDiLayar(id, ikon);
            return; // Hentikan kode di sini agar tidak baca ke bawah
        }

        // 2. CEK DOKUMEN RUNTIME (Kertas)
        RuntimeItemData runtime = GlobalBattleState.CariRuntimeItem(id);
        if(runtime != null)
        {
            BukaDokumen(runtime);
            return;
        }

        // 3. CEK DOKUMEN MANUAL LAMA (Peti)
        if(id == "CatatanSandi")
        {
            if(panelKertas != null) panelKertas.SetActive(true);
            if(teksCatatan != null)
            {
                teksCatatan.text = "Sebuah catatan bernoda darah...\n\n\"Kombinasi brankas hari ini adalah: <color=red>" + PetiPassword.passwordRahasiaSaatIni + "</color>\"";
            }
            return;
        }
    }

    // Fungsi pembantu untuk mengambil Sprite gambar dari tas
    private Sprite CariIkonBarang(string id)
    {
        foreach(DataBarang data in databaseBarang) 
        {
            if(data.itemID == id) return data.ikonBarang;
        }
        RuntimeItemData runtime = GlobalBattleState.CariRuntimeItem(id);
        if(runtime != null) return runtime.sprite;
        return null;
    }


    private void BukaDokumen(RuntimeItemData data)
    {


        if(panelKertas == null ||
           teksCatatan == null)
            return;

        panelKertas.SetActive(true);


        // Default semua kertas
        teksCatatan.text =
            "Dokumen : " +
            data.nama +
            "\n\nTidak ada tulisan.";


        // Khusus password
        if(data.id == "Catatan Sandi")
        {

            teksCatatan.text =
            "Sebuah catatan bernoda darah...\n\n" +
            "\"Kombinasi brankas hari ini adalah: <color=red>" +
            PetiPassword.passwordRahasiaSaatIni +
            "</color>\"";

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
