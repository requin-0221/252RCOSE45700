using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] GrowthUI growthUI;
    [SerializeField] BossInfoUI bossInfoUI;
    [SerializeField] RewardInfoUI rewardInfoUI;

    [Header("Layout")]
    [SerializeField] Button changeLayoutButton;
    [SerializeField] Sprite changeLayoutButton_inventory;
    [SerializeField] Sprite changeLayoutButton_battle;
    [SerializeField] Background backgroundYellow;

    [Header("Prefab")]
    public GameObject messageGroupPrefab; // 인스펙터에서 할당
    public Transform popupParent;         // 팝업이 생성될 Canvas

    [Header("Tier Colors")]
    [SerializeField] private List<Color> _tierColorList;
    public IReadOnlyList<Color> tierColorList => _tierColorList;

    [Header("Stat Names")]
    [SerializeField] private List<StatName> _statNamesList;
    public IReadOnlyList<StatName> statNamesList => _statNamesList;

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
        ChangeLayout();
        RefreshAllUI();
        if (changeLayoutButton != null)
        {
            changeLayoutButton.onClick.AddListener(ChangeLayout);
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
        upgradeUI.UpdateInfo(item);
        growthUI.UpdateInfo(item);
    }

    public void DeselectSlot()
    {
        SelectSlot(null);
    }

    public void RefreshAllUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager is null");
            return;
        }

        if (GameManager.Instance.isInventoryMode)
        {
            if (inventoryUI != null) inventoryUI.UpdateUI();
            if (equipmentUI != null) equipmentUI.UpdateEquipmentUI();
        } else
        {
            if (bossInfoUI != null) bossInfoUI.UpdateUI();
            if (rewardInfoUI != null) rewardInfoUI.UpdateUI();
        }
        if (resourceUI != null) resourceUI.UpdateUI();
        if (statusUI != null) statusUI.UpdateUI();
    }

    public void RefreshStatusUI()
    {
        if (statusUI != null) statusUI.UpdateUI();
    }

    public void RefreshResourceUI()
    {
        if (resourceUI != null) resourceUI.UpdateUI();
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

    public void ShowGrowthUI()
    {
        if (growthUI != null)
        {
            growthUI.enable();
        }
    }

    public void ChangeLayout()
    {
        if (inventoryUI == null || equipmentUI == null ||
            bossInfoUI == null || rewardInfoUI == null) return;

        if (GameManager.Instance.isInventoryMode)
        {
            if (changeLayoutButton != null)
            {
                changeLayoutButton.GetComponent<Image>().sprite = changeLayoutButton_inventory;
            }
            inventoryUI.disable();
            equipmentUI.disable();

            DeselectSlot();

            bossInfoUI.UpdateUI();
            rewardInfoUI.UpdateUI();
            bossInfoUI.enable();
            rewardInfoUI.enable();
            
            backgroundYellow.targetFillAmount = 1f;
        }
        else
        {
            if (changeLayoutButton != null)
            {
                changeLayoutButton.GetComponent<Image>().sprite = changeLayoutButton_battle;
            }
            bossInfoUI.disable();
            rewardInfoUI.disable();
            
            DeselectSlot();

            inventoryUI.UpdateUI();
            equipmentUI.UpdateEquipmentUI();
            inventoryUI.enable();
            equipmentUI.enable();
            
            backgroundYellow.targetFillAmount = 0f;
        }

        GameManager.Instance.isInventoryMode = !GameManager.Instance.isInventoryMode;
    }
}