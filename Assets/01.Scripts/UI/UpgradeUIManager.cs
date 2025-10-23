using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UpgradeUIManager : MonoBehaviour
{
    Dictionary<string, string> upgrade1 = new Dictionary<string, string>()
    {
        {"name", "방탄조끼" },
        {"info","체력 추가" },
        {"flavor", "미리 보험을 들어두는 것도 나쁘지 않죠" }
    };
    Dictionary<string, string> upgrade2 = new Dictionary<string, string>()
    {
        {"name", "급속 온열기" },
        {"info","과열 속도 증가" },
        {"flavor", "좀 더 뜨겁게, 좀 더 화려하게" }
    };
    Dictionary<string, string> upgrade3 = new Dictionary<string, string>()
    {
        {"name", "부스터 2.0" },
        {"info","스테미나 증가" },
        {"flavor", "달까지도 날아갈 수 있습니다" }
    };

    public RectTransform upgrades;
    public TextMeshProUGUI name;
    public TextMeshProUGUI info;
    public TextMeshProUGUI flavor;


    public void SetUpgradeText(int order)
    {
        Dictionary<string,string>[] arrayUpgrades = new Dictionary<string,string>[3];
        arrayUpgrades[0] = upgrade1;
        arrayUpgrades[1] = upgrade2;
        arrayUpgrades[2] = upgrade3;

        name.text = arrayUpgrades[order]["name"];
        info.text = arrayUpgrades[order]["info"];
        flavor.text = arrayUpgrades[order]["flavor"];
    }

}