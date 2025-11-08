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

        // 연주 -  블러 기능 추가
        //[Header("블러 제어용")]
        //[SerializeField] private CameraBlurController _blurController;
        //[SerializeField] private bool _isBlurActive = false;

        // 가로,세로의 현재 플레이어의 회전값
        private float _xRotValue;
        private float _yRotValue;

        private void Awake()
        {
            xTransform = transform;
            yTransform = transform.root;
        
            _pc = yTransform.GetComponent<PlayerController>();

            // 연주 - 카메라 블러 컨트롤러 갖고 오기
            //var mainCam = Camera.main;
            //if (mainCam != null)
            //    _blurController = mainCam.GetComponent<CameraBlurController>();
        }

        private void LateUpdate()
        {
            // 연주 - 블러가 활성화 중이면 시점 업데이트 중지
            // if (_isBlurActive) return;

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

        /// <summary>
        /// 연주 - 외부에서 블러 상태를 갱신하고 제어하는 메서드
        /// </summary>
        //public void UpdateBlurState(bool enable, float duration = 1f)
        //{
        //    if (_blurController == null) return;

        //    _isBlurActive = enable;

        //    if (enable)
        //    {
        //        Debug.Log("AimLook에서 블러 ON 요청");
        //        StartCoroutine(FadeBlurIn(duration));
        //    }
        //    else
        //    {
        //        Debug.Log("AimLook에서 블러 OFF 요청");
        //        StartCoroutine(FadeBlurOut(duration));
        //    }
        //}

        //private System.Collections.IEnumerator FadeBlurIn(float duration)
        //{
        //    float time = 0f;
        //    while (time < duration)
        //    {
        //        time += Time.deltaTime;
        //        float t = Mathf.Clamp01(time / duration);
        //        _blurController.SetBlurIntensity(t);
        //        yield return null;
        //    }
        //}

        //private System.Collections.IEnumerator FadeBlurOut(float duration)
        //{
        //    float time = 0f;
        //    while (time < duration)
        //    {
        //        time += Time.deltaTime;
        //        float t = 1f - Mathf.Clamp01(time / duration);
        //        _blurController.SetBlurIntensity(t);
        //        yield return null;
        //    }

        //    _blurController.DisableBlur();
        //}




    }
}
