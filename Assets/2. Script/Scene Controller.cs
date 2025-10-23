using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class CutsceneController : MonoBehaviour
{
    [System.Serializable]
    public class Cut
    {
        public Sprite image;
        [TextArea] public string text;
        public float fadeDuration = 1f;
        public bool isPanEffect = false;  //  4번 컷용 옵션
        public Vector2 panOffset = new Vector2(0, 800f); // 4번 pan 움직일 거리
    }

    [Header("UI 연결")]
    public Image cutImage;   // 일반 컷 (1500x550)
    public Image cutImageLarge;  //4번 컷 (1500x1357) 
    public TextMeshProUGUI cutText;
    
    public Cut[] cuts;
    public string nextSceneName = "Game Test";

    private bool isTyping = false;
    private bool nextPressed = false;
    

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    void Update()
    {
        // 유저가 Enter(혹은 Space) 입력하면 플래그 세팅
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            nextPressed = true;
        }
    }

    IEnumerator PlayCutscene()
    {
        for (int i = 0; i < cuts.Length; i++)
        {
            Cut cut = cuts[i];
            bool isPan = cut.isPanEffect;

            // 어떤 이미지 쓸지 선택
            Image activeImage = isPan ? cutImageLarge : cutImage;
            cutImage.gameObject.SetActive(!isPan);
            cutImageLarge.gameObject.SetActive(isPan);

            // 초기 세팅
            activeImage.sprite = cut.image;
            cutText.text = "";
            SetAlpha(activeImage, 0);
            SetAlpha(cutText, 0);

            // 페이드 인
            yield return FadeBoth(activeImage, cutText,0, 1, cut.fadeDuration);

            // 4번 컷 pan 효과
            if (isPan)
            {
                yield return StartCoroutine(PanUp(cutImageLarge.rectTransform, cut.panOffset, 4f));
            }
            // 텍스트 타이핑
            yield return StartCoroutine(TypeText(cut.text));

            
            // 엔터 입력 기다리기
            yield return new WaitUntil(() => nextPressed);
            nextPressed = false;

            //페이드 아웃
            yield return FadeBoth(activeImage, cutText, 1, 0, cut.fadeDuration);
        }

        // 컷씬 끝 → 다음 씬 이동
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeBoth(Graphic img, Graphic txt, float start, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            float alpha = Mathf.Lerp(start, end, t);
            SetAlpha(img, alpha);
            SetAlpha(txt, alpha);
            time += Time.deltaTime;
            yield return null;
        }
        SetAlpha(img, end);
        SetAlpha(txt, end);
    }

    void SetAlpha(Graphic g, float a)
    {
        if (g == null) return;
        Color c = g.color;
        c.a = a;
        g.color = c;
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        cutText.text = "";
        SetAlpha(cutText, 1);
        foreach (char c in text)
        {
            cutText.text += c;

            // 유저가 Enter를 빠르게 눌렀다면 즉시 전체 텍스트 표시
            if (nextPressed)
            {
                cutText.text = text;
                nextPressed = false;
                break;
            }

            // 글자 당 속도
            yield return new WaitForSeconds(0.3f);
        }
        isTyping = false;
    }

    IEnumerator PanUp(RectTransform rect, Vector2 offset, float duration)
    {
        Vector2 start = rect.anchoredPosition;
        Vector2 end = start - offset;

        float time = 0f;
        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);
            rect.anchoredPosition = Vector2.Lerp(start, end, t);
            time += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = end;
    }
}