using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton
    public static InventoryManager Instance;

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
    }

    public void AddItem(ItemData data)
    {
        ItemInstance newItem = new ItemInstance(data);
        inventory.Add(newItem);
        Debug.Log($"아이템 획득: {data.itemName}");

        // UI 갱신
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshAllUI();
        }
    }

    public void RemoveItem(ItemInstance item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);

            // UI 갱신
            if (UIManager.Instance != null)
            {
                UIManager.Instance.RefreshAllUI();
            }
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

        // 3. 판매 로직 실행
        Instance.SellItem(targetItem);
        UIManager.Instance.RefreshAllUI();
    }
}
