using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager m_audioManager;

    public AudioSource m_MusicAudioSource;
    public AudioSource m_SfxAudioSource;

    public float m_BaseMusicVolume;

    private void Awake()
    {
        if (m_audioManager != null)
        {
            Destroy(gameObject);
            return;
        }
        m_audioManager = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        m_BaseMusicVolume = m_MusicAudioSource.volume;
    }
    public static AudioManager GetAudioManager()
    {
        return m_audioManager;
    }

    public void PlaySfxAudioClip(AudioClip sfx)
    {
        m_SfxAudioSource.clip = sfx;
        m_SfxAudioSource.PlayOneShot(m_SfxAudioSource.clip);

        RandomPitch();
    }
    public void PlaySourceAudioClip(AudioSource source, AudioClip sfx)
    {
        source.clip = sfx;
        source.PlayOneShot(source.clip);

        RandomPitch(source);
    }
    public void PlaySourceAudioClip(AudioSource source)
    {
        source.PlayOneShot(source.clip);

        RandomPitch(source);
    }
    public void PlaySourceRegular(AudioSource source)
    {
        source.Play();

        RandomPitch(source);
    }
    public void PlayWorldAudioClip(AudioClip sfx, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(sfx, position);
    }
    public void SetMusicVolume(float volume)
    {
        m_MusicAudioSource.volume = m_BaseMusicVolume * volume;
    }

    private void RandomPitch()
    {
        m_SfxAudioSource.pitch = 0.9f + 0.2f * Random.value;
    }
    private void RandomPitch(AudioSource source)
    {
        source.pitch = 0.9f + 0.2f * Random.value;
    }
}
