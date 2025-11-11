using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameOverManager : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI kills;
    public TextMeshProUGUI killsValue;
    public TextMeshProUGUI time;
    public TextMeshProUGUI timeValue;
    public TextMeshProUGUI styles;
    public TextMeshProUGUI stylesValue;
    public TextMeshProUGUI score;
    public Button menuButton;

    private int scoreValue;

    private void OnEnable()
    {
        background.DOFade(0.2f, 0f);
        background.DOFade(1f, 8f);
        GlobalMethod.Fade(gameObject, 5f);
//        StartCoroutine(ShowScore());
    }

//    IEnumerator ShowScore()
//    {
//        SetScore(23, 2300, 600, false);
//        yield return new WaitForSeconds(2.5f);
//        kills.gameObject.SetActive(true);
//        yield return new WaitForSeconds(0.7f);
//        styles.gameObject.SetActive(true);
//        yield return new WaitForSeconds(0.7f);
//        time.gameObject.SetActive(true);
//        yield return new WaitForSeconds(0.7f);
//        score.gameObject.SetActive(true);

//        for (int i = 0; i <= scoreValue; i += (scoreValue - i == 1) ? 1 : 2)
//        {
//            score.text = "Á¡¼ö : " + i.ToString();
//            yield return new WaitForSeconds(0.000001f);
//        }

//        yield return new WaitForSeconds(0.3f);
//        menuButton.gameObject.SetActive(true);
//    }

    public void SetScore(int killsValue, int stylesValue, int timeValue, bool isBossKilled)
    {
        TimeSpan displayTime = TimeSpan.FromSeconds(timeValue);
        string timeFormat = string.Format("{0:00}:{1:00}", displayTime.Minutes, displayTime.Seconds);

        kills.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = killsValue.ToString();
        time.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = timeFormat;
        styles.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stylesValue.ToString();

        ScoreFormula(stylesValue,timeValue, isBossKilled);

    }
    private void ScoreFormula(int style, int time, bool bosskill)
    {
        int bossBonus = bosskill ? 1000 : 0;
        scoreValue = style + time * 5 + bossBonus;
        
    }
}
