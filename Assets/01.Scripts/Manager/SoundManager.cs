using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundManager : MonoBehaviour
{
  [Header("Audio Sources")]
  [Tooltip("사운드 이펙트(SFX) 재생에 사용할 오디오 소스")]
  private AudioSource sfxSource;
  [Tooltip("배경 음악(BGM) 재생에 사용할 오디오 소스")]
  private AudioSource musicSource;

  private AudioClip sfxClip;
  private AudioClip musicClip;

  private Dictionary<string, AudioClip> _soundDict;
  void Awake()
  {
    InitializeAudioSource();
  }

  // async void Start()
  // {
  //   StartCoroutine(Set_soundDict());
  // }

  private void InitializeAudioSource()
  {
    sfxSource = gameObject.AddComponent<AudioSource>();

    sfxSource.playOnAwake = false; // 기본적으로 자동 재생 끄기
    sfxSource.spatialBlend = 0;    // 2D 사운드로 설정
    sfxSource.volume = 0.5f;       // 볼륨 조절

    musicSource = gameObject.AddComponent<AudioSource>();

    musicSource.playOnAwake = false; // 기본적으로 자동 재생 끄기
    musicSource.loop = true;         // 루프 활성화
    musicSource.spatialBlend = 0;    // 2D 사운드로 설정
    musicSource.volume = 0.2f;       // 볼륨 조절
  }

  public void PlaySFX(string soundName)
  {
    GameManager.ResourceEx.SoundDict.TryGetValue(soundName, out AudioClip sfxClip);
    if (sfxClip == null)
    {
      Debug.Log(soundName + " 효과음 파일을 찾을 수 없음");
      return;
    }
    sfxSource.PlayOneShot(sfxClip);
  }

  public void PlayMusic(string musicName)
  {
    musicSource.Stop();
    if (_soundDict[musicName] == null) return;
    musicClip = _soundDict[musicName];
    musicSource.clip = musicClip;
    musicSource.Play();
  }

  public void Set_soundDict()
  {
    _soundDict = GameManager.ResourceEx.SoundDict;
  }
}