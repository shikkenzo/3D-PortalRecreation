using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public GameObject m_CompanionCube;

    [Header("Animations")]
    public Animation m_Animation;
    public AnimationClip m_PressedAnimationClip;

    [Header("Audio")]
    public AudioSource m_SpawnerAudioSource;

    [Header("ParticleSystem")]
    public ParticleSystem m_ParticleSystem;

    public void Spawn()
    {
        m_Animation.Play(m_PressedAnimationClip.name);
        AudioManager.GetAudioManager().PlaySourceAudioClip(m_SpawnerAudioSource);
        m_ParticleSystem.Play();

        m_CompanionCube.GetComponent<IRespawnable>().Reset();
    }
}