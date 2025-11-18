using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    [Header("테스트용")]
    public ItemData testitem;
    public ItemData testitem2;

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

    // --- 플레이어 데이터 (원래는 PlayerData 클래스로 분리하지만 14일이니 합침) ---
    [Header("Resources")]
    public long gold { get; private set; } = 0;
    const long maxGold = 999999999999;
    public int stones { get; private set; } = 0; // 강화 재료
    const int maxStones = 9999999;

    [Header("Inventory")]
    public List<ItemInstance> inventory = new List<ItemInstance>(); // 인벤토리 리스트

    // 장착된 아이템 (Key: 부위, Value: 아이템 인스턴스)
    [Header("Equipments")]
    public Dictionary<EquipSlot, ItemInstance> equipments = new Dictionary<EquipSlot, ItemInstance>();


    // --- 테스트용 초기화 ---
    private void Initialize()
    {
        // 게임 시작 시 빈 딕셔너리 초기화
        equipments.Clear();
        inventory.Clear();

        // (테스트) 골드 지급
        gold = 10000;
        stones = 50;

        Debug.Log("GameManager Initialized.");

        for(int i = 0; i < 25; i++)
        {
            AddItem(testitem);
            AddItem(testitem2);
        }
    }

    // --- 편의 기능: 아이템 획득 ---
    public void AddItem(ItemData data)
    {
        ItemInstance newItem = new ItemInstance(data);
        inventory.Add(newItem);
        Debug.Log($"아이템 획득: {data.itemName}");

        // TODO: 나중에 UI 갱신 이벤트를 여기서 호출해야 함
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
                UIManager.Instance.SelectSlot(UIManager.Instance.CurrSlot);
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
        gold += sellPrice;
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
    }
}