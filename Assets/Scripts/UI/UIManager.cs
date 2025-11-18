using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Singleton
    public static UIManager Instance { get; private set; }

    [Header("UI Component References")]
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] Iteminfo itemInfo;
    [SerializeField] ResourceUI resourceUI;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowItemInfo(ItemInstance item)
    {
        if (itemInfo != null)
        {
            // ItemInfo.cs에 있는 패널 업데이트 함수를 호출
            itemInfo.UpdateInfo(item);
        }
    }
}