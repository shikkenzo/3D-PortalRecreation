using System.Collections;
using UnityEngine;

public class PortalGate : MonoBehaviour, IRespawnable
{
    public Animation m_Animation;
    public AnimationClip m_OpenAnimationClip;
    public AnimationClip m_OpenedAnimationClip;
    public AnimationClip m_CloseAnimationClip;
    public AnimationClip m_ClosedAnimationClip;

    bool m_IsOpened;

    public enum TState
    {
        OPEN,
        OPENED,
        CLOSE,
        CLOSED
    }
    private TState m_state;

    public void OnEnable()
    {
        GameManager.OnSetRespawn += Set;
    }
    public void OnDisable()
    {
        GameManager.OnSetRespawn -= Set;
    }
    private void Start()
    {
        if (m_IsOpened)
            SetOpenedState();
        else
            SetClosedState();
    }

    private void SetOpenedState()
    {
        m_state = TState.OPENED;
        m_Animation.Play(m_OpenedAnimationClip.name);
    }

    private void SetClosedState()
    {
        m_state = TState.CLOSED;
        m_Animation.Play(m_ClosedAnimationClip.name);
    }

    public void Open()
    {
        if (m_state == TState.OPENED || m_state == TState.OPEN) return;

        m_state = TState.OPEN;
        m_Animation.CrossFade(m_OpenAnimationClip.name, 0.1f);
        StopAllCoroutines();
        StartCoroutine(SetState(m_OpenAnimationClip.length, TState.OPENED));
    }

    public void Close()
    {
        if (m_state == TState.CLOSED || m_state == TState.CLOSE) return;

        m_state = TState.CLOSE;
        m_Animation.CrossFade(m_CloseAnimationClip.name, 0.1f);
        StopAllCoroutines();
        StartCoroutine(SetState(m_CloseAnimationClip.length, TState.CLOSED));
    }

    IEnumerator SetState(float animationTime, TState state)
    {
        yield return new WaitForSeconds(animationTime);
        m_state = state;
    }
    public void Set()
    {
        GameManager.GetGameManager().SetToRespawnableList(this);
    }
    public void Reset()
    {
        SetClosedState();
    }
}
