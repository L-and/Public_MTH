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
    public TextMeshProUGUI levels;
    public TextMeshProUGUI levelsValue;
    public TextMeshProUGUI score;
    public Button restartButton;
    public Button menuButton;

    private int scoreValue;

    private void OnEnable()
    {
        background.DOFade(0.2f, 0f);
        background.DOFade(1f, 8f);
        GlobalMethod.Fade(gameObject, 5f);
        StartCoroutine(ShowScore());
    }

    IEnumerator ShowScore()
    {
        SetScore(23, 610, 4);
        yield return new WaitForSeconds(3);
        kills.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        time.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        levels.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        score.gameObject.SetActive(true);

        for (int i = 0; i <= scoreValue; i++)
        {
            score.text = "Á¡¼ö : " + i.ToString();
            yield return new WaitForSeconds(0.003f);
        }

        yield return new WaitForSeconds(0.3f);
        restartButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
    }

    public void SetScore(int killsValue, float timeValue, int levelsValue)
    {
        TimeSpan displayTime = TimeSpan.FromSeconds(timeValue);
        string timeFormat = string.Format("{0:00}:{1:00}", displayTime.Minutes, displayTime.Seconds);

        kills.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = killsValue.ToString();
        time.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = timeFormat;
        levels.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = levelsValue.ToString();

        ScoreFormula();

    }
    private void ScoreFormula()
    {
        scoreValue = 1222;
    }
}
