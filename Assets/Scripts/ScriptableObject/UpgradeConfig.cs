using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct UpgradeData
{
    public int level;               // 현재 강화 단계
    public float successRate;       // 성공 확률 0.0 ~ 1.0
    public long cost;               // 비용
    public long cumulativeCost;     // 누적 기댓값 (판매가 계산용)
}

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Scripts/Data/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject
{
    public static UpgradeConfig Instance // 유사 싱글톤
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<UpgradeConfig>("Data/Upgrade/UpgradeConfig");
            return _instance;
        }
    }
    private static UpgradeConfig _instance;

    [Header("강화 테이블 데이터")]
    public List<UpgradeData> levelTable = new List<UpgradeData>();

    // 강화 누적 기댓값
    public long GetUpgradeCost(int currentLevel)
    {
        // 범위를 벗어나면 마지막 값 반환
        if (currentLevel >= levelTable.Count)
            return levelTable[levelTable.Count - 1].cost;

        return levelTable[currentLevel].cost;
    }

    // 강화 성공 확률
    public float GetSuccessProb(int currentLevel)
    {
        if (currentLevel >= levelTable.Count)
            return 0f;

        return levelTable[currentLevel].successRate;
    }

    // 강화 가능 여부 체크
    public bool CanUpgrade(ItemInstance item)
    {
        // 아이템 고유의 한계치보다 현재 레벨이 낮아야 강화 가능
        return item.upgradeLv < item.data.maxUpgrade;
    }

    public long GetCumulativeCost(int currentLevel)
    {
        // 강화 안 된 상태(0강)는 추가 비용 0
        if (currentLevel <= 0) return 0;

        // 테이블 범위 체크 (1강은 index 0에 위치한다고 가정 시)
        int index = currentLevel - 1;
        if (index >= levelTable.Count)
            return levelTable[levelTable.Count - 1].cumulativeCost;

        return levelTable[index].cumulativeCost;
    }
}
