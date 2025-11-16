using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Transform content; // ScrollView의 Content
    public GameObject slot;   // 슬롯 프리팹

    public int minSlots = 36;       // 기본 슬롯 개수 (36)
    public int slotsPerRow = 6;     // 한 줄당 슬롯 개수 (6)

    // 슬롯 재사용 리스트
    private List<ItemSlot> slots = new List<ItemSlot>();

    private void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // GameManager에서 인벤토리 데이터 가져오기
        List<ItemInstance> inventoryList = GameManager.Instance.inventory;

        int neededSlots = Mathf.Max(minSlots, inventoryList.Count);
        if (neededSlots % slotsPerRow != 0)
        {
            neededSlots = (neededSlots / slotsPerRow + 1) * slotsPerRow;
        }

        // 필요 시 슬롯 프리팹 생성
        if (slots.Count < neededSlots)
        {
            int toAdd = neededSlots - slots.Count;
            for (int i = 0; i < toAdd; i++)
            {
                GameObject obj = Instantiate(slot, content);
                ItemSlot slotUI = obj.GetComponent<ItemSlot>();
                slots.Add(slotUI);
            }
        }

        // 슬롯 프리팹 갱신
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventoryList.Count)
            {
                // 데이터가 있는 칸 -> 아이템 정보 표시
                slots[i].SetItem(inventoryList[i]);
            }
            else
            {
                // 데이터가 없는 칸 -> 빈 슬롯 처리
                slots[i].ClearSlot();
            }
        }
    }
}