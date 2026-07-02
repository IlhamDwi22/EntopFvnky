using UnityEngine;

[CreateAssetMenu(fileName = "ItemBaru", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identitas Item")]
    public string itemID;
    public string namaBarang;
    public Sprite ikonBarang;

    [Header("Pengaturan Dokumen")]
    public bool isDokumen = false;
    [Tooltip("Centang ini jika kertas ini khusus untuk menampilkan password acak brankas darah secara dinamis.")]
    public bool menampilkanPasswordBrankas = false;

    [TextArea(5, 10)]
    public string isiTeks;
}
