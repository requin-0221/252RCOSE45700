using System;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    // Singleton
    public static BossManager Instance { get; private set; }

    // 현재 난이도
    public BossDifficulty difficulty = BossDifficulty.Easy;

    // 현재 보스 ID
    public int currBossID { get; private set; } = 0;

    [SerializeField] private List<BossData> _bossList;
    public IReadOnlyList<BossData> bossList => _bossList;

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
}
