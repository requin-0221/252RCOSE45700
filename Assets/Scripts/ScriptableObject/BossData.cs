using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BossSpec
{
    public long maxHp;
    public long maxShield;
    public int atk;
    public int def;
    public long goldReward;
    public long stoneReward;
    public int minTier;
    public int maxTier;
    public float dropRate;
    public int dropCount;
}

[System.Serializable]
public struct BossDifficultyData
{
    public BossDifficulty difficulty; // Key
    public BossSpec spec;             // Value
}

[CreateAssetMenu(fileName = "BossData", menuName = "Scriptable Objects/BossData")]
public class BossData : ScriptableObject
{
    [Header("기존 정보")]
    public string bossName;
    public Sprite bossSprite; // 보스 일러스트
    public Sprite bossIcon; // 보스 아이콘

    [Header("난이도별 스펙 정보")]
    // 인스펙터 노출용, CSV 파싱 데이터를 저장
    [SerializeField]
    private List<BossDifficultyData> difficultyList = new List<BossDifficultyData>();

    // 런타임 전용 딕셔너리
    private Dictionary<BossDifficulty, BossSpec> _specDict;

    // 딕셔너리 접근 프로퍼티 (Lazy Initialization)
    public Dictionary<BossDifficulty, BossSpec> SpecDict
    {
        get
        {
            if (_specDict == null)
            {
                _specDict = new Dictionary<BossDifficulty, BossSpec>();
                foreach (var data in difficultyList)
                {
                    // 중복 키 방지 (혹시 모를 데이터 오류 대비)
                    if (!_specDict.ContainsKey(data.difficulty))
                        _specDict.Add(data.difficulty, data.spec);
                }
            }
            return _specDict;
        }
    }

    public void SetSpec(BossDifficulty difficulty, BossSpec spec)
    {
        // 리스트에서 해당 난이도가 이미 있는지 확인
        int index = difficultyList.FindIndex(x => x.difficulty == difficulty);

        BossDifficultyData newData = new BossDifficultyData { difficulty = difficulty, spec = spec };

        if (index >= 0)
        {
            // 덮어쓰기
            difficultyList[index] = newData;
        }
        else
        {
            difficultyList.Add(newData);
        }
    }
}