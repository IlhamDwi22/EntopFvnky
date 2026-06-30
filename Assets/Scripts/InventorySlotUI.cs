using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventorySlotUI : MonoBehaviour
{
    [Header("Komponen UI Slot")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;

    private Button slotButton;

    private string myItemID;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
    }

    public void SetupSlot(Sprite icon, string itemName, string itemID)
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.enabled = true;
        }

        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }

        myItemID = itemID;

        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    private void OnSlotClicked()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.GunakanBarang(myItemID);
        }
    }
}