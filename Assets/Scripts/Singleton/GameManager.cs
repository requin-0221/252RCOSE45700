using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    [Header("테스트용")]
    public ItemData testitem;

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

        for(int i = 0; i < 50; i++)
        {
            AddItem(testitem);
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
}