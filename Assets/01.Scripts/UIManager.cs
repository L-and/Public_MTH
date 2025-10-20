using DG.Tweening;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject hud;
    public GameObject gameover;
    public Image gameoverBackground;
    public Image gameoverFade;
    public Slider hpBar;
    public Slider ohBar;
    public TextMeshProUGUI ohBarText;
    public Slider stmBar;
    public Image ohBarFill;
    public Image stmBarFill;

    float hp = 100f;
    float maxHp = 100f;
    float oh = 100f;
    float maxOh = 100f;
    float stm = 1;
    float maxStm = 1;
    bool ohFulled = false;
    bool stmFulled = false;
    bool isGameover = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StatusReset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetHp(hp + 1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetHp(hp - 1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetOh(oh + 1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetOh(oh - 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (stm >= 1)
            {
                SetStm(stm - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Restart();
        }
        if (stm < 1) SetStm(stm + 0.5f * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.G) && !isGameover)
        {
            GameOver();
        }
    }
    void SetHp(float value)
    {
        hp = Mathf.Clamp(value, 0, maxHp);
        hpBar.maxValue = maxHp;
        hpBar.value = hp;
    }
    void SetOh(float value)
    {
        oh = Mathf.Clamp(value, 0, maxOh);
        ohBar.maxValue = maxOh;
        ohBarText.text = oh + "%";
        ohBar.value = oh;
        Color color1;
        ColorUtility.TryParseHtmlString("#D6D21D", out color1);
        Color color2; 
        ColorUtility.TryParseHtmlString("#FF8D00", out color2);
        ohBarFill.color = Color.Lerp(color1,color2, oh/100);

        if (oh < maxOh)  ohFulled = false;
        if (!ohFulled && oh >= maxOh)
        {
            ohFulled = true;
            ohBarFill.DOColor(Color.white, 0.8f).SetLoops(2,LoopType.Yoyo);
        }
    }
    void SetStm(float value)
    {
        stm = Mathf.Clamp(value, 0, maxStm);
        stmBar.maxValue = maxStm;
        stmBar.value = stm;

        if (stm < maxStm) stmFulled = false;
        if (!stmFulled && stm >= maxStm)
        {
            stmFulled = true;
            stmBarFill.DOColor(Color.white, 0.15f).SetLoops(2, LoopType.Yoyo);
        }
    }
    
    void StatusReset()
    {
        SetHp(maxHp);
        SetOh(maxOh);
        SetStm(maxStm);
    }

    void GameOver()
    {
        isGameover = true;
        hud.SetActive(false);
        gameover.SetActive(true);
        gameoverBackground.DOFade(0.2f, 0f);
        gameoverBackground.DOFade(1f, 8f);
        gameoverFade.DOFade(1f, 0f);
        gameoverFade.DOFade(0f, 5f);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
