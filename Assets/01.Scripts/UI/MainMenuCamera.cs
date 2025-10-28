using UnityEngine;
using DG.Tweening;

public class MainMenuCamera : MonoBehaviour
{
    public GameObject mainCamera;
    bool diveStarted = false;

    public GameObject canvas;
    private MainMenuUIManager mainMenuUiManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuUiManager = canvas.GetComponent<MainMenuUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!diveStarted) transform.Rotate(0, 20 * Time.deltaTime, 0);
    }

    public void DiveInHole()
    {
        diveStarted = true;
        mainCamera.transform.DOLocalRotate(new Vector3(90,0,0), 1.5f);
        mainCamera.transform.DOMove(Vector3.zero, 1.75f).OnComplete(() => mainMenuUiManager.MoveScene());
        GlobalMethod.Fade(canvas, 1.75f, false, false);
    }
}
