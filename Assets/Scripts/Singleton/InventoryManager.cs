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

    private void Start()
    {
        // 테스트 아이템 추가
        for (int i = 0; i < testItemNum; i++)
        {
            ItemData temp = testitems[Random.Range(0, testitems.Count)];
            AddItem(temp);
        }
    }

    void Initialize()
    {
        // 게임 시작 시 빈 딕셔너리 초기화
        equipments.Clear();
        inventory.Clear();

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
            Debug.Log($"index : {inventory.IndexOf(newItem)}, item : {oldItem}");
            inventory.Insert(inventory.IndexOf(newItem), oldItem);
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
        long sellPrice = item.GetSellPrice();

        // 골드 지급
        GameManager.Instance.AddGold(sellPrice);
        Debug.Log($"아이템 판매: {item.data.itemName} (+{sellPrice} Gold)");

        RemoveItem(item);
    }

    public bool UpgradeAttempt(ItemInstance item)
    {
        if (item == null) return false;

        // 판매 가격 계산
        long cost = item.GetUpgradeCost();

        // 강화 비용 지불
        GameManager.Instance.AddGold(-1 * cost);
        Debug.Log($"아이템 강화: {item.data.itemName} (-{cost} Gold)");

        if (Random.Range(0f, 1f) <= UpgradeConfig.Instance.GetSuccessProb(item.upgradeLv))
        {
            item.Upgrade();
            OnInventoryChanged();
            return true;
        }
        UIManager.Instance.RefreshResourceUI();
        return false;
    }

    public void GrowingAttempt(ItemInstance item)
    {
        if (item == null) return;

        // 성장 비용 계산
        int cost = item.GetGrowthCost();

        // 성장 비용 지불
        GameManager.Instance.AddStone(-1 * cost);
        Debug.Log($"성장 : {item.data.itemName} (-{cost} Stone)");

        item.Growing();
        OnInventoryChanged();
    }

    public void GrowthStackRestore(ItemInstance item, StatType type)
    {
        if (item == null) return;
        if (!item.growthStacks.ContainsKey(type)) return;

        int stackNum = item.growthStacks[type];

        // 초기화 비용 계산
        long cost = item.GetGrowthStackRestoreCost(stackNum);

        // 초기화 비용 지불
        GameManager.Instance.AddGold(-1 * cost);
        Debug.Log($"스택 초기화 : {item.data.itemName} (-{cost} Gold)");

        item.GrowthStackRestore(type);
        OnInventoryChanged();
    }

    public void GrowthEntireRestore(ItemInstance item)
    {
        if (item == null) return;

        // 초기화 가격 계산
        long cost = item.GetEntireRestoreCost();

        // 초기화 비용 지불
        GameManager.Instance.AddGold(-1 * cost);
        Debug.Log($"전체 스택 초기화 : {item.data.itemName} (-{cost} Gold)");

        item.GrowthEntireRestore();
        OnInventoryChanged();
    }

    private void OnInventoryChanged()
    {
        if (PlayerStat.Instance != null)
        {
            // 플레이어 스탯 재계산
            PlayerStat.Instance.CalculateAllStats();
        }

        // UI 갱신
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshAllUI();
            if (UIManager.Instance.CurrSlot != null)
            {
                UIManager.Instance.SelectSlot(UIManager.Instance.CurrSlot);
                return;
            }
            UIManager.Instance.DeselectSlot();
        }
    }
}
