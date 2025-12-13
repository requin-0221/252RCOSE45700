using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField] CanvasGroup upgradeGroup;

    public Transform content; // ScrollView의 Content
    public GameObject slot;   // 슬롯 프리팹
    public List<ItemSlot> slots = new List<ItemSlot>();

    public int minSlots = 24;       // 기본 슬롯 개수 (40)
    public int slotsPerRow = 8;     // 한 줄당 슬롯 개수 (8)

    private void Awake()
    {
        if (upgradeGroup == null) upgradeGroup = GetComponent<CanvasGroup>();
    }

    public void disable()
    {
        upgradeGroup.alpha = 0f;
        upgradeGroup.interactable = false;
        upgradeGroup.blocksRaycasts = false;
    }

    public void enable()
    {
        upgradeGroup.alpha = 1f;
        upgradeGroup.interactable = true;
        upgradeGroup.blocksRaycasts = true;
    }

    void OnEnable()
    {
        if (InventoryManager.Instance == null) return;
        UpdateUI();
    }

    public void UpdateUI()
    {
        // InventoryManager에서 인벤토리 데이터 가져오기
        List<ItemInstance> inventoryList = InventoryManager.Instance.inventory;

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
        // 필요 시 슬롯 프리팹 제거
        else if (slots.Count > neededSlots)
        {
            for (int i = slots.Count-1; i >= neededSlots; i--)
            {
                if (UIManager.Instance.CurrSlot == slots[i])
                {
                    UIManager.Instance.DeselectSlot();
                }
                Destroy(slots[i].transform.gameObject);
                slots.RemoveAt(i);
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