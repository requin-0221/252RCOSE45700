using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GrowthProfileImporter : EditorWindow
{
    // CSV 파일명 접두사
    private const string CSV_PREFIX = "GrowthStatTable_";

    // Scriptable Object에 데이터 주입
    private static string baseAssetPath = "Assets/Resources/Data/Growth/";

    private const int STAT_COUNT = 19;

    [MenuItem("Tools/Import Growth Stat Table")]
    public static void ImportStatTable()
    {
        // 프로젝트 내 모든 CSV 파일 검색
        string[] guids = AssetDatabase.FindAssets("GrowthStatTable_ t:TextAsset");
        if (guids.Length == 0)
        {
            Debug.LogWarning($"'{CSV_PREFIX}'로 시작하는 CSV 파일을 찾을 수 없습니다.");
            return;
        }

        int successCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            // 파일명이 접두사로 시작하는지 재확인
            if (!fileName.StartsWith(CSV_PREFIX)) continue;

            // 키워드 추출
            string keyword = fileName.Substring(CSV_PREFIX.Length);

            // CSV 읽기 및 SO 생성
            TextAsset csvFile = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            if (CreateUpgradeProfile(keyword, csvFile.text))
            {
                successCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"GrowthProfile {successCount}개 생성");
    }

    public static bool CreateUpgradeProfile(string keyword, string csvContent)
    {
        string[] lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return false;

        // 데이터를 담을 임시 dictionary
        var statValuesMap = new Dictionary<StatType, List<float>>();
        for (int i = 0; i < STAT_COUNT; i++)
        {
            statValuesMap[(StatType)i] = new List<float>();
        }

        // --- 데이터 파싱 (헤더 무시하고 1번째 줄부터) ---
        for (int row = 1; row < lines.Length; row++)
        {
            string[] rowData = lines[row].Split(',');

            // col 0은 Level이므로 건너뛰고, col 1부터
            for (int col = 1; col <= STAT_COUNT; col++)
            {
                // CSV 열이 부족하면 0 처리
                float value = 0f;
                if (col < rowData.Length)
                {
                    float.TryParse(rowData[col], out value);
                }

                // [핵심] 인덱스로 바로 Enum 캐스팅
                StatType targetType = (StatType)(col - 1);

                // 딕셔너리에 추가
                if (statValuesMap.ContainsKey(targetType))
                {
                    statValuesMap[targetType].Add(value);
                }
            }
        }

        // UpgradeProfile SO 생성 및 주입
        string assetFullPath = baseAssetPath + $"GrowthProfile{keyword}.asset";
        GrowthProfile profile = AssetDatabase.LoadAssetAtPath<GrowthProfile>(assetFullPath);

        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<GrowthProfile>();
            AssetDatabase.CreateAsset(profile, assetFullPath);
        }

        profile.rules = new List<StatIncreaseList>();

        // 딕셔너리 -> 리스트 변환
        foreach (var kvp in statValuesMap)
        {
            // 모든 값이 0인 스탯은 규칙에서 제외
            bool isAllZero = true;
            foreach(var v in kvp.Value) if(v != 0) isAllZero = false;
            if(isAllZero) continue;

            StatIncreaseList newRule = new StatIncreaseList();
            newRule.statType = kvp.Key;
            newRule.values = kvp.Value.ToArray();

            profile.rules.Add(newRule);
        }

        EditorUtility.SetDirty(profile);
        return true;
    }
}