using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance {get; private set;}

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

    [Header("Resources")]
    public long gold { get; private set; } = 0;
    const long maxGold = 999999999999;
    public int stones { get; private set; } = 0; // 강화 재료
    const int maxStones = 9999999;

    private void Initialize()
    {
        // (테스트) 골드 지급
        gold = 100000000000;
        stones = 50;

        Debug.Log("GameManager Initialized.");
    }

    public void AddGold(long value)
    {
        gold += value;
    }
}