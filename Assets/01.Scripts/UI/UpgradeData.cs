using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Object/UpgradeData", order = 31)]
public class UpgradeData : ScriptableObject
{
    Dictionary<string, string> upgrade1 = new Dictionary<string, string>()
    {
        {"name", "방탄조끼" },
        {"info","체력 추가" },
        {"flavor", "미리 보험을 들어두는 것도 나쁘지 않죠" }
    };
}
