using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public GameObject gamePlay;
    public GameObject control;
    public GameObject video;
    public GameObject audio;

    void OnEnable()
    {
        ShowGamePlay();
        transform.DOScaleY(0.8f, 0).SetUpdate(true);
        transform.DOScaleY(1, 0.2f).SetUpdate(true);
    }
    private void OnDisable()
    {
        gameObject.SetActive(false);
    }


    public void ShowGamePlay()
    {
        Reset();
        gamePlay.SetActive(true);
    }
    public void ShowControl()
    {
        Reset();
        control.SetActive(true);
    }
    public void ShowVideo()
    {
        Reset();
        video.SetActive(true);
    }
    public void ShowAudio()
    {
        Reset();
        audio.SetActive(true);
    }

    private void Reset()
    {
        gamePlay.SetActive(false);
        control.SetActive(false);  
        video.SetActive(false);
        audio.SetActive(false);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void OnFullscreenToggleClicked(bool value)
    {
        Screen.fullScreen = value;
    }

}
