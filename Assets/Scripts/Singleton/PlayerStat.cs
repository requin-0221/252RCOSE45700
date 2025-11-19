using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    // Singleton
    public static PlayerStat Instance { get; private set; }

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

    private void Initialize()
    {
        Debug.Log("PlayerStat Initialized.");
    }

    public void CalculateAllStats()
    {
        Debug.Log("PlayerStat Calculated.");
    }
}
