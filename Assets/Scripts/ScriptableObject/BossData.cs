using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Scriptable Objects/BossData")]
public class BossData : ScriptableObject
{
    [Header("기본 정보")]

    [Tooltip("보스 이름")]
    public string bossName;

    [Tooltip("보스 일러스트")]
    public Sprite illust;

    [Tooltip("보스 난이도")]
    public BossDifficulty difficulty;
}
