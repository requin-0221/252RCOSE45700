using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] string nextScene;
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeInDuration = 2f;
    [SerializeField] float fadeOutDuration = 2f;

    bool isFadingIn = true;
    bool isFadingOut = false;

    void Start()
    {
        isFadingIn = true;
        fadeImage.color = Color.black;
    }

    void Update()
    {
        if (isFadingIn) // 입장 시 페이드 인
        {
            float alpha = fadeImage.color.a - Time.deltaTime / fadeInDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            fadeImage.raycastTarget = true;
            if (alpha <= 0)
            {
                fadeImage.raycastTarget = false;
                isFadingIn = false;
            }
        }
        else if (isFadingOut) // 페이드 아웃 후 다음 페이즈로 전환
        {
            float alpha = fadeImage.color.a + Time.deltaTime / fadeOutDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            fadeImage.raycastTarget = true;
            if (alpha >= 1)
            {
                if (nextScene != null)
                {
                    SceneManager.LoadScene(nextScene);
                }
            }
        }
    }

    public void StartFadeOut(string next)
    {
        if (next != null) nextScene = next;
        isFadingOut = true;
    }
}
