using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public TextMeshProUGUI title;
    public RectTransform buttons;
    public GameObject options;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void Quit()
    {
        Application.Quit();
    }
}
