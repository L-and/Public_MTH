using UnityEngine;

public class AttackSoundMelee : MonoBehaviour
{
    [SerializeField] private AudioClip attackClip;  // Inspector에서 직접 할당
    private AudioSource aud;

    void Start()
    {
        // 이 스크립트 전용 AudioSource가 없으면 새로 추가
        aud = GetComponent<AudioSource>();
        if (aud == null)
            aud = gameObject.AddComponent<AudioSource>();

        aud.playOnAwake = false;
        aud.spatialBlend = 1f; // 3D 사운드
    }

    public void PlaySound()
    {
        if (attackClip == null) return;
        aud.PlayOneShot(attackClip);
    }
}
