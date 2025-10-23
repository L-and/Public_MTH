using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private UpgradeUIManager upgradeUiManager;
    private CanvasGroup upgradeUiCanvasGroup;
    public GameObject upgradeUi;
    private Button button;

    [SerializeField] private int order;
    private bool isTouchable = false;

    private Tween scaleTween;

    void Start()
    {
        upgradeUiCanvasGroup = upgradeUi.GetComponent<CanvasGroup>();
        upgradeUiManager = upgradeUi.GetComponent<UpgradeUIManager>();
        button = GetComponent<Button>();
        UpgradeOn();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isTouchable)
        {
            transform.DOScale(1, 0);
            scaleTween = transform.DOScale(1.2f, 0.2f);
            upgradeUiManager.SetUpgradeText(order);
        }
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        scaleTween.Kill(transform);
        transform.DOScale(1, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isTouchable)
        {
            SetIstouchable(false);
            GlobalMethod.Fade(upgradeUi,0.4f);
            transform.DOLocalMoveX(0, 0.5f).OnComplete(() =>
            {
                upgradeUiCanvasGroup.DOFade(1, 1.2f).OnComplete(() => upgradeUiCanvasGroup.DOFade(0, 1));
            });

            for (int i = 0; i < 3; i++)
            {
                if (order == i) continue;
                Transform child = transform.parent.GetChild(i);
                child.GetComponent<Image>().DOFade(0, 0.2f).OnComplete(() => child.gameObject.SetActive(false));
            }
        }
    }


    void UpgradeOn()
    {
        transform.DOLocalMoveY(650, 0);
        transform.DOLocalMoveY(0, 0.6f).OnComplete(() => SetIstouchable(true));
    }
    void SetIstouchable(bool value) 
    { 
        isTouchable = value;
    }
}
