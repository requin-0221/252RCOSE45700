using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    private Image img;

    public float targetFillAmount = 0f;
    [SerializeField] private float lerpSpeed = 3f;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        if (Mathf.Abs(img.fillAmount - targetFillAmount) > 0.000001f)
        {
            // Lerp로 새로운 fillAmount 계산
            float newAmount = Mathf.Lerp(img.fillAmount, targetFillAmount, Time.deltaTime * lerpSpeed);
            img.fillAmount = newAmount;
        }
    }
}
