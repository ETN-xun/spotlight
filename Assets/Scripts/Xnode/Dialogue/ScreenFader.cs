using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }
    
    [SerializeField] private Image fadeImage; // 纯色遮罩

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 确保初始状态是透明的
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        }
    }

    /// <summary>
    /// 执行一个完整的“淡入-停留-淡出”渐变效果
    /// </summary>
    /// <param name="duration">单程渐变时长</param>
    /// <param name="holdTime">全黑停留时长</param>
    /// <param name="color">渐变颜色</param>
    /// <param name="onPeak">当画面达到最黑时执行的回调</param>
    /// <param name="onComplete">当整个渐变（包括淡出）完成时执行的回调</param>
    public void StartFade(float duration, float holdTime, Color color, System.Action onPeak, System.Action onComplete)
    {
        StartCoroutine(FadeCoroutine(duration, holdTime, color, onPeak, onComplete));
    }

    private IEnumerator FadeCoroutine(float duration, float holdTime, Color color, System.Action onPeak, System.Action onComplete)
    {
        float timer = 0f;
        
        // --- 淡入过程 ---
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        // 确保最终是全黑
        fadeImage.color = new Color(color.r, color.g, color.b, 1);
        
        // 在画面最黑时执行回调（例如隐藏UI）
        onPeak?.Invoke();
        yield return new WaitForSeconds(holdTime);
        
        // --- 淡出过程 (已修复) ---
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        // 确保最终是全透明
        fadeImage.color = new Color(color.r, color.g, color.b, 0);

        // 在所有效果结束后执行回调
        onComplete?.Invoke(); 
    }
}