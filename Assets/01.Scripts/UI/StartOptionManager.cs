using DG.Tweening;
using TMPro;
using UnityEngine;

public class StartOptionManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI featureText;
    void OnEnable()
    {
        transform.DOScaleY(0.8f, 0).SetUpdate(true);
        transform.DOScaleY(1, 0.2f).SetUpdate(true);
    }

    public void SetText(string name, string feature)
    {
        nameText.text = name; 
        featureText.text = feature;
    }
}
