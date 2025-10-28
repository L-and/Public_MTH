using System;
using _01.Scripts.PlayerControll.Status;
using UnityEngine;

namespace _01.Scripts.Emission
{
    [RequireComponent(typeof(PlayerStatus)), Serializable]
    public class PlayerEmission : MonoBehaviour
    {
        [Header("방출공격 위치")] public Transform firePosition;
        
        public PlayerStatus Status { get; private set; }

        private void Awake()
        {
            Status = GetComponent<PlayerStatus>();
        }

        /// <summary>
        /// GameManager에 설정된 방출을 실행합니다 // TODO 임시 data
        /// </summary>
        public void ExecuteEmission(EmissionAbilityData data)
        {
            // TODO GameManager에서 EmissionAbilityData 가져오는것 추가

            EmissionAbilityData emissionData = data;
            if (emissionData.behavior != null)
            {
                emissionData.behavior.Execute(this, emissionData);
            }
            else
            {
                Debug.Log($"{emissionData.name}에 Behavior이 할당되지 않았습니다.");
            }
        }
        
    }
}
