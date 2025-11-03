using DG.Tweening;
using System.Collections;
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
    private Vector2 originPosition;

    [SerializeField] private int order;
    private bool isTouchable = false;

    private Tween scaleTween;
    private int dataId;

    void Awake()
    {
        originPosition = transform.localPosition;
        upgradeUiCanvasGroup = upgradeUi.GetComponent<CanvasGroup>();
        upgradeUiManager = upgradeUi.GetComponent<UpgradeUIManager>();
        button = GetComponent<Button>();
    }
    void OnEnable()
    {
        transform.localPosition = originPosition;

        SetDataIDRandomly();
        if (order == 0) CompareDuplicate();
        UpgradeOn();
        GetComponent<Image>().DOFade(1, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isTouchable)
        {
            transform.DOScale(1, 0);
            scaleTween = transform.DOScale(1.2f, 0.2f);
            upgradeUiManager.SetUpgradeText(dataId); // UpgradeUI는 이쪽으로 표시됨
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
                upgradeUiCanvasGroup.DOFade(1, 1.2f).OnComplete(() => upgradeUiCanvasGroup.DOFade(0, 1)).OnComplete(()
                => OnObjectDisable());
            });

            for (int i = 0; i < 3; i++)
            {
                if (order == i) continue;
                Transform child = transform.parent.GetChild(i);
                child.GetComponent<Image>().DOFade(0, 0.2f).OnComplete(() => child.gameObject.SetActive(false));
            }

            // 연주 - ElevatorUpgradeManager로 연동
            var manager =Object.FindFirstObjectByType<ElevatorUpgradeManager>();
            if (manager != null)
            {
                manager.ApplyUpgrade(dataId);
                Debug.Log($"[DEBUG] 업그레이드 적용 시도: ID {dataId}");
            }
            else
            {
                Debug.Log("ElevatorUpgradeManager를 찾을 수 없습니다!");
            }
        }
    }


    void UpgradeOn()
    {
        transform.DOLocalMoveY(0, 1).OnComplete(() => SetIstouchable(true));
    }
    void SetIstouchable(bool value) 
    { 
        isTouchable = value;
    }

    void OnObjectDisable()
    {
        if (CheckEVHacker() && upgradeUiManager.upgradeChance > 0)
        {   
            upgradeUiManager.upgradeChance--;
            upgradeUiManager.UpgradesReActive();
        }
        else upgradeUi.SetActive(false);
    }

    bool CheckEVHacker()
    {
        //Todo 업그레이드 슬롯에 엘리베이터 해커 확인
        return true;
    }

    void SetDataIDRandomly()
    {
        // 데이터 아이디 0~20 설정 ->  id를 UpgradeUI 에 가져가서 텍스트 변경 설정
        dataId = Random.Range(0, 21);
    }

    void CompareDuplicate()
    {
        int dataId1 = transform.parent.GetChild(0).GetComponent<UpgradeButton>().dataId;
        int dataId2 = transform.parent.GetChild(1).GetComponent<UpgradeButton>().dataId;
        int dataId3 = transform.parent.GetChild(2).GetComponent<UpgradeButton>().dataId;
        if (dataId1 == dataId2)
        {
            transform.parent.GetChild(0).GetComponent<UpgradeButton>().SetDataIDRandomly();
            CompareDuplicate();
        }
        if (dataId2 == dataId3)
        {
            transform.parent.GetChild(1).GetComponent<UpgradeButton>().SetDataIDRandomly();
            CompareDuplicate();
        }
        if (dataId1 == dataId3)
        {
            transform.parent.GetChild(2).GetComponent<UpgradeButton>().SetDataIDRandomly();
            CompareDuplicate();
        }
    }

}
