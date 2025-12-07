using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton
    public static InventoryManager Instance {get; private set;}

    [Header("테스트용")]
    public List<ItemData> testitems;
    public int testItemNum;

    [Header("Inventory")]
    public List<ItemInstance> inventory = new List<ItemInstance>(); // 인벤토리 리스트
    public List<ItemSlot> slots = new List<ItemSlot>();

    // 장착된 아이템 (Key: 부위, Value: 아이템 인스턴스)
    [Header("Equipments")]
    public Dictionary<EquipSlot, ItemInstance> equipments = new Dictionary<EquipSlot, ItemInstance>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        // 게임 시작 시 빈 딕셔너리 초기화
        equipments.Clear();
        inventory.Clear();

        for (int i = 0; i < testItemNum; i++)
        {
            ItemData temp = testitems[Random.Range(0, testitems.Count)];
            AddItem(temp);
        }

        Debug.Log("Inventory Initialized.");
    }

    public void EquipItem(ItemSlot newItemSlot)
    {
        ItemInstance newItem = newItemSlot._item;
        EquipSlot equipSlot = newItem.data.equipSlot;
        if (equipSlot == EquipSlot.None) return;

        // 이미 다른 아이템이 장착되어 있는지 확인
        if (equipments.ContainsKey(equipSlot) && equipments[equipSlot] != null)
        {
            // 기존 아이템을 해제하여 인벤토리로 되돌림 (Swap)
            ItemInstance oldItem = equipments[equipSlot];
            Debug.Log($"index : {slots.IndexOf(newItemSlot)}, item : {oldItem}");
            inventory.Insert(slots.IndexOf(newItemSlot), oldItem);
        }

        // 3. 새 아이템 장착 처리
        inventory.Remove(newItem);       // 인벤토리에서 제거
        equipments[equipSlot] = newItem;   // 장비창에 등록

        // 4. 마무리 (스탯 재계산 및 UI 갱신)
        OnInventoryChanged();
    }

    public void ReleaseItem(ItemSlot oldItemSlot)
    {
        ItemInstance oldItem = oldItemSlot._item;
        EquipSlot slot = oldItem.data.equipSlot;

        // 장비에서 제거
        if (equipments.ContainsKey(slot) && equipments[slot] == oldItem)
        {
            equipments.Remove(slot);
        }
        inventory.Add(oldItem);

        OnInventoryChanged();
    }

    public void AddItem(ItemData data)
    {
        ItemInstance newItem = new ItemInstance(data);
        inventory.Add(newItem);
        Debug.Log($"아이템 획득: {data.itemName}");

        OnInventoryChanged();
    }

    public void RemoveItem(ItemInstance item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            OnInventoryChanged();
        }
    }

    public void SellItem(ItemInstance item)
    {
        if (!inventory.Contains(item)) return;

        // 판매 가격 계산
        // 나중에 공식 사용
        long sellPrice = item.GetSellPrice();

        // 골드 지급
        GameManager.Instance.AddGold(sellPrice);
        Debug.Log($"아이템 판매: {item.data.itemName} (+{sellPrice} Gold)");

        RemoveItem(item);
    }

    public void OnClickSellButton()
    {
        // 아이템 확인
        ItemSlot targetSlot = UIManager.Instance.CurrSlot;
        ItemInstance targetItem = UIManager.Instance.CurrSlot._item;

        if (targetItem == null)
        {
            Debug.Log("빈 슬롯 오류");
            return;
        }

        // 장착 중인 아이템은 판매 불가 처리
        if (Instance.equipments.ContainsValue(targetItem))
        {
            Debug.Log("장착 중인 아이템은 판매할 수 없습니다.");
            return;
        }

        Instance.SellItem(targetItem);
    }

    public void OnClickTakeButton()
    {
        // 아이템 확인
        ItemSlot targetSlot = UIManager.Instance.CurrSlot;
        ItemInstance targetItem = UIManager.Instance.CurrSlot._item;

        if (targetItem == null)
        {
            Debug.Log("빈 슬롯 오류");
            return;
        }

        // 장착 중인 아이템은 해제
        if (Instance.equipments.ContainsValue(targetItem))
        {
            Instance.ReleaseItem(targetSlot);
        }
        else
        {
            // 아니라면 장착
            Instance.EquipItem(targetSlot);
        }
        return;
    }

    private void OnInventoryChanged()
    {
        if (UIManager.Instance == null || PlayerStat.Instance == null) return;

        // 플레이어 스탯 재계산
        PlayerStat.Instance.CalculateAllStats();

        // UI 갱신
        UIManager.Instance.RefreshAllUI();
        if (UIManager.Instance.CurrSlot != null)
        {
            UIManager.Instance.SelectSlot(UIManager.Instance.CurrSlot);
            return;
        }
        UIManager.Instance.DeselectAll();
    }
}
