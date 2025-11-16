using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 쓴다면

public class ItemSlot : MonoBehaviour
{
    [Header("UI 요소")]
    public Image iconImage;
    public TextMeshProUGUI upgradeText; // 일반 강화 레벨
    public TextMeshProUGUI growthText;  // 성장 강화 레벨
    public Button slotButton;

    private ItemInstance _item; // 이 슬롯이 담고 있는 아이템 데이터

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
            upgradeText.text = $"+{item.upgradeLv}"; // 장비: +1, +2...
        else
            growthText.text = $"+{item.growthLv}"; // 예시

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
    }

    void OnSlotClicked()
    {
        // 아이템 클릭 시 상세 팝업을 띄우거나, 장착 로직 호출
        Debug.Log($"클릭한 아이템: {_item.data.itemName}");

        // 예: UIManager에게 "이 아이템 상세창 띄워줘"라고 요청
        // UIManager.Instance.ShowItemDetail(_item);
    }
}