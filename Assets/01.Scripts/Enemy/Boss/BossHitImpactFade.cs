using System.Collections;
using UnityEngine;

public class BossHitImpactFade : MonoBehaviour
{
    [Header("피격 이펙트 사라지는 타이머")]
    public float effectFadeTimer = 10.0f;

    [Header("Audio")]
    public AudioClip[] effectSounds;
    public AudioSource effectSoundSource;

    private void Start()
    {
        StartCoroutine(DespawnTimer());
        effectSoundSource.clip = effectSounds[Random.Range(0, effectSounds.Length)];
        effectSoundSource.Play();
    }

    private IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(effectFadeTimer);
        Destroy(gameObject);
    }
}
