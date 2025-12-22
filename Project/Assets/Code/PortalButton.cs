using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    public enum ButtonType
    {
        CUBE = 0,
        REFRACTION
    }

    [Header("ButtonType")]
    public ButtonType m_ButtonType;

    [Header("Animations")]
    public AnimationClip m_OpenAnimationClip;
    public AnimationClip m_CloseAnimationClip;
    Animation m_Animation;

    [Header("Events")]
    public UnityEvent m_OpenEvent, m_CloseEvent;

    [Header("Audio")]
    public AudioSource m_ButtonAudioSource;
    public AudioClip m_PressButtonAudioClip;
    public AudioClip m_ReverseButtonAudioClip;

    [Header("ParticleSystem")]
    public ParticleSystem m_ParticleSystem;

    bool m_IsNotPressed = true, m_IsBeingRefracted = false;

    private void Start()
    {
        m_Animation = GetComponent<Animation>();
    }

    private void Update()
    {
        if (m_ButtonType == ButtonType.REFRACTION)
        {
            if (m_IsBeingRefracted)
                m_IsBeingRefracted = false;
            else if (!m_IsNotPressed)
                StopButton();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_ButtonType == ButtonType.CUBE)
            if (other.CompareTag("Cube"))
                PlayButton();
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_ButtonType == ButtonType.CUBE)
            if (other.CompareTag("Cube"))
                StopButton();
    }

    void PlayButton()
    {
        AudioManager.GetAudioManager().PlaySourceAudioClip(m_ButtonAudioSource, m_PressButtonAudioClip);
        m_Animation.Play(m_OpenAnimationClip.name);
        m_ParticleSystem.Play();

        m_IsNotPressed = false;
        m_OpenEvent.Invoke();
    }

    public void StopButton()
    {
        AudioManager.GetAudioManager().PlaySourceAudioClip(m_ButtonAudioSource, m_ReverseButtonAudioClip);
        m_Animation.Play(m_CloseAnimationClip.name);
        m_ParticleSystem.Play();

        m_IsNotPressed = true;
        m_CloseEvent.Invoke();
    }

    public void PlayButtonRefraction()
    {
        if (m_ButtonType != ButtonType.REFRACTION)
            return;

        m_IsBeingRefracted = true;

        if (!m_IsNotPressed)
            return;

        PlayButton();
    }
}