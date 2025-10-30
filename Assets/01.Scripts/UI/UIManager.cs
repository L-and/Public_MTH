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
    public Slider hpBar;
    public Slider ohBar;
    public TextMeshProUGUI ohBarText;
    public Slider stmBar;
    public Image hpBarFill;
    public Image ohBarFill;
    public Image stmBarFill;
    public TextMeshProUGUI levelInfo;
    public GameObject pauseMenu;
    public TextMeshProUGUI statText;

    Color hpBarFillColor;
    Color ohBarFillColor;
    Color stmBarFillColor;
    PlayerStatus playerStatus;
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
        if (GameObject.FindWithTag("Player") != null)
        {
            playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
            UpdateStatus();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (playerStatus.stamina.Value >= 1)
            {
                SetStm(playerStatus.stamina.Value - 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && !isGameover)
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (playerStatus != null)
        {   
            if (playerStatus.stamina.Value < playerStatus.stamina.maxValue) SetStm(playerStatus.stamina.Value + 0.5f * Time.deltaTime);
            UpdateStatus();
        }
    }
    #region METHOD
    public void SetHp(float value)
    {
        if (value > playerStatus.hp.Value)
        {
            DOTween.Kill(hpBarFill);
            hpBarFill.color = hpBarFillColor;
            hpBarFill.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
        playerStatus.hp.Value = Mathf.Clamp(value, 0, playerStatus.hp.maxValue);
        hpBar.maxValue = playerStatus.hp.maxValue;
        hpBar.value = playerStatus.hp.Value;
    }
    public void SetOh(float value)
    {
        playerStatus.overheat.Value = playerStatus.overheat.maxValue;
        ohBar.maxValue = playerStatus.overheat.maxValue;
        ohBarText.text = playerStatus.overheat.Value + "%";
        ohBar.value = playerStatus.overheat.Value;
        Color color2;
        ColorUtility.TryParseHtmlString("#FF8D00", out color2);
        ohBarFill.color = Color.Lerp(ohBarFillColor, color2, playerStatus.overheat.Value / 100);

        if (playerStatus.overheat.Value < playerStatus.overheat.maxValue) ohFulled = false;
        if (!ohFulled && playerStatus.overheat.Value >= playerStatus.overheat.maxValue)
        {
            ohFulled = true;
            DOTween.Kill(ohBarFill);
            ohBarFill.color = color2;
            ohBarFill.DOColor(Color.white, 0.8f).SetLoops(2, LoopType.Yoyo);
        }
    }
    public void SetStm(float value)
    {
        playerStatus.stamina.Value = value;
        stmBar.maxValue = playerStatus.stamina.maxValue;
        stmBar.value = playerStatus.stamina.Value;

        if (playerStatus.stamina.Value < playerStatus.stamina.maxValue) stmFulled = false;
        if (!stmFulled && playerStatus.stamina.Value >= playerStatus.stamina.maxValue)
        {
            stmFulled = true;
            DOTween.Kill(stmBarFill);
            stmBarFill.color = stmBarFillColor;
            stmBarFill.DOColor(Color.white, 0.15f).SetLoops(2, LoopType.Yoyo);
        }
    }


    void GameOver()
    {
        isGameover = true;
        hud.SetActive(false);
        gameover.SetActive(true);
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
        SetHp(playerStatus.hp.Value);
        SetOh(playerStatus.overheat.Value);
        SetStm(playerStatus.stamina.Value);
        SetStatText();
    }

    void SetStatText()
    {
        statText.text = "HP : " + playerStatus.hp.Value + "\nMaxHP : " + playerStatus.hp.maxValue + "\nOverheat : " + playerStatus.overheat.Value +
            "\nMaxOverheat : " + playerStatus.overheat.maxValue + "\nStemina : " + playerStatus.stamina.Value + "\nMaxStemina : " + playerStatus.stamina.maxValue;
    }

    #endregion
}

