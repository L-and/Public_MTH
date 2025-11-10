using UnityEngine;
using UnityEngine.Audio;

public class EnemySound : MonoBehaviour
{
    [Header("Audio Source & Mixer")]
    [SerializeField] private AudioSource source;   // 비워두면 자동으로 가져오거나 생성
    [SerializeField] private AudioMixerGroup outputGroup; // 선택

    [Header("Clips")]
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private AudioClip[] walkClips;
    [SerializeField] private AudioClip[] AttackClips;

    [Header("Volumes")]
    [Range(0f, 1f)] public float hitVolume = 1f;
    [Range(0f, 1f)] public float deathVolume = 1f;
    [Range(0f, 1f)] public float walkVolume = 1f;
    [Range(0f, 1f)] public float AttackVolume = 1f;

    [Header("Pitch Randomize")]
    public bool randomizePitch = true;
    [Range(0.5f, 1.5f)] public float pitchMin = 0.95f;
    [Range(0.5f, 1.5f)] public float pitchMax = 1.05f;

    [Header("3D Settings")]
    [Range(0f, 1f)] public float spatialBlend = 1f; // 1=3D
    public float minDistance = 1.5f;
    public float maxDistance = 20f;
    public AudioRolloffMode rolloff = AudioRolloffMode.Logarithmic;
    [Header("Footstep Source (전용)")]
    [SerializeField] private AudioSource footstepSource; // Inspector에 비워두면 자동 생성
    [Range(0f,1f)] public float footstepSpatialBlend = 0.6f; // 살짝 3D
    public float footstepMinDistance = 4f;   // 가까울 때 크게 들리도록 ↑
    public float footstepMaxDistance = 18f;  // 감쇠 범위는 짧게
    public AudioRolloffMode footstepRolloff = AudioRolloffMode.Linear;
    [Range(0f, 2f)] public float footstepGain = 1.3f; // 발소리 보정 게인

    void Awake()
    {
        if (!source)
        {
            source = GetComponent<AudioSource>();
            if (!source) source = gameObject.AddComponent<AudioSource>();
        }

        // 3D 세팅 기본값
        source.playOnAwake = false;
        source.spatialBlend = spatialBlend;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = rolloff;
        if (outputGroup) source.outputAudioMixerGroup = outputGroup;

        if (!footstepSource)
        {
            // 같은 오브젝트에 전용 소스 추가
            footstepSource = gameObject.AddComponent<AudioSource>();
        }
        footstepSource.playOnAwake = false;
        footstepSource.spatialBlend = footstepSpatialBlend;
        footstepSource.minDistance = footstepMinDistance;
        footstepSource.maxDistance = footstepMaxDistance;
        footstepSource.rolloffMode = footstepRolloff;
        footstepSource.outputAudioMixerGroup = outputGroup; // 필요시 동일 그룹
        footstepSource.dopplerLevel = 0f; // 발소리는 도플러 꺼두는 편이 자연스러움
    }

    // 외부에서 호출할 간단 API들
    public void PlayHit()
    {
        PlayOneShotRandom(hitClips, hitVolume);
    }

    public void PlayDeath()
    {
        PlayOneShotRandom(deathClips, deathVolume);
    }

    public void PlayWalk()
    {
        var clip = PickRandom(walkClips);
        if (!clip) return;

        // 랜덤 피치는 너무 과하면 박자감이 무너짐 → 살짝만
        float prevPitch = footstepSource.pitch;
        if (randomizePitch) footstepSource.pitch = Random.Range(Mathf.Max(0.97f, pitchMin), Mathf.Min(1.03f, pitchMax));

        footstepSource.PlayOneShot(clip, walkVolume * footstepGain);

        footstepSource.pitch = prevPitch;
    }

    public void PlayAttack()
    {
        PlayOneShotRandom(AttackClips, AttackVolume);
    }

    public void Sfx_ByClip(AudioClip clip)
    {
        if (!clip) return;
        PlayClip(clip, AttackVolume);
    }
    

    // 내부 보조
    private void PlayOneShotRandom(AudioClip[] bank, float volume)
    {
        var clip = PickRandom(bank);
        if (!clip) return;
        PlayClip(clip, volume);
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        if (!clip || !source) return;

        float prevPitch = source.pitch;
        if (randomizePitch)
            source.pitch = Random.Range(pitchMin, pitchMax);

        // 한 AudioSource로 여러 소리를 겹쳐 재생하려면 PlayOneShot 사용
        source.PlayOneShot(clip, volume);

        source.pitch = prevPitch;
    }    

    private static AudioClip PickRandom(AudioClip[] bank)
    {
        if (bank == null || bank.Length == 0) return null;
        int i = Random.Range(0, bank.Length);
        return bank[i];
    }
}
