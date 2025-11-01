using _01.Scripts.Manager;
using UnityEngine;

public class HitSource : MonoBehaviour
{
    public float damage = 10f;
    public float GetDamage()
    {
        return GameManager.PlayerManager.PlayerStatus.bulletDamage;
    }
    // 필요하면 owner/팀/치명타 여부 등 확장
}