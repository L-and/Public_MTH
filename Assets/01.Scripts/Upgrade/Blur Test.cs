using UnityEngine;

public class BlurTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            var blur = Object.FindFirstObjectByType<CameraBlurController>();
            if (blur != null)
            {
                blur.SetBlurIntensity(1f);
                Debug.Log("블러 켜짐");
            }
            else
            {
                Debug.LogWarning("CameraBlurController를 찾지 못했습니다!");
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            var blur = Object.FindFirstObjectByType<CameraBlurController>();
            if (blur != null)
            {
                blur.DisableBlur();
                Debug.Log("블러 꺼짐");
            }
            else
            {
                Debug.LogWarning("CameraBlurController를 찾지 못했습니다!");
            }
        }
    }
}
