using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }
    
    [SerializeField] private Image fadeImage; //   纯色遮罩

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// 执行一个完整的“淡入-停留-淡出”渐变效果
    /// </summary>
    /// <param name="duration">单程渐变时长</param>
    /// <param name="holdTime">全黑停留时长</param>
    /// <param name="color">渐变颜色</param>
    /// <param name="onPeak">当画面达到最黑时执行的回调</param>
    public void StartFade(float duration, float holdTime, Color color, System.Action onPeak,System.Action onComplete)
    {
//        Debug.Log(333333);
        StartCoroutine(FadeCoroutine(duration, holdTime, color, onPeak,onComplete));
    }

    private IEnumerator FadeCoroutine(float duration, float holdTime, Color color, System.Action onPeak,System.Action onComplete)
    {
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        // 确保最终是全黑
        fadeImage.color = new Color(color.r, color.g, color.b, 1);
        
        // 关闭对话UI
        onPeak?.Invoke();
        yield return new WaitForSeconds(holdTime);
        
        /*timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }*/
        // 确保最终是全透明
        fadeImage.color = new Color(color.r, color.g, color.b, 0);
        onComplete?.Invoke(); 
    }
}