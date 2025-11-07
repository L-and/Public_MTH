using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
  public TextMeshProUGUI title;
  public RectTransform buttons;
  public GameObject options;
    public GameObject startOption;
  // Start is called once before the first execution of Update after the MonoBehaviour is created

 
  private void Start()
  {
    ButtonsOn();
        StartCoroutine(PlayMainMusic());
  }

  public void ButtonsOn()
  {
    buttons.DOMoveY(0, 0.2f);
  }
  public void ButtonsOff()
  {
    buttons.DOMoveY(-235, 0.2f);
  }
  public void OpenOptions()
  {
    ButtonsOff();
    options.SetActive(true);
  }
  public void CloseOptions()
  {
    ButtonsOn();
    options.SetActive(false);
  }
    public void OpenStartOption()
    {
        ButtonsOff();
        startOption.SetActive(true);
    }
    public void CloseStartOption()
    {
        ButtonsOn();
        startOption.SetActive(false);
    }

    IEnumerator PlayMainMusic()
    {
        yield return new WaitForSeconds(3);
        GameManager.Sound.PlayMusic("bgm_cold_dawn");
    }


    public void Quit()
  {
    Application.Quit();
  }
}
