using UnityEngine;

public class RefractionBase : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public float m_MaxDistance = 50.0f;
    public LayerMask m_LayerMask;
    private bool m_isReflectingLaser = false;

    private void Start()
    {
        m_LineRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_isReflectingLaser)
        {
            if (m_LineRenderer.transform.parent.CompareTag("Portal"))
            {
                if (m_LineRenderer.transform.parent.gameObject.activeInHierarchy)
                {
                    UpdateLaser();
                    m_isReflectingLaser = false;
                }
                else
                    m_LineRenderer.gameObject.SetActive(false);
            }
            else
            {
                UpdateLaser();
                m_isReflectingLaser = false;
            }
        }
        else
            m_LineRenderer.gameObject.SetActive(false);
    }

    public void Reflect()
    {
        if (m_isReflectingLaser)
            return;

        m_isReflectingLaser = true;
        UpdateLaser();
    }

    public void Reflect(Vector3 laserPosition, Vector3 laserDirection)
    {
        if (m_isReflectingLaser)
            return;

        if (m_LineRenderer.transform.parent.CompareTag("Portal"))
            if (!m_LineRenderer.transform.parent.gameObject.activeInHierarchy)
                return;

        m_isReflectingLaser = true;
        UpdateLaser(laserPosition, laserDirection);
    }

    private void UpdateLaser(Vector3 laserPosition, Vector3 laserDirection)
    {
        Portal l_portal = gameObject.GetComponent<Portal>();
        Vector3 l_worldPosition = laserPosition;
        Vector3 l_localPosition = l_portal.m_OtherPortalTransform.InverseTransformPoint(l_worldPosition);
        m_LineRenderer.transform.position = l_portal.m_MirrorPortal.transform.TransformPoint(l_localPosition);

        Vector3 l_worldDirection = laserDirection;
        Vector3 l_localDirection = l_portal.m_OtherPortalTransform.InverseTransformDirection(l_worldDirection);
        m_LineRenderer.transform.forward = l_portal.m_MirrorPortal.transform.TransformDirection(l_localDirection);

        UpdateLaser();
    }

    private void UpdateLaser()
    {
        m_LineRenderer.gameObject.SetActive(true);

        float l_distance = m_MaxDistance;
        Ray l_ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        if (Physics.Raycast(l_ray, out RaycastHit l_raycastHit, m_MaxDistance, m_LayerMask.value, QueryTriggerInteraction.Ignore))
        {
            l_distance = l_raycastHit.distance;
            if (l_raycastHit.collider.CompareTag("RefractionCube"))
                l_raycastHit.collider.GetComponent<RefractionBase>().Reflect();

            else if (l_raycastHit.collider.CompareTag("Turret"))
                l_raycastHit.collider.GetComponent<TeleportBase>().Kill();

            else if (l_raycastHit.collider.CompareTag("Button"))
                l_raycastHit.collider.GetComponent<PortalButton>().PlayButtonRefraction();

            else if (l_raycastHit.collider.transform.parent != null)
            {
                if (l_raycastHit.collider.transform.parent.CompareTag("Portal"))
                    l_raycastHit.collider.transform.parent.GetComponent<RefractionBase>().Reflect(l_raycastHit.point, m_LineRenderer.transform.forward);
            }
            else if (l_raycastHit.collider.CompareTag("Player"))
                l_raycastHit.collider.GetComponent<PlayerController>().Kill();
        }
        Vector3 l_position = Vector3.forward * l_distance;
        m_LineRenderer.SetPosition(1, l_position);
    }
}