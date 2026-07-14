using UnityEngine;

public class SavePanelUI : MonoBehaviour
{
    private SaveSlotUI[] allSlots;

    private void Awake()
    {
        allSlots = GetComponentsInChildren<SaveSlotUI>(true);
    }

    private void OnEnable()
    {
        RefreshAllSlots();
    }

    public void RefreshAllSlots()
    {
        if (allSlots == null) return;
        foreach (var slot in allSlots)
        {
            if (slot != null)
            {
                slot.RefreshUI();
            }
        }
    }
}
