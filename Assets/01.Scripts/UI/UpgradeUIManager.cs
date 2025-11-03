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

    public int upgradeChance = 1;

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