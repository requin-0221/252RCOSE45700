using UnityEngine;
using System.Collections.Generic;

public class EquipmentUI : MonoBehaviour
{
    // --- [1] 인스펙터 설정을 위한 매핑 구조체 ---
    [System.Serializable]
    public struct SlotMapping
    {
        public EquipSlot slotType;
        public ItemSlot slotScript;
    }

    [Header("EquipSlot Map")]
    public List<SlotMapping> slotMappings;

    // 빠른 접근을 위한 딕셔너리
    private Dictionary<EquipSlot, ItemSlot> uiSlotLookup = new Dictionary<EquipSlot, ItemSlot>();


    private void Awake()
    {
        // 리스트를 딕셔너리로 변환 (검색 속도 최적화)
        foreach (var mapping in slotMappings)
        {
            if (mapping.slotScript != null && !uiSlotLookup.ContainsKey(mapping.slotType))
            {
                uiSlotLookup.Add(mapping.slotType, mapping.slotScript);
            }
        }
    }

    private void Start()
    {
        if (InventoryManager.Instance == null) return;
        UpdateEquipmentUI();
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance == null) return;
        UpdateEquipmentUI();
    }

    public void UpdateEquipmentUI()
    {
        // GameManager가 데이터 관리
        var equippedData = InventoryManager.Instance.equipments;

        // UI에 등록된 모든 슬롯을 순회
        foreach (var kvp in uiSlotLookup)
        {
            EquipSlot type = kvp.Key;
            ItemSlot uiSlot = kvp.Value;

            if (equippedData.TryGetValue(type, out ItemInstance item) && item != null)
            {
                // 장착 아이템 있음 -> 슬롯 활성화
                uiSlot.SetItem(item);
            }
            else
            {
                // 장착 아이템 없음 -> 슬롯 비우기
                uiSlot.ClearSlot();
            }
        }
    }
}
