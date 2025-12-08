using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class UpgradeTableImporter : EditorWindow
{
    // 강화 테이블 파일 경로
    private static string csvFilePath = "/Resources/Data/UpgradeTable.csv";

    // Scriptable Object에 데이터 주입
    private static string configAssetPath = "Assets/Resources/Data/Upgrade/UpgradeConfig.asset";

    [MenuItem("Tools/Import UpgradeTable")]
    public static void ImportTable()
    {
        string fullPath = Application.dataPath + csvFilePath;

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"CSV 파일 없음: {fullPath}");
            return;
        }

        // 타겟 SO 로드
        UpgradeConfig config = AssetDatabase.LoadAssetAtPath<UpgradeConfig>(configAssetPath);
        if (config == null)
        {
            Debug.LogError($"UpgradeConfig 에셋 없음: {configAssetPath}");
            return;
        }

        // 리스트 초기화
        config.levelTable.Clear();

        // 파일 읽기
        string[] lines = File.ReadAllLines(fullPath);

        // 헤더 제외 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] data = line.Split(',');

            try
            {
                UpgradeData levelData = new UpgradeData();

                // Level
                levelData.level = int.Parse(data[0]);

                // SuccessRate
                float rate = float.Parse(data[1]);
                // 정수로 들어오면 소수로 변환, 1 이하면 그대로 사용
                if (rate > 1.0f) rate /= 100f;
                levelData.successRate = rate;

                // Cost
                levelData.cost = long.Parse(data[2]);

                // CumulativeCost
                levelData.cumulativeCost = long.Parse(data[3]);

                // 리스트에 추가
                config.levelTable.Add(levelData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Line {i + 1}] 파싱 에러: {e.Message}");
            }
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();

        Debug.Log($"강화 단계별 변수 테이블 임포트 완료, 총 {config.levelTable.Count}개 레벨.");
    }
}