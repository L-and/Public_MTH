using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject hud;
    public GameObject gameover;
    public Slider hpBar;
    public Slider ohBar;
    public TextMeshProUGUI ohBarText;
    public Slider stmBar;
    public Animation ohAnim;
    public Animation stmAnim;

    float hp = 100f;
    float maxHp = 100f;
    float oh = 100f;
    float maxOh = 100f;
    float stm = 1;
    float maxStm = 1;
    bool ohFulled = false;
    bool stmFulled = false; 

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
            if (stm >= 1)
            {
                stmFulled = false;
                SetStm(stm - 1);
            }
        }
        if (stm < 1) SetStm(stm + 0.5f * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.G))
        {
            GameOver();
        }
    }
    void SetHp(float value)
    {
        hp = Mathf.Clamp(value, 0, maxHp);
        hpBar.maxValue = maxHp;
        hpBar.value = hp;
    }
    void SetOh(float value)
    {
        oh = Mathf.Clamp(value, 0, maxOh);
        ohBar.maxValue = maxOh;
        ohBarText.text = oh + "%";
        ohBar.value = oh;
        if (oh < maxOh)
        {
            Color color; 
            ColorUtility.TryParseHtmlString("#D6D21D", out color);
            ohBar.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().color = color;
            ohFulled = false;
        }
        if (!ohFulled && oh >= maxOh)
        {
            ohFulled = true;
            ohAnim.Play();
            Debug.Log("aa");
        }
    }
    void SetStm(float value)
    {
        stm = Mathf.Clamp(value, 0, maxStm);
        stmBar.maxValue = maxStm;
        stmBar.value = stm;


        if (!stmFulled && stm >= maxStm)
        {
            stmFulled = true;
            stmAnim.Play();
        }
    }
    
    void StatusReset()
    {
        SetHp(maxHp);
        SetOh(maxOh);
        SetStm(maxStm);
    }

    void GameOver()
    {
        hud.SetActive(false);
        gameover.SetActive(true);
    }

    void Restart
}
