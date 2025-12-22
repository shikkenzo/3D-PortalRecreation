using UnityEngine;

public class CheckpointBase : MonoBehaviour
{
    public ParticleSystem m_ParticleList;
    public bool m_FirstCheckPoint = false;

    bool CheckpointActivated = false;

    [Header("Audio")]
    public AudioSource m_SpawnerAudioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (!CheckpointActivated && other.CompareTag("Player"))
        {
            m_ParticleList.Play();
            AudioManager.GetAudioManager().PlaySourceRegular(m_SpawnerAudioSource);

            CheckpointActivated = true;

            if (!m_FirstCheckPoint)
                GameManager.GetGameManager().GetPlayer().HasTouchedCheckPoint();
        }
    }

    public void Reset()
    {
        CheckpointActivated = false;
    }
}