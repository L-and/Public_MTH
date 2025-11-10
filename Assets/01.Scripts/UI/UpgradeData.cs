using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Object/UpgradeData", order = 31)]
public class UpgradeData : ScriptableObject
{
    [SerializedDictionary("ID", "Info")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<int, Info> Upgrade;

    [System.Serializable]
    public class Info
    {
        public string Name;
        public string Type;
        public string Feature;
        public string Flavour;
        public string Effect;  //연주 -  효과 스크립트
        public Sprite Icon;
    }
}
