using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField] private List<ItemData> allItems;

    public List<ItemData> GenerateDrops(int minTier, int maxTier, int count, float dropChance)
    {
        List<ItemData> result = new List<ItemData>();

        // 후보군 필터링
        var candidates = allItems
            .Where(item => item.itemTier >= minTier && item.itemTier <= maxTier)
            .ToList();

        if (candidates.Count == 0) return result;

        // 최대 획득 가능 개수만큼 반복
        for (int i = 0; i < count; i++)
        {
            if (Random.value > dropChance)
            {
                continue;
            }

            // 획득 성공 시: 가중치 룰렛
            ItemData pickedItem = GetDynamicWeightedRandomItem(candidates);
            if (pickedItem != null)
            {
                result.Add(pickedItem);
            }
        }

        return result;
    }

    // 가중치 랜덤 뽑기
    private ItemData GetDynamicWeightedRandomItem(List<ItemData> items)
    {
        // 전체 가중치 합 구하기
        float totalWeight = 0;

        // 가중치 리스트
        List<float> currentWeights = new List<float>();

        foreach (var item in items)
        {
            // 공식: 100 / Tier
            float weight = 100f / item.itemTier;

            currentWeights.Add(weight);
            totalWeight += weight;
        }

        // 랜덤 포인트 선정
        float randomPoint = Random.Range(0, totalWeight);

        // 룰렛
        for (int i = 0; i < items.Count; i++)
        {
            if (randomPoint < currentWeights[i])
            {
                return items[i];
            }
            randomPoint -= currentWeights[i];
        }

        return items.Last();
    }
}