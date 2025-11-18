using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton
    public static UIManager Instance { get; private set; }

    [Header("UI Component References")]
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] Iteminfo itemInfo;
    [SerializeField] ResourceUI resourceUI;
    [SerializeField] EquipmentUI equipmentUI;

    public ItemSlot CurrSlot { get; private set; } = null;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectSlot(ItemSlot slotUI)
    {
        // 이전 슬롯 선택 해제
        if (CurrSlot != null) CurrSlot.SetSelectState(false);

        // 새로운 슬롯
        CurrSlot = slotUI;
        if (slotUI != null) slotUI.SetSelectState(true);

        if (slotUI == null)
        {
            ShowItemInfo(null);
            return;
        }
        ShowItemInfo(slotUI._item);
    }

    public void ShowItemInfo(ItemInstance item)
    {
        itemInfo.UpdateInfo(item);
    }

    public void DeselectAll()
    {
        SelectSlot(null);
    }

    public void RefreshAllUI()
    {
        if (inventoryUI != null) inventoryUI.UpdateUI();
        if (equipmentUI != null) equipmentUI.UpdateEquipmentUI();
        if (resourceUI != null) resourceUI.UpdateUI();
    }
}