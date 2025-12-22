using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera m_Camera;
    public Transform m_OtherPortalTransform;
    public Portal m_MirrorPortal;
    public float m_NearCameraOffset = 0.5f;
    public List<Transform> m_ValidPositions;
    public GameObject m_Visual, m_LineRenderer;
    public Transform m_LeftPortalPlane, m_RightPortalPlane;

    [Header("Validation")]
    public float m_ValidDistanceOffset = 0.15f;
    public LayerMask m_ValidPortalLayerMask;
    public float m_MaxAnglePermitted = 5.0f;

    [Header("Preview")]
    public GameObject m_RedPortalPreview;
    public GameObject m_GreenPortalPreview;

    [Header("Resizing")]
    public List<float> m_PortalSizes;
    public bool m_WheelDirectionUp;

    [Header("Audio")]
    public AudioSource m_PortalAudioSource;
    public AudioClip m_TeleportAudioClip;

    public enum portalSize
    {
        SMALL = 0,
        BASE,
        LARGE
    }
    public portalSize m_currentPortalSize { get; private set; }

    private void Start()
    {
        m_currentPortalSize = portalSize.BASE;
    }

    private void Update()
    {
        if (IsAlone())
        {
            m_Visual.SetActive(false);
            m_LineRenderer.SetActive(false);
        }
        else if (!m_Visual.activeInHierarchy)
            m_Visual.SetActive(true);
    }

    private void LateUpdate()
    {
        Vector3 l_worldPosition = Camera.main.transform.position;
        Vector3 l_localPosition = m_OtherPortalTransform.InverseTransformPoint(l_worldPosition);

        m_MirrorPortal.m_Camera.transform.position = m_MirrorPortal.transform.TransformPoint(l_localPosition);

        Vector3 l_worldForward = Camera.main.transform.forward;
        Vector3 l_localForward = m_OtherPortalTransform.InverseTransformDirection(l_worldForward);

        m_MirrorPortal.m_Camera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_localForward);

        float l_distanceToPortal = Vector3.Distance(m_MirrorPortal.transform.position, m_MirrorPortal.m_Camera.transform.position);
        m_MirrorPortal.m_Camera.nearClipPlane = l_distanceToPortal + m_NearCameraOffset;
    }

    public bool IsValidPosition(Vector3 position, Vector3 normal)
    {
        gameObject.SetActive(false);
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(normal);
        bool l_valid = true;

        Vector3 l_cameraPosition = Camera.main.transform.position;
        for (int i = 0; i < m_ValidPositions.Count; i++)
        {
            Vector3 l_validPosition = m_ValidPositions[i].position;

            Vector3 l_direction = l_validPosition - l_cameraPosition;
            float l_distance = l_direction.magnitude;
            l_direction /= l_distance;

            Ray l_Ray = new Ray(l_cameraPosition, l_direction);
            if (Physics.Raycast(l_Ray, out RaycastHit l_raycastHit, l_distance + m_ValidDistanceOffset, m_ValidPortalLayerMask.value, QueryTriggerInteraction.Ignore))
            {
                if (l_raycastHit.collider.gameObject.CompareTag("DrawableWall"))
                {
                    if (Vector3.Distance(l_raycastHit.point, l_validPosition) < m_ValidDistanceOffset)
                    {
                        float l_dotAngle = Vector3.Dot(l_raycastHit.normal, m_ValidPositions[i].forward);
                        if (l_dotAngle < Mathf.Cos(m_MaxAnglePermitted * Mathf.Deg2Rad))
                        {
                            l_valid = false;
                        }
                    }
                    else
                    {
                        l_valid = false;
                    }
                }
                else
                {
                    l_valid = false;
                }
            }
            else
            {
                l_valid = false;
            }
        }
        return l_valid;
    }

    public void ResizePortal(bool isDirectionUp)
    {
        m_WheelDirectionUp = isDirectionUp;
        if (m_WheelDirectionUp)
        {
            m_currentPortalSize++;
            if ((int)m_currentPortalSize >= m_PortalSizes.Count)
            {
                m_currentPortalSize = (portalSize)m_PortalSizes.Count - 1;
            }
            ResizePortal(m_PortalSizes[(int)m_currentPortalSize]);
        }
        else if (!m_WheelDirectionUp)
        {
            m_currentPortalSize--;
            if ((int)m_currentPortalSize < 0)
            {
                m_currentPortalSize = 0;
            }
            ResizePortal(m_PortalSizes[(int)m_currentPortalSize]);
        }
    }

    public void ResizePortal(float scale)
    {
        gameObject.transform.localScale = Vector3.one * scale;
        m_GreenPortalPreview.transform.localScale = Vector3.one * scale;
        m_RedPortalPreview.transform.localScale = Vector3.one * scale;
    }

    public bool IsAlone()
    {
        return !m_MirrorPortal.gameObject.activeInHierarchy;
    }

    public void ResetPortalSize()
    {
        ResizePortal(1.0f);
        m_currentPortalSize = portalSize.BASE;
    }
}
