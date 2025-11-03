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

    Color hpBarFillColor;
    Color ohBarFillColor;
    Color stmBarFillColor;
    PlayerStat _playerStat;
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
            _playerStat = GameObject.FindWithTag("Player").GetComponent<PlayerStat>();
            UpdateStatus();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_playerStat.stamina.Value >= 1)
            {
                SetStm(_playerStat.stamina.Value - 1);
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

        if (_playerStat != null)
        {   
            if (_playerStat.stamina.Value < _playerStat.stamina.maxValue) SetStm(_playerStat.stamina.Value + 0.5f * Time.deltaTime);
            UpdateStatus();
        }
    }
    #region METHOD
    public void SetHp(float value)
    {
        if (value > _playerStat.hp.Value)
        {
            DOTween.Kill(hpBarFill);
            hpBarFill.color = hpBarFillColor;
            hpBarFill.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
        _playerStat.hp.Value = Mathf.Clamp(value, 0, _playerStat.hp.maxValue);
        hpBar.maxValue = _playerStat.hp.maxValue;
        hpBar.value = _playerStat.hp.Value;
    }
    public void SetOh(float value)
    {
        _playerStat.overheat.Value = _playerStat.overheat.maxValue;
        ohBar.maxValue = _playerStat.overheat.maxValue;
        ohBarText.text = _playerStat.overheat.Value + "%";
        ohBar.value = _playerStat.overheat.Value;
        Color color2;
        ColorUtility.TryParseHtmlString("#FF8D00", out color2);
        ohBarFill.color = Color.Lerp(ohBarFillColor, color2, _playerStat.overheat.Value / 100);

        if (_playerStat.overheat.Value < _playerStat.overheat.maxValue)
        { 
            ohFulled = false;
            DOTween.Kill(ohBarText.rectTransform);
            ohBarText.rectTransform.localScale = Vector3.one;
        }
        if (!ohFulled && _playerStat.overheat.Value >= _playerStat.overheat.maxValue)
        {
            ohFulled = true;
            DOTween.Kill(ohBarFill);
            ohBarFill.color = color2;
            ohBarFill.DOColor(Color.white, 0.8f).SetLoops(2, LoopType.Yoyo);
            ohBarText.rectTransform.DOScale( new Vector3(1.2f,1.2f,1), 0.4f).SetLoops(-1, LoopType.Yoyo);
        }
    }
    public void SetStm(float value)
    {
        _playerStat.stamina.Value = value;
        stmBar.maxValue = _playerStat.stamina.maxValue;
        stmBar.value = _playerStat.stamina.Value;

        if (_playerStat.stamina.Value < _playerStat.stamina.maxValue) stmFulled = false;
        if (!stmFulled && _playerStat.stamina.Value >= _playerStat.stamina.maxValue)
        {
            stmFulled = true;
            DOTween.Kill(stmBarFill);
            stmBarFill.color = stmBarFillColor;
            stmBarFill.DOColor(Color.white, 0.15f).SetLoops(2, LoopType.Yoyo);
        }
    }


    public void GameOver()
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
        SetHp(_playerStat.hp.Value);
        SetOh(_playerStat.overheat.Value);
        SetStm(_playerStat.stamina.Value);
    }

    #endregion
}

