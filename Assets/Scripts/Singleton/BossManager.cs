using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ClearRecord
{
    public int bossID;
    public BossDifficulty difficulty;
}

public class BossManager : MonoBehaviour
{
    // Singleton
    public static BossManager Instance { get; private set; }

    // 현재 난이도
    public BossDifficulty difficulty = BossDifficulty.Easy;

    // 현재 보스 ID
    public int currBossID { get; private set; } = 0;

    [Header("장비 드랍 관리자")]
    [SerializeField] DropManager dropManager;

    [Header("보스 목록")]
    [SerializeField] private List<BossData> _bossList;
    public IReadOnlyList<BossData> bossList => _bossList;

    [Header("클리어 기록")]
    [SerializeField] private List<ClearRecord> clearList;


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
        if (dropManager == null) dropManager = GetComponent<DropManager>();
        Debug.Log("BossManager Initialized.");
    }

    public void SetCurrentBossID(int id)
    {
        id = Mathf.Clamp(id, 0, bossList.Count - 1);
        currBossID = id;
    }

    public void SetDifficulty(BossDifficulty _difficulty)
    {
        difficulty = _difficulty;
    }

    public void SetDifficulty(int _difficulty)
    {
        _difficulty = Mathf.Clamp(_difficulty, 0, Enum.GetNames(typeof(BossDifficulty)).Length-1);
        difficulty = (BossDifficulty)_difficulty;
    }

    public BossData GetCurrentBossData()
    {
        if (bossList.Count == 0 || currBossID >= bossList.Count) return null;
        return bossList[currBossID];
    }

    public bool IsExistClearRecord(int id, BossDifficulty difficulty)
    {
        foreach (var record in clearList)
        {
            if (record.bossID != id) continue;
            if (record.difficulty != difficulty) continue;
            return true;
        }
        return false;
    }

    public void AddClearRecord(int id, BossDifficulty difficulty)
    {
        if (IsExistClearRecord(id, difficulty)) return;
        
        ClearRecord record = new ClearRecord();
        record.bossID = id;
        record.difficulty = difficulty;

        clearList.Add(record);
    }

    // 보상 지급
    public void AddReward()
    {
        bool isFirst = !IsExistClearRecord(currBossID, difficulty);

        BossData data = GetCurrentBossData();
        if (!data.SpecDict.ContainsKey(difficulty)) { return; }
        var dict = data.SpecDict[difficulty];

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddGold(dict.goldReward * (isFirst ? 5 : 1));
            GameManager.Instance.AddStone((int)dict.stoneReward * (isFirst ? 5 : 1));
        }

        if (dropManager != null && InventoryManager.Instance != null)
        {
            var items = dropManager.GenerateDrops(dict.minTier, dict.maxTier, dict.dropCount, dict.dropRate);
            foreach (var item in items)
            {
                InventoryManager.Instance.AddItem(item);
            }
        }

        Debug.Log("보상 지급 완료");

        // 최초 클리어 시 클리어 기록 추가
        if (isFirst) AddClearRecord(currBossID, difficulty);
    }
}
