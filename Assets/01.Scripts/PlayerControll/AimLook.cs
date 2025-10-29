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

        // 가로,세로의 현재 플레이어의 회전값
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
            var mouseX = _pc.MouseDeltaInput.x * _pc.Status.mouseSensitivity;
            var mouseY = _pc.MouseDeltaInput.y * _pc.Status.mouseSensitivity;

            _yRotValue += mouseX;
            _xRotValue -= mouseY;
            _xRotValue = Mathf.Clamp(_xRotValue, -90f, 90f);
            
            
            // 상하 회전적용
            xTransform.localRotation = Quaternion.Euler(_xRotValue, 0f, 0f);
            // 좌우 회전적용
            yTransform.localRotation = Quaternion.Euler(0f, _yRotValue, 0f);


        }

        /// <summary>
        /// 외부에서 플레이어의 카메라회전 조작 후 회전값을 갱신하는 메서드
        /// </summary>
        /// <param name="newXRotation">새로운 X회전값</param>
        /// <param name="newYRotation">새로운 Y회전값</param>
        public void UpdateRotation(Quaternion newXRotation, Quaternion newYRotation)
        {
            _xRotValue = newXRotation.eulerAngles.x;
            _yRotValue = newYRotation.eulerAngles.y;
        }
    }
}
