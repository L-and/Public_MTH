using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UpgradeUIManager : MonoBehaviour
{
    public RectTransform upgrades;
    public TextMeshProUGUI name;
    public TextMeshProUGUI info;
    public TextMeshProUGUI flavor;
    public UpgradeData upgradeData;
    public GameObject upgradeUiPrefab;
    public UpgradeButton upgradeButton;
    public UpgradeButton upgradeButton2;
    public UpgradeButton upgradeButton3;
    int dataId;
    int dataId2;
    int dataId3;
    int dataIdMax = 21;

    public int upgradeChance = 1;

    private void Start()
    {
        // 0~21 id랜덤 제시 ->  UpgradeUI 반영
        SetDataID();
    }

    public void SetDataID()
    {
        CreateDataIDValue();
        upgradeButton.dataId = dataId;
        upgradeButton2.dataId = dataId2;
        upgradeButton3.dataId = dataId3;
    }
    void CreateDataIDValue()
    {
        dataId = Random.Range(0, dataIdMax);
        do
        {
            dataId2 = Random.Range(0, dataIdMax);
        } while (dataId2 == dataId);
        do
        {
            dataId3 = Random.Range(0, dataIdMax);
        } while (dataId3 == dataId || dataId3 == dataId2);
    }

    public void UpgradesReActive()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void SetUpgradeText(int order)
    {
        name.text = upgradeData.Upgrade[order].Name;
        info.text = upgradeData.Upgrade[order].Feature;
        flavor.text = upgradeData.Upgrade[order].Flavour;
    }

    bool CheckEVHacker()
    {
        return true;
    }
}