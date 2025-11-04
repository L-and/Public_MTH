using UnityEngine;

namespace _01.Scripts.PlayerControll
{
    /// <summary>
    /// 마우스움직임에 따른 화면회전을 적용하는 스크립트
    /// </summary>
    public class AimLook : MonoBehaviour
    {
        private PlayerController _pc;

        [Header("회전기준 트랜스폼")] 
        [SerializeField] private Transform xTransform;
        [SerializeField] private Transform yTransform;

        private float _xRotValue;
        private float _yRotValue;

        private void Awake()
        {
            xTransform = transform;
            yTransform = transform.root;
        
            _pc = yTransform.GetComponent<PlayerController>();
        }

        private void LateUpdate()
        {
            // 마우스움직임으로 회전 적용
            var mouseX = _pc.MouseDeltaInput.x * _pc.Stat.mouseSensitivity;
            var mouseY = _pc.MouseDeltaInput.y * _pc.Stat.mouseSensitivity;

            _yRotValue += mouseX;
            _xRotValue -= mouseY;
            _xRotValue = Mathf.Clamp(_xRotValue, -90f, 90f);
            
            
            // 상하 회전적용
            xTransform.localRotation = Quaternion.Euler(_xRotValue, 0f, 0f);
            // 좌우 회전적용
            yTransform.localRotation = Quaternion.Euler(0f, _yRotValue, 0f);
        }
    }
}
