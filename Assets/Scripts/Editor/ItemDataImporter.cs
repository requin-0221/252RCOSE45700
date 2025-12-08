using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class ItemDataImporter : EditorWindow
{
    // Item Table Path
    static string csvPath = "/Resources/Data/ItemTable.csv";

    // Data Export Target Path
    static string baseExportPath = "Assets/Resources/Data/Equipments/";

    // Item Icon Image Path
    static string baseIconPath = "Assets/Sprites/Equipments/";

    // 헤더
    static Dictionary<string, int> headerMap = new Dictionary<string, int>();

    // 장비 타입별 카운터
    static Dictionary<string, int> typeCounters = new Dictionary<string, int>();

    // 에디터 수정
    [MenuItem("Tools/Import Item Data from CSV")]
    public static void ImportData()
    {
        string fullPath = Application.dataPath + csvPath;

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"CSV 파일 없음: {fullPath}");
            return;
        }

        string[] lines = File.ReadAllLines(fullPath);
        if (lines.Length < 2) // 헤더만 있는 경우
        {
            Debug.LogError($"CSV가 헤더만 있음: {fullPath}");
            return;
        }

        // 헤더 파싱
        headerMap.Clear();
        string[] headers = lines[0].Split(',');
        for (int i = 0; i < headers.Length; i++)
        {
            headerMap[headers[i].Trim()] = i;
        }

        // 카운터 초기화
        typeCounters.Clear();

        // 데이터 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] block = line.Split(',');

            try
            {
                CreateOrUpdateItem(block);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Line {i + 1}] 파싱 에러: {e.Message}\n{line}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("아이템 데이터 임포트 완료!");
    }

    private static void CreateOrUpdateItem(string[] data)
    {
        // 에셋 파일 이름 및 경로 결정
        string equipType = data[headerMap["EquipType"]].Trim();

        // 카운터 증가
        if (!typeCounters.ContainsKey(equipType))
            typeCounters[equipType] = 0;

        int currentNumber = typeCounters[equipType]++;

        // 추출 폴더 경로 생성 (예: ../Data/Equipments/Sword/)
        string targetPath = baseExportPath + equipType + "/";
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        // 파일명 생성 (Sword_0.asset)
        string fileName = $"{equipType}_{currentNumber}.asset";
        string assetPath = targetPath + fileName;


        // ItemData 생성/로드
        ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemData>();
            AssetDatabase.CreateAsset(item, assetPath);
        }

        // ItemData.cs 필드 매핑

        // 아이템 이름
        item.itemName = data[headerMap["ItemName"]];

        // 아이콘
        string iconPath = baseIconPath + data[headerMap["EquipType"]] + "/" + $"spr{equipType}_{currentNumber}" + ".png";
        Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        if (loadedSprite != null)
        {
            item.icon = loadedSprite;
        }
        else
        {
            // 경로가 틀렸을 때 경고
            Debug.LogWarning($"[ID: {item.name}] 아이콘을 찾을 수 없습니다: {iconPath}");
        }

        // Price
        item.baseSellPrice = long.Parse(data[headerMap["BaseSellPrice"]]);

        // EquipSlot (Enum)
        if (Enum.TryParse(data[headerMap["EquipSlot"]], out EquipSlot slot))
            item.equipSlot = slot;

        item.itemTier = int.Parse(data[headerMap["ItemTier"]]);
        item.maxUpgrade = int.Parse(data[headerMap["MaxUpgrade"]]);
        item.maxGrowth = int.Parse(data[headerMap["MaxGrowth"]]);

        // AttackType
        string atkTypeStr = data[headerMap["AttackType"]];
        if (!string.IsNullOrEmpty(atkTypeStr) && Enum.TryParse(atkTypeStr, out AttackType aType))
            item.attackType = aType;
        else
            item.attackType = AttackType.Physical;

        // UpgradeProfile
        string upgradeProfileType = data[headerMap["UpgradeProfile"]];
        string upgradeProfilePath = "Data/Upgrade/UpgradeProfile";
        switch(upgradeProfileType)
        {
            case "방어구":
                upgradeProfilePath = upgradeProfilePath + "Armor";
                break;

            case "장신구":
                upgradeProfilePath = upgradeProfilePath + "Accessory";
                break;

            case "무기":
                upgradeProfilePath = upgradeProfilePath + "Weapon";
                break;

            case "보조무기":
                upgradeProfilePath = upgradeProfilePath + "Subweapon";
                break;

            default:
                upgradeProfilePath = upgradeProfilePath + "Armor";
                break;
        }
        item.upgradeProfile = Resources.Load<UpgradeProfile>(upgradeProfilePath);

        // 스탯 리스트 처리
        item.baseStats = new List<StatData>();

        foreach (string statName in Enum.GetNames(typeof(StatType)))
        {
            if (headerMap.ContainsKey(statName))
            {
                int col = headerMap[statName];
                
                if (col < data.Length)
                {
                    string valueStr = data[col];
                    if (!string.IsNullOrEmpty(valueStr) && float.TryParse(valueStr, out float value))
                    {
                        if (value != 0)
                        {
                            StatData newData = new StatData();
                            newData.statType = (StatType)Enum.Parse(typeof(StatType), statName);
                            newData.value = value;
                            item.baseStats.Add(newData);
                        }
                    }
                }
            }
        }

        EditorUtility.SetDirty(item);
    }
}