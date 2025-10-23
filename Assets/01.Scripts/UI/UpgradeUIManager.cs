using DG.Tweening;
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


    public void SetUpgradeText(int order)
    {
        name.text = upgradeData.Upgrade[order].Name;
        info.text = upgradeData.Upgrade[order].Feature;
        flavor.text = upgradeData.Upgrade[order].Flavour;
    }

}