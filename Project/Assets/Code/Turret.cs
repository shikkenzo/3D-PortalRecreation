using UnityEngine;

public class Turret : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public float m_MaxDistance = 50.0f;
    public LayerMask m_LayerMask;
    public float m_MaxAliveAngle = 15.0f;

    [Header("Audio")]
    public AudioSource m_AudioSource;

    private void Update()
    {
        if (gameObject.CompareTag("Turret"))
        {
            float l_dotValue;
            if (transform.parent == null)
            {
                l_dotValue = Vector3.Dot(transform.up, Vector3.up);
            }
            else
            {
                l_dotValue = Vector3.Dot(transform.up, transform.parent.transform.up);
            }

            if (l_dotValue < Mathf.Cos(m_MaxAliveAngle * Mathf.Deg2Rad))
            {
                m_LineRenderer.gameObject.SetActive(false);
                m_AudioSource.Pause();
            }
            else
                UpdateLaser();
        }
        else
            UpdateLaser();
    }

    void UpdateLaser()
    {
        m_AudioSource.UnPause();

        m_LineRenderer.gameObject.SetActive(true);
        float l_distance = m_MaxDistance;

        Ray l_ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        if (Physics.Raycast(l_ray, out RaycastHit l_raycastHit, m_MaxDistance, m_LayerMask.value, QueryTriggerInteraction.Ignore))
        {
            l_distance = l_raycastHit.distance;

            if (l_raycastHit.collider.CompareTag("Player"))
                l_raycastHit.collider.GetComponent<PlayerController>().Kill();

            else if (l_raycastHit.collider.CompareTag("Turret"))
                l_raycastHit.collider.GetComponent<TeleportBase>().Kill();

            if (l_raycastHit.collider.CompareTag("RefractionCube"))
                l_raycastHit.collider.GetComponent<RefractionBase>().Reflect();
            else if (l_raycastHit.collider.transform.parent != null)
            {
                if (l_raycastHit.collider.transform.parent.CompareTag("Portal"))
                    l_raycastHit.collider.transform.parent.GetComponent<RefractionBase>().Reflect(l_raycastHit.point, m_LineRenderer.transform.forward);
            }
        }
        Vector3 l_position = Vector3.forward * l_distance;
        m_LineRenderer.SetPosition(1, l_position);
    }
}