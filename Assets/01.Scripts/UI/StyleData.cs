using AYellowpaper.SerializedCollections;
using UnityEngine;
[CreateAssetMenu(fileName = "StyleData", menuName = "Scriptable Object/StyleData", order = 31)]
public class StyleData : ScriptableObject
{
    [SerializedDictionary("ID", "Info")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<int, Info> Style;

    [System.Serializable]
    public class Info
    {
        public string Name;
        public float Value;
        public Color Color = Color.white;
    }
}

