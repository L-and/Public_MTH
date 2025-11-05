using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("사운드 이펙트(SFX) 재생에 사용할 오디오 소스")]
    public AudioSource sfxSource;
    [Tooltip("배경 음악(BGM) 재생에 사용할 오디오 소스")]
    public AudioSource musicSource;

    public void PlaySound(string soundName)
    {
        sfxSource.Play();
    }
}
