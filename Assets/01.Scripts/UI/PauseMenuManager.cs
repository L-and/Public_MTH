using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuManager : MonoBehaviour
{
    
    public GameObject buttons;
    public GameObject options;
    private void OnEnable()
    {
        buttons.transform.DOScaleY(0.8f, 0).SetUpdate(true);
        buttons.transform.DOScaleY(1, 0.2f).SetUpdate(true);
    }
    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenOptions()
    {
        options.SetActive(true);
    }
    public void CloseOptions()
    {
        options.SetActive(false);
    }

    public void ReturnToMainMenu()
    {

        SceneManager.LoadScene("MainMenu");
    }
}
