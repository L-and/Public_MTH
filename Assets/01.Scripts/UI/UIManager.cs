using _01.Scripts.PlayerControll.Status;
using DG.Tweening;
using System.Collections;
using _01.Scripts.PlayerControll;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Michsky.MUIP;

public class UIManager : MonoBehaviour
{

  public GameObject hud;
  public GameObject gameover;
  public GameObject gameWon;
  public ProgressBar hpBar;
  public ProgressBar ohBar;
  public TextMeshProUGUI ohBarText;
  public ProgressBar stmBar;
  public Image hpBarFill;
  public Image ohBarFill;
  public Image stmBarFill;
  public TextMeshProUGUI levelInfo;
  public GameObject pauseMenu;
  private AudioSource ohFullKeepSound;
  public TextMeshProUGUI statText;
  public Image upgradeSlot1;
  public Image upgradeSlot2;
  public Image upgradeSlot3;


  Color hpBarFillColor;
  Color ohBarFillColor;
  Color stmBarFillColor;

  private PlayerStat PlayerStat => GameManager.PlayerManager?.PlayerStat;
  private PlayerController PlayerController => GameManager.PlayerManager?.PlayerController;

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
    ohFullKeepSound = ohBar.GetComponent<AudioSource>();
    if (!GameManager.PlayerManager)
    {
      Debug.LogWarning("GameManager.PlayerManager가 정의되지 않았습니다!");
    }

    UpdateStatus();
  }

  #region 보스가 죽었을 경우 발생할 이벤트 등록 및 해제
  void OnEnable()
  {
    BossPhase.OnBossDefeated += GameWon;
  }

  void OnDisable()
  {
    BossPhase.OnBossDefeated -= GameWon;
  }
  #endregion

  // Update is called once per frame
  void Update()
  {

    // 플레이어 게임오버 처리 TODO 수정필요
    if (Input.GetKeyDown(KeyCode.G) && !isGameover)
    {
      GameWon();
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Pause();
    }

    // PlayerStat값에 맞게 UI 업데이트
    UpdateStatus();
  }
  #region METHOD
  public void SetHp(float value)
  {
    if (value > PlayerStat.hp.Value)
    {
      DOTween.Kill(hpBarFill);
      hpBarFill.color = hpBarFillColor;
      hpBarFill.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
    }
    PlayerStat.hp.Value = Mathf.Clamp(value, 0, PlayerStat.hp.maxValue);
    hpBar.maxValue = PlayerStat.hp.maxValue;
    hpBar.currentPercent = PlayerStat.hp.Value;
  }
  public void SetOh(float value)
  {
    if (PlayerStat.overheat.Value < value)
    {
      GameManager.Sound.PlaySFX("Overheat_Up");
    }

    PlayerStat.overheat.Value = value;
    ohBar.maxValue = PlayerStat.overheat.maxValue;
    ohBarText.text = PlayerStat.overheat.Value + "%";
    ohBar.currentPercent = PlayerStat.overheat.Value;
    Color color2;
    ColorUtility.TryParseHtmlString("#FF8D00", out color2);
    ohBarFill.color = Color.Lerp(ohBarFillColor, color2, PlayerStat.overheat.Value / 100);

    if (ohFulled && PlayerStat.overheat.Value < PlayerStat.overheat.maxValue)
    {
      ohFulled = false;
      DOTween.Kill(ohBarText.rectTransform);
      ohBarText.rectTransform.localScale = Vector3.one;
      ohFullKeepSound.Stop();
    }
    if (!ohFulled && PlayerStat.overheat.Value >= PlayerStat.overheat.maxValue)
    {
      ohFulled = true;
      GameManager.Sound.PlaySFX("Overheat_Full");
      ohFullKeepSound.Play();
      DOTween.Kill(ohBarFill);
      ohBarFill.color = color2;
      ohBarFill.DOColor(Color.white, 0.8f).SetLoops(2, LoopType.Yoyo);
      ohBarText.rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.4f).SetLoops(-1, LoopType.Yoyo);
    }
  }
  public void SetStm(float value)
  {
    PlayerStat.stamina.Value = value;
    stmBar.maxValue = PlayerStat.stamina.maxValue;
    stmBar.currentPercent = PlayerStat.stamina.Value;

    if (PlayerStat.stamina.Value < PlayerStat.stamina.maxValue) stmFulled = false;
    if (!stmFulled && PlayerStat.stamina.Value >= PlayerStat.stamina.maxValue)
    {
      stmFulled = true;
      GameManager.Sound.PlaySFX("Stemina_Full");
      DOTween.Kill(stmBarFill);
      stmBarFill.color = stmBarFillColor;
      stmBarFill.DOColor(Color.white, 0.15f).SetLoops(2, LoopType.Yoyo);
    }
  }

  public void SetUpgradeSlot()
  {
    Image[] upgradeSlots = new Image[3] { upgradeSlot1, upgradeSlot2, upgradeSlot3 };

    for (int i = 0; i < upgradeSlots.Length; i++)
    {
      if (i <= PlayerStat.UpgradeNumber - 1) upgradeSlots[i].gameObject.SetActive(true);
      else upgradeSlots[i].gameObject.SetActive(false);
    }


  }


  public void GameOver()
  {
    isGameover = true;
    GameManager.Sound.PlayMusic("BGM_Game_Over");
    hud.SetActive(false);
    gameover.SetActive(true);
  }

  public void GameWon()
  {
    hud.SetActive(false);
    gameWon.SetActive(true);
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
        SetHp(PlayerStat.hp.Value);
        SetOh(PlayerStat.overheat.Value);
        SetStm(PlayerStat.stamina.Value);
        SetUpgradeSlot();
    }
    
    void SetStatText()
    {
      PlayerController.ActivePlayerInput();
      pauseMenu.SetActive(false);
      Time.timeScale = 1;
      isPause = false;
      return;
    }
  }

  void UpdateStatus()
  {
    SetHp(PlayerStat.hp.Value);
    SetOh(PlayerStat.overheat.Value);
    SetStm(PlayerStat.stamina.Value);
    SetStatText();
    SetUpgradeSlot();
  }

  void SetStatText()
  {
    statText.text = "HP : " + PlayerStat.hp.Value + "\nMaxHP : " + PlayerStat.hp.maxValue + "\nOverheat : " + PlayerStat.overheat.Value +
        "\nMaxOverheat : " + PlayerStat.overheat.maxValue + "\nStemina : " + PlayerStat.stamina.Value + "\nMaxStemina : " + PlayerStat.stamina.maxValue;
  }

  #endregion
}

