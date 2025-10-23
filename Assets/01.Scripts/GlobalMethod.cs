using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class GlobalMethod
{
    public static void Fade(GameObject canvas, float time, bool isOut = true, bool isWhite = true)
    {
        GameObject fade = new GameObject("Fade");
        fade.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = fade.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.one;


        Image image = fade.AddComponent<Image>();
        if (!isWhite) image.color = Color.black;
        if (!isOut)
        {
            image.DOFade(0, 0);
            image.DOFade(1, time);
            return;
        }
        image.DOFade(1, 0);
        image.DOFade(0, time).OnComplete(() => UnityEngine.Object.Destroy(fade));
    }
}
