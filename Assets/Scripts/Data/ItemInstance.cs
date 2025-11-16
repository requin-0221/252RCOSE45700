using System;
using System.Collections.Generic;

[Serializable]
public class ItemInstance
{
    public ItemData data;

    // 고유 ID
    public string uniqueID;
    
    // 상태
    public int upgradeLv; // 일반 강화 레벨
    public int growingLv; // 성장 레벨(총 스택)

    // 성장 능력치 스택 (Key: 능력치, Value: 스택 수)
    public Dictionary<StatType, int> growingStacks;

    // 생성자 (아이템 획득 시 호출)
    public ItemInstance(ItemData itemData)
    {
        this.data = itemData;
        this.uniqueID = Guid.NewGuid().ToString(); // 고유값 생성
        this.upgradeLv = 0;
        this.growingLv = 0;
        this.growingStacks = new Dictionary<StatType, int>();
    }
}