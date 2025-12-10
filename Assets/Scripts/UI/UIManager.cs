using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class UIManager : MonoBehaviour
{
    // Singleton
    public static UIManager Instance { get; private set; }

    [Header("UI Component References")]
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] Iteminfo itemInfo;
    [SerializeField] ResourceUI resourceUI;
    [SerializeField] EquipmentUI equipmentUI;
    [SerializeField] StatusUI statusUI;
    [SerializeField] UpgradeUI upgradeUI;

    [Header("Prefab")]
    public GameObject messageGroupPrefab; // 인스펙터에서 할당
    public Transform popupParent;         // 팝업이 생성될 Canvas

    [Header("Tier Colors")]
    [SerializeField] private List<Color> _tierColorList;
    public IReadOnlyList<Color> tierColorList => _tierColorList;

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

    private void Start()
    {
        Instance.RefreshAllUI();
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
        upgradeUI.UpdateInfo(item);
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
        if (statusUI != null) statusUI.UpdateUI();
    }

    public void RefreshStatusUI()
    {
        if (statusUI != null) statusUI.UpdateUI();
    }

    public void ShowPopup(string message, Action callback = null, string btnTxt = "확인", string title = "Message")
    {
        GameObject obj = Instantiate(messageGroupPrefab, popupParent);
        MessageUI popup = obj.GetComponent<MessageUI>();
        popup.Setup(message, callback, btnTxt, title);
    }

    public void ShowUpgradeUI()
    {
        if (upgradeUI != null)
        {
            upgradeUI.enable();
        }
    }
}