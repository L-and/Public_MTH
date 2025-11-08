using UnityEngine;
using UnityEngine.UI;

public class CommonButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnPressed);
    }
    void OnPressed()
    {
        GameManager.Sound.PlaySFX("Button_Click(Paid)");
    }

}
