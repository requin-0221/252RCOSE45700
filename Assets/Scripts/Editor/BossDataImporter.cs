using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class BossDataImporter : EditorWindow
{
    // 기본 경로 설정 (사용 환경에 맞춰 수정 가능)
    private string csvFilePath = "Assets/Resources/Data/BossDataTable.csv";
    private string soSavePath = "Assets/Resources/Data/Bosses/";

    // 스프라이트 검색을 위한 폴더 (전체 검색은 느릴 수 있으므로 범위 지정 추천)
    // 비워두면 프로젝트 전체에서 검색합니다.
    private string spriteRootFolder = "Assets/Sprites/Boss";

    [MenuItem("Tools/Import Boss Data")]
    public static void ShowWindow()
    {
        GetWindow<BossDataImporter>("Boss Data Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Boss Data Import Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 경로 입력 필드
        csvFilePath = EditorGUILayout.TextField("CSV File Path", csvFilePath);
        soSavePath = EditorGUILayout.TextField("SO Save Path", soSavePath);
        spriteRootFolder = EditorGUILayout.TextField("Sprite Search Folder", spriteRootFolder);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("CSV 구조: Name, Sprite, Difficulty, Hp, Shield, Atk, Def, GoldReward, StoneReward", MessageType.Info);

        if (GUILayout.Button("Import Boss Data", GUILayout.Height(40)))
        {
            ImportData();
        }
    }

    private void ImportData()
    {
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError($"CSV 파일 없음 : {csvFilePath}");
            return;
        }

        // 저장 폴더가 없으면 생성
        if (!Directory.Exists(soSavePath))
        {
            Directory.CreateDirectory(soSavePath);
        }

        string[] lines = File.ReadAllLines(csvFilePath);
        if (lines.Length <= 1)
        {
            Debug.LogWarning("CSV 파일이 비어있거나 헤더만 존재");
            return;
        }

        int successCount = 0;

        // 보스 이름별로 작업하기 위해 딕셔너리에 임시 저장할 수도 있지만,
        // 여기서는 한 줄씩 읽으며 즉시 SO에 반영하는 방식을 사용합니다.
        // (AssetDatabase가 알아서 기존 에셋을 로드해줍니다.)

        // 1번째 줄(헤더) 건너뛰고 시작
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] row = line.Split(',');

            // CSV 컬럼 파싱
            // 0:Name, 1:Sprite, 2:Difficulty, 3:Hp, 4:Shield, 5:Atk, 6:Def, 7:GoldReward, 8:StoneReward
            try
            {
                string bossName = row[0].Trim();
                string spriteName = row[1].Trim();
                string diffStr = row[2].Trim();

                // Enum 파싱 (대소문자 무시)
                BossDifficulty difficulty = (BossDifficulty)Enum.Parse(typeof(BossDifficulty), diffStr, true);

                // 스펙 데이터 생성
                BossSpec spec = new BossSpec
                {
                    maxHp = ParseLongS(row[3]),
                    maxShield = ParseLongS(row[4]),
                    atk = ParseLongS(row[5]),
                    def = ParseLongS(row[6]),
                    goldReward = ParseLongS(row[7]),
                    stoneReward = ParseLongS(row[8])
                };

                // SO 생성 또는 로드 및 데이터 주입
                UpdateBossSO(bossName, spriteName, difficulty, spec);
                successCount++;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Line {i + 1}] 파싱 에러: {e.Message} / 데이터: {line}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Import Complete", $"{successCount}개의 데이터 행이 처리되었습니다.", "OK");
    }

    private void UpdateBossSO(string bossName, string spriteName, BossDifficulty difficulty, BossSpec spec)
    {
        string assetPath = $"{soSavePath}Boss_{spriteName}.asset";

        // 기존 SO 로드
        BossData bossSO = AssetDatabase.LoadAssetAtPath<BossData>(assetPath);

        // 없으면 새로 생성
        if (bossSO == null)
        {
            bossSO = ScriptableObject.CreateInstance<BossData>();
            bossSO.bossName = bossName;
            AssetDatabase.CreateAsset(bossSO, assetPath);
        }

        // 스프라이트 연결
        spriteName = "spr" + spriteName;
        if (bossSO.bossSprite == null || bossSO.bossSprite.name != spriteName)
        {
            Sprite spr = FindSpriteByName(spriteName);
            if (spr != null)
            {
                bossSO.bossSprite = spr;
            }
            else
            {
                Debug.LogWarning($"[{bossName}] 스프라이트를 찾을 수 없습니다: {spriteName}");
            }
        }

        // 스펙 설정
        bossSO.SetSpec(difficulty, spec);

        EditorUtility.SetDirty(bossSO);
    }

    private Sprite FindSpriteByName(string spriteName)
    {
        string[] guids;

        // 검색 범위 지정 (속도 향상)
        if (!string.IsNullOrEmpty(spriteRootFolder) && Directory.Exists(spriteRootFolder))
        {
            guids = AssetDatabase.FindAssets($"{spriteName} t:Sprite", new[] { spriteRootFolder });
        }
        else
        {
            guids = AssetDatabase.FindAssets($"{spriteName} t:Sprite");
        }

        if (guids.Length > 0)
        {
            // 첫 번째 검색 결과 반환
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        return null;
    }

    private long ParseLongS(string value)
    {
        // 문자열이 비어있거나 공백만 있는 경우 0 반환
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        // 파싱 시도 (숫자가 아닌 이상한 문자열이 섞여 있어도 0으로 처리하여 에러 방지)
        if (long.TryParse(value, out long result))
        {
            return result;
        }
        return 0;
    }
}