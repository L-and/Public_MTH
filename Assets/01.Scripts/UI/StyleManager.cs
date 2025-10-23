using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StyleManager : MonoBehaviour
{
    private Canvas targetCanvas;
    [SerializeField] private GameObject textGrid;
    [SerializeField] private TextMeshProUGUI combo;
    [SerializeField] private Slider comboBar;
    public TMP_FontAsset font;
    public StyleData styleData;
    public UIManager uiManager;

    public int comboes = 0;

    private bool comboStopped = false;

    Dictionary<string, float> styleList = new Dictionary<string, float>()
    {
        {"처치", 3 },
        {"대형 처치", 8 },
        {"회피", 6 }
    };


    void Start()
    {
        targetCanvas = FindFirstObjectByType<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CreateNewStyle(0);
            SetCombo(comboes + 1);
            RestoreComboBar();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            CreateNewStyle(1);
            SetCombo(comboes + 1);
            RestoreComboBar();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CreateNewStyle(2);
            SetCombo(comboes + 1);
            RestoreComboBar();
        }

        DecreaseComboBar();
    }
    public TextMeshProUGUI CreateText(string name, string text, float size, Color color)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(textGrid.transform, true);
        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
        DestroyTimer script = tmpText.AddComponent<DestroyTimer>();
        rectTransform.sizeDelta = new Vector2(400, 100);

        tmpText.text = text;                
        tmpText.fontSize = size;           
        tmpText.color = color;              
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.font = font;

        script.time = 3f;
        

        return tmpText;
    }

    public void CreateNewStyle(int id)
    {
        string name = styleData.Style[id].Name;
        float value = styleData.Style[id].Value;
        Color color = styleData.Style[id].Color;

        uiManager.SetOh(uiManager.oh + value);
        CreateText("style"," "+ name +" +"+ value + "%", 36, color);

        if (textGrid.transform.childCount > 7)
        {
            RemoveLatestStyle();
        }
    }

    public void RemoveLatestStyle()
    {
        Destroy(textGrid.transform.GetChild(0).gameObject);
    }

    public void SetCombo(int value)
    {
        if (comboes < value)
        {
            DOTween.Kill(combo.rectTransform);
            combo.rectTransform.localScale = new Vector2(1, 1);
            combo.rectTransform.DOScale(new Vector2(1.1f, 1.1f), 0.1f).SetLoops(2, LoopType.Yoyo);
        }
        comboes = value;
        combo.text = value + "x";
    }
    public void DecreaseComboBar()
    {
        comboBar.value -= 1 * Time.deltaTime;
        if (comboBar.value <= 0 && !comboStopped)
        {
            SetCombo(0);
            comboStopped = true;
        }

    }
    public void RestoreComboBar()
    {
        comboStopped = false;
        comboBar.value = comboBar.maxValue;
    }
}
