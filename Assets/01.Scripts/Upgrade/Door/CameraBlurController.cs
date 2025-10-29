using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraBlurController : MonoBehaviour
{
    private Volume volume;
    private DepthOfField dof;

    [Header("블러 기본값 설정")]
    public float minFocusDistance = 10f; // 초점이 멀수록 배경 선명
    public float maxFocusDistance = 2.8f; // 초점이 가까울수록 배경 흐림
    public float minAperture = 8f;  //조리개 작게 (선명할 때)
    public float maxAperture = 2.8f; // 조리개 크게 (흐릴 때)

    private bool isInitialized = false;

    void Awake()
    {
        volume = GetComponent<Volume>();
        if (volume == null)
        {
            if (volume == null)
            {
                Debug.Log("CameraBlurController: Volume 컴포넌트를 찾지 못했습니다!");
                return;
            }

            if (!volume.profile.TryGet(out dof))
            {
                Debug.Log("CameraBlurController: DepthOfField 프로필을 찾지 못했습니다!");
                return;
            }

            //초기화
            isInitialized = true;

            // 시작 시 블러 비활성화
            dof.active = false;
            dof.focusDistance.value = minFocusDistance;
            dof.aperture.value = minAperture;

            Debug.Log("블러 컨트롤 초기화 완료");

        }
    }

    public void SetBlurIntensity(float t)
    {
        if (!isInitialized  ||  dof == null) return;

        // 활성화
        dof.active = true;
        dof.focusDistance.value = Mathf.Lerp(minFocusDistance, maxFocusDistance, t);
        dof.aperture.value = Mathf.Lerp(minAperture, maxAperture, t);

    }

    public void DisableBlur() 
    {
         if (!isInitialized  ||  dof == null) return;

        if (dof == null) return;
        dof.focusDistance.value = minFocusDistance;
        dof.aperture.value = minAperture;
        dof.active = false;

        Debug.Log("블러 OFF (화면 복원 완료)");
    }


}
