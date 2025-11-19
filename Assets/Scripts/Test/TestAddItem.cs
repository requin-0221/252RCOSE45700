using UnityEngine;

public class TestAddItem : MonoBehaviour
{
    public ItemData testItemData; // 인스펙터에서 TestSword 연결

    public void OnClickAdd()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(testItemData);
        }
    }
}