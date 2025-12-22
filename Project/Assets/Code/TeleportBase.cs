using UnityEngine;

public class TeleportBase : MonoBehaviour, IRespawnable
{
    Rigidbody m_rigidbody;
    public float m_PortalDistance = 1.5f;
    public float m_MaxAngleToTeleport = 45.0f;
    private bool m_attachedObject = false;
    public int m_MaxResizeCount = 2;
    public ParticleSystem m_ParticleSystem;

    Vector3 m_initialScale, m_initialPosition, m_initialVelocity, m_angularVelocity;
    Quaternion m_initialRotation;
    Transform m_parent;

    public float m_TeleportCooldown = 0.2f;
    private float m_CooldownTimer;

    [Header("Audio")]
    public AudioClip m_DeathAudioClip;
    public void OnEnable()
    {
        GameManager.OnSetRespawn += Set;
    }

    public void OnDisable()
    {
        GameManager.OnSetRespawn -= Set;
    }

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_initialScale = transform.localScale;
        m_initialPosition = transform.position;
        m_initialRotation = transform.rotation;
        m_initialVelocity = m_rigidbody.linearVelocity;
        m_angularVelocity = m_rigidbody.angularVelocity;
        m_parent = transform.parent;
    }

    private void Update()
    {
        m_CooldownTimer -= Time.deltaTime;
        if (m_CooldownTimer < 0) m_CooldownTimer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            Portal l_portal = other.GetComponent<Portal>();
            if (CanTeleport(l_portal))
            {
                Teleport(l_portal);
            }
        }
        else if (other.CompareTag("DestroyingSurface") || other.CompareTag("DeadZone"))
        {
            if (gameObject.CompareTag("RefractionCube"))
            {
                PlayParticles();
                gameObject.GetComponent<IRespawnable>().Reset();
            }
            else
                Kill();
        }
    }

    public void Kill()
    {
        if (GameManager.GetGameManager().GetPlayer().PlayerHasRigidbodyAttached(m_rigidbody))
        {
            GameManager.GetGameManager().GetPlayer().ThrowObject(0.0f);
        }

        PlayParticles();
        gameObject.SetActive(false);
    }

    void PlayParticles()
    {
        AudioManager.GetAudioManager().PlayWorldAudioClip(m_DeathAudioClip, transform.position);

        if (m_ParticleSystem != null)
        {
            m_ParticleSystem.gameObject.transform.position = gameObject.transform.position;
            m_ParticleSystem.Play();
        }
    }

    private bool CanTeleport(Portal _portal)
    {
        float l_dotValue = Vector3.Dot(_portal.transform.forward, -m_rigidbody.linearVelocity.normalized);
        return (!m_attachedObject) && (l_dotValue > Mathf.Cos(m_MaxAngleToTeleport * Mathf.Deg2Rad)) && !_portal.IsAlone() && (m_CooldownTimer <= 0);
    }

    private void Teleport(Portal _portal)
    {
        m_CooldownTimer = m_TeleportCooldown;

        AudioManager.GetAudioManager().PlaySourceAudioClip(_portal.m_MirrorPortal.m_PortalAudioSource, _portal.m_MirrorPortal.m_TeleportAudioClip);

        Vector3 l_direction = m_rigidbody.linearVelocity.normalized;
        Vector3 l_worldPosition = transform.position + l_direction * m_PortalDistance;
        Vector3 l_localPosition = _portal.m_OtherPortalTransform.InverseTransformPoint(l_worldPosition);
        transform.position = _portal.m_MirrorPortal.transform.TransformPoint(l_localPosition);

        Vector3 l_worldDirection = transform.forward;
        Vector3 l_localDirection = _portal.m_OtherPortalTransform.InverseTransformDirection(l_worldDirection);
        transform.forward = _portal.m_MirrorPortal.transform.TransformDirection(l_localDirection);

        Vector3 l_worldVelocity = m_rigidbody.linearVelocity;
        Vector3 l_localVelocity = _portal.m_OtherPortalTransform.InverseTransformDirection(l_worldVelocity);
        m_rigidbody.linearVelocity = _portal.m_MirrorPortal.transform.TransformDirection(l_localVelocity);

        if (gameObject.CompareTag("Cube"))
        {
            float l_scale = _portal.m_MirrorPortal.transform.localScale.x / _portal.transform.localScale.x;
            m_rigidbody.transform.localScale = Vector3.one * l_scale * m_rigidbody.transform.localScale.x;

            if ((transform.localScale).magnitude < (m_initialScale * Mathf.Pow(_portal.m_PortalSizes[0], m_MaxResizeCount)).magnitude)
                transform.localScale = m_initialScale * Mathf.Pow(_portal.m_PortalSizes[0], m_MaxResizeCount);

            if ((transform.localScale).magnitude > (m_initialScale * Mathf.Pow(_portal.m_PortalSizes[_portal.m_PortalSizes.Count - 1], m_MaxResizeCount)).magnitude)
                transform.localScale = m_initialScale * Mathf.Pow(_portal.m_PortalSizes[_portal.m_PortalSizes.Count - 1], m_MaxResizeCount);
        }
    }

    public void SetAttachedObject(bool attachedObject)
    {
        m_attachedObject = attachedObject;
    }
    public void Set()
    {
        GameManager.GetGameManager().SetToRespawnableList(this);
    }
    public void Reset()
    {
        GameManager.GetGameManager().GetPlayer().ThrowObject(0.0f);
        m_rigidbody.transform.SetParent(m_parent);
        m_rigidbody.linearVelocity = m_initialVelocity;
        m_rigidbody.angularVelocity = m_angularVelocity;
        transform.position = m_initialPosition;
        transform.rotation = m_initialRotation;
        transform.localScale = m_initialScale;

        gameObject.SetActive(true);
    }
}