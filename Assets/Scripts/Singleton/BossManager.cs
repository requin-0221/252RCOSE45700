using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    // Singleton
    public static BossManager Instance { get; private set; }

    public List<BossData> bossList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ¾ÀÀÌ ¹Ù²î¾îµµ ÆÄ±«µÇÁö ¾ÊÀ½
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
}
