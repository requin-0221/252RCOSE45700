using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [Header("UI 요소")]
    public Image iconImage;
    public GameObject selectionBorder;
    public TextMeshProUGUI upgradeText; // 일반 강화 레벨
    public TextMeshProUGUI growthText;  // 성장 강화 레벨
    public Button slotButton;

    public ItemInstance _item { get; private set; } // 슬롯 아이템 데이터

    // 초기화 (InventoryUI가 호출)
    public void SetItem(ItemInstance item)
    {
        _item = item;

        if (item.data.icon != null)
        {
            iconImage.sprite = item.data.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false; // 아이콘 없으면 숨김
        }

        if (item.data.equipSlot != EquipSlot.None)
        {
            if (item.upgradeLv == 0) upgradeText.text = null;
            else upgradeText.text = $"+{item.upgradeLv}";

            if (item.growthLv == 0) growthText.text = null;
            else growthText.text = $"+{item.growthLv}";
        }
        else
        {
            upgradeText.text = null;
            growthText.text = null;
        }

        slotButton.onClick.RemoveAllListeners(); // 재사용 시 중복 방지
        slotButton.onClick.AddListener(OnSlotClicked);
    }

    public void ClearSlot()
    {
        _item = null;

        iconImage.sprite = null;
        iconImage.enabled = false; // 아이콘 숨김 (배경만 보임)
        upgradeText.enabled = false;
        growthText.enabled = false;

        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.CurrSlot == this) UIManager.Instance.SelectSlot(this);
        }

        slotButton.onClick.RemoveAllListeners(); // 재사용 시 중복 방지
        slotButton.onClick.AddListener(OnSlotClicked);
    }

    public void SetSelectState(bool isSelected)
    {
        if (selectionBorder != null)
        {
            selectionBorder.SetActive(isSelected);
        }
    }

    void OnSlotClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SelectSlot(this);
        }
    }
}