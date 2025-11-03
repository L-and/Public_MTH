using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.SubWeapon
{
    // TODO 애니메이션의 시작/진행중/종료 실행시간을 SO에서 조작가능하도록 설계하면 좋을듯 함
    
    /// <summary>
    /// 보조무기의 속성들/동작방식을 작성하는 SO클래스
    /// </summary>
    [CreateAssetMenu(fileName = "New SubWeaponData", menuName = "SubWeapon/SubWeapon Data")]
    public class SubWeaponData : ScriptableObject
    {
        [Header("기본정보")] 
        public string subWeaponName;
        public float useDuration; // 사용 지속시간
        public float coolDown;
        [TextArea] public string description;
        public SubWeaponBehavior behavior;

        [Header("프리팹")] 
        public GameObject prefab;
    }
}