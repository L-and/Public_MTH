using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Slider hpBar;
    public Slider ohBar;
    public TextMeshProUGUI ohBarText;
    public Slider stmBar;

    float hp = 100f;
    float maxHp = 100f;
    float oh = 100f;
    float maxOh = 100f;
    float stm = 1;
    float maxStm = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StatusReset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetHp(hp + 1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetHp(hp - 1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetOh(oh + 1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetOh(oh - 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SetStm(0);
        }
        if (stm < 1) SetStm(stm + 0.5f * Time.deltaTime);
    }
    void SetHp(float value)
    {
        hp = Mathf.Clamp(value, 0, maxHp);
        if (hp > maxHp) hp = maxHp;
        hpBar.maxValue = maxHp;
        hpBar.value = hp;
    }
    void SetOh(float value)
    {
        oh = Mathf.Clamp(value, 0, maxOh);
        ohBar.maxValue = maxOh;
        ohBarText.text = oh + "%";
        ohBar.value = oh;
    }
    void SetStm(float value)
    {
        stm = Mathf.Clamp(value, 0, maxStm);
        if (stm > maxStm) hp = maxHp;
        stmBar.maxValue = maxStm;
        stmBar.value = stm;
    }
    
    void StatusReset()
    {
        SetHp(maxHp);
        SetOh(maxOh);
        SetStm(maxStm);
    }
}
