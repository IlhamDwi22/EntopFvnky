using UnityEngine;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Slot Setup")]
    public int slotNumber;
    public TextMeshProUGUI textInformasiSlot;

    [Header("UI Button Reference (Opsional)")]
    [Tooltip("Tarik tombol Load Anda ke sini agar otomatis mati saat slot kosong")]
    public UnityEngine.UI.Button tombolLoad;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (SaveManager.Instance == null) return;

        bool hasSave = SaveManager.Instance.HasSaveFile(slotNumber);

        // Tombol Load dinonaktifkan jika slot kosong agar tidak bisa diklik
        if (tombolLoad != null)
        {
            tombolLoad.interactable = hasSave;
        }

        if (!hasSave)
        {
            if (textInformasiSlot != null)
            {
                textInformasiSlot.text = $"Slot {slotNumber}\n< KOSONG >";
            }
            return;
        }

        string path = SaveManager.Instance.GetSavePath(slotNumber);
        try
        {
            string json = System.IO.File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (textInformasiSlot != null)
            {
                textInformasiSlot.text = $"Slot {slotNumber}\n{data.namaScene} - {data.waktuSimpan}";
            }
        }
        catch
        {
            if (textInformasiSlot != null)
            {
                textInformasiSlot.text = $"Slot {slotNumber}\n[ Corrupted ]";
            }
        }
    }

    // Dipanggil oleh tombol UI "SAVE" di dalam slot ini
    public void ClickSave()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame(slotNumber);
            RefreshUI(); // Segarkan tampilan slot
        }
    }

    // Dipanggil oleh tombol UI "LOAD" di dalam slot ini
    public void ClickLoad()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSaveFile(slotNumber))
        {
            SaveManager.Instance.LoadGame(slotNumber);
        }
    }
}
