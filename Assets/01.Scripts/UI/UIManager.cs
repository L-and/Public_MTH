using _01.Scripts.PlayerControll.Status;
using DG.Tweening;
using System.Collections;
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
    public Slider hpBar;
    public Slider ohBar;
    public TextMeshProUGUI ohBarText;
    public Slider stmBar;
    public Image hpBarFill;
    public Image ohBarFill;
    public Image stmBarFill;
    public TextMeshProUGUI levelInfo;
    public GameObject pauseMenu;

    public float hp = 100f;
    public float maxHp = 100f;
    public float oh = 100f;
    public float maxOh = 100f;
    public float stm = 1;
    public float maxStm = 1;
    Color hpBarFillColor;
    Color ohBarFillColor;
    Color stmBarFillColor;
    bool ohFulled = false;
    bool stmFulled = false;
    bool isGameover = false;
    bool isPause = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBarFillColor = hpBarFill.color;
        ohBarFillColor = ohBarFill.color;
        stmBarFillColor = stmBarFill.color;
        StartCoroutine(ShowLevelInfoCoroutine());
        UpdateStatus();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetHp(hp + 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetHp(hp - 1);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetOh(oh + 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
        if (stm < 1) SetStm(stm + 0.5f * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.G) && !isGameover)
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        UpdateStatus();
    }

    #region METHOD
    public void SetHp(float value)
    {
        if (value > hp)
        {
            DOTween.Kill(hpBarFill);
            hpBarFill.color = hpBarFillColor;
            hpBarFill.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
        hp = Mathf.Clamp(value, 0, maxHp);
        hpBar.maxValue = maxHp;
        hpBar.value = hp;
    }
    public void SetOh(float value)
    {
        oh = Mathf.Clamp(value, 0, maxOh);
        ohBar.maxValue = maxOh;
        ohBarText.text = oh + "%";
        ohBar.value = oh;
        Color color2; 
        ColorUtility.TryParseHtmlString("#FF8D00", out color2);
        ohBarFill.color = Color.Lerp(ohBarFillColor,color2, oh/100);

        if (oh < maxOh)  ohFulled = false;
        if (!ohFulled && oh >= maxOh)
        {
            ohFulled = true;
            DOTween.Kill(ohBarFill);
            ohBarFill.color = color2;
            ohBarFill.DOColor(Color.white, 0.8f).SetLoops(2,LoopType.Yoyo);
        }
    }
    public void SetStm(float value)
    {
        stm = Mathf.Clamp(value, 0, maxStm);
        stmBar.maxValue = maxStm;
        stmBar.value = stm;

        if (stm < maxStm) stmFulled = false;
        if (!stmFulled && stm >= maxStm)
        {
            stmFulled = true;
            DOTween.Kill(stmBarFill);
            stmBarFill.color = stmBarFillColor;
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
        GlobalMethod.Fade(gameObject, 5f);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator ShowLevelInfoCoroutine()
    {
        levelInfo.DOFade(0, 0);
        levelInfo.DOFade(1, 1);
        yield return new WaitForSeconds(5);
        levelInfo.DOFade(0, 1);
    }

    public void Pause()
    {
        if (!isPause)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            isPause = true;
            return;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            isPause = false;
            return;
        }
    }

    void UpdateStatus()
    {
        if (GameObject.Find("Player") == null) return;

        PlayerStatus playerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();
//        SetHp(playerStatus.hp);
//        SetOh(playerStatus.oh);
//        SetStm(playerStatus.stm);
    }

    #endregion
}

