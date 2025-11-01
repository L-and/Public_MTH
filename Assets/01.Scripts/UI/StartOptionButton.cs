using DG.Tweening;
using UnityEngine;

public class StartOptionButton : MonoBehaviour
{
    private StartOptionManager startOption;
    public GameObject arrow;
    public enum Type { Weapon, Side, Emission };

    [SerializeField] private string textName = "";
    [SerializeField] private string textFeature = "";
    [SerializeField] private Type type = Type.Weapon;
    [SerializeField] private int id;


    void Start()
    {
        startOption = transform.parent.parent.GetComponent<StartOptionManager>();
    }

    public void OnPressed()
    {
        startOption.SetText(textName, textFeature);
        arrow.transform.DOMoveX(transform.position.x, 0.3f);
        switch(type)
        {
            case Type.Weapon:
                GameManager.GameData.SetCurrentWeaponID(id); break;
            case Type.Side:
                GameManager.GameData.SetCurrentSideWeaponID(id); break;
            case Type.Emission:
                GameManager.GameData.SetCurrentEmissionID(id); break;

        }
    }
}
