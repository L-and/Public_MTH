using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.Emission
{
    public class PlayerEmission : MonoBehaviour
    {
        [SerializeField] public EmissionAbilityData data; // 방출 속성정보들
        
        [Header("방출공격 위치")] public Transform firePosition;
        
        [Header("방출 장착 위치")] [SerializeField]
        private Transform equipPosition;
        
        public PlayerStat Stat { get; private set; }
        public Transform PlayerCamera { get; private set; }

        private GameObject _emissionGO;
        public GameObject EmissionGO => _emissionGO;
        
        /// <summary>
        /// 선택된 방출SO를 사용해 초기설정을 진행하는 메서드
        /// </summary>
        public void InitializeEmission(PlayerController playerController)
        {
            Stat = GameManager.PlayerManager.PlayerStat;
            PlayerCamera = playerController.PlayerCamera;
            
            // 방출무기 모델 장착위치 설정
            if (equipPosition == null)
            {
                equipPosition = GameObject.FindGameObjectWithTag("LeftHand Position").transform;
            }
            
            // 방출무기 모델 생성 및 비활성화 (사용할때만 활성화하는 식으로 사용)
            _emissionGO = Instantiate(data.prefab, equipPosition);
            
            _emissionGO.SetActive(false);
            
            // 방출 사격위치 설정
            if (firePosition == null)
            {
                firePosition = _emissionGO.transform.Find("Fire Position");
            }
            
            if (!firePosition) Debug.LogWarning($"[방출:{data.name}] 발사위치 설정안됨! (이름: \"Fire Position\"의 오브젝트를 방출 프리팹에 추가하시오)");
        }

        /// <summary>
        /// GameManager에 설정된 방출을 실행합니다
        /// </summary>
        public void ExecuteEmission(EmissionAbilityData data)
        {

            EmissionAbilityData emissionData = data;
            if (emissionData.behavior != null)
            {
                emissionData.behavior.Execute(this, PlayerCamera, emissionData);
            }
            else
            {
                Debug.Log($"{emissionData.name}에 Behavior이 할당되지 않았습니다.");
            }
        }

        /// <summary>
        /// 방출의 모델 활성화를 설정하는 메서드
        /// </summary>
        /// <param name="isActive"></param>
        public void ModelSetActive(bool isActive)
        {
            _emissionGO.SetActive(isActive);
        }
    }
}
