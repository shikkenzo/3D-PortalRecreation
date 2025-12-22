using UnityEngine;

public class PlayerController : MonoBehaviour, IRespawnable
{
    [Header("Camera")]
    private float m_Yaw, m_InitialYaw;
    private float m_Pitch, m_InitialPitch;

    public float m_YawSpeed;
    public float m_PitchSpeed;

    public float m_MinPitch;
    public float m_MaxPitch;

    public Transform m_PitchController;

    public bool m_UseInvertedYaw;
    public bool m_UseInvertedPitch;

    private bool m_AngleLocked = false;

    public Camera m_Camera;

    [Header("Movement")]
    public CharacterController m_CharacterController;

    public float m_BaseMovementSpeed;
    public float m_RunningSpeedMultiplier;
    public float m_JumpSpeed;

    public float m_GravityMultiplier;
    private float m_CurrentGravityMultiplier;
    private float m_VerticalSpeed;

    public float m_CoyoteTime;
    private float m_CoyoteTimer;

    [Header("Health")]
    public int m_InitialHP = 100;
    public int m_CurrentHP { get; private set; }

    [Header("Respawn")]
    Vector3 m_RespawnPosition;
    Quaternion m_StartRotation;

    [Header("Shooting")]
    public float m_ShootMaxDistance = 50.0f;

    public float m_BulletCooldown;
    public bool m_UseBulletCooldown;
    private float m_bulletCooldownTimer;

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_GrabKeyCode = KeyCode.E;
    public int m_BlueShootMouseButton = 0;
    public int m_OrangeShootMouseButton = 1;
    public int m_ThrowMouseButton = 0;
    public int m_DropMouseButton = 1;
    public string m_MouseWheelAxisName = "Mouse ScrollWheel";

    [Header("Animation")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;

    [Header("Teleport")]
    public float m_PortalDistance = 1.5f;
    private Vector3 m_movementDirection;
    public float m_MaxAngleToTeleport = 60.0f;

    [Header("Portals")]
    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public LayerMask m_ValidShootingLayerMask;

    [Header("AttachObject")]
    public ForceMode m_ForceMode;
    public float m_ThrowForce = 10.0f;
    public Transform m_GripTransform;
    private Rigidbody m_attachedObjectRigidbody;
    private bool m_attachingObject;
    private Vector3 m_startAttachingObjectPosition;
    private Quaternion m_startAttachingObjectRotation;
    private float m_attachingCurrentTime;
    public float m_AttachingTime = 1.5f;
    public float m_AttachingObjectRotationDistanceLerp = 2.0f;
    private bool m_attachedObject;
    public LayerMask m_ValidInteractLayerMask;
    public float m_GrabMaxDistance = 5.0f;

    [Header("Audio")]
    public AudioSource m_PlayerFootstepsAudioSource;
    public AudioClip m_PortalGunAudioClip;
    public AudioClip m_TeleportAudioClip;
    public AudioClip m_DeathAudioClip;
    private float m_footstepTimer;
    public float m_MinimumVelocityFootsteps = 0.3f;

    [Header("ParticleSystem")]
    public ParticleSystem m_ShootParticles;

    [Header("Debug Input")]
    public KeyCode m_AngleLockedKeycode = KeyCode.I;

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
        PlayerController l_player = GameManager.GetGameManager().GetPlayer();
        if (l_player != null)
        {
            l_player.m_CharacterController.enabled = false;
            l_player.transform.position = CheckpointController.GetCheckPointController().GetPlayerPosition().position;
            l_player.transform.rotation = CheckpointController.GetCheckPointController().GetPlayerPosition().rotation;
            l_player.m_CharacterController.enabled = true;

            l_player.m_RespawnPosition = CheckpointController.GetCheckPointController().GetPlayerPosition().position;
            l_player.m_StartRotation = CheckpointController.GetCheckPointController().GetPlayerPosition().rotation;

            l_player.m_Yaw = m_Yaw;
            l_player.m_Pitch = m_Pitch;

            GameObject.Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        GameManager.GetGameManager().SetPlayer(this);

        Cursor.lockState = CursorLockMode.Locked;

        m_RespawnPosition = CheckpointController.GetCheckPointController().GetPlayerPosition().position;
        m_StartRotation = CheckpointController.GetCheckPointController().GetPlayerPosition().rotation;
        transform.position = m_RespawnPosition;
        transform.rotation = m_StartRotation;

        Vector3 l_InitialRotation = CheckpointController.GetCheckPointController().GetPlayerPosition().eulerAngles;
        m_Yaw = l_InitialRotation.y;
        m_Pitch = l_InitialRotation.x;

        m_Camera.transform.rotation = Quaternion.Euler(m_Pitch, m_Yaw, 0f);

        m_InitialYaw = m_Yaw;
        m_InitialPitch = m_Pitch;

        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);

        m_BluePortal.m_RedPortalPreview.SetActive(false);
        m_BluePortal.m_GreenPortalPreview.SetActive(false);
        m_OrangePortal.m_RedPortalPreview.SetActive(false);
        m_OrangePortal.m_GreenPortalPreview.SetActive(false);
    }

    private void Update()
    {
        //DEBUG//
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Reset();
        }
        //DEBUG END//

        if (CanvasController.m_IsInGameOver || CanvasController.m_IsInPause)
            return;

        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(m_AngleLockedKeycode))
            m_AngleLocked = !m_AngleLocked;

        if (!m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_MouseX * m_YawSpeed * Time.deltaTime * (m_UseInvertedYaw ? -1.0f : 1.0f);
            m_Pitch = m_Pitch + l_MouseY * m_PitchSpeed * Time.deltaTime * (m_UseInvertedPitch ? -1.0f : 1.0f);

            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

            transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
            m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
        }

        Vector3 l_Movement = Vector3.zero;

        float l_YawPiRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90PiRadians = (m_Yaw + 90) * Mathf.Deg2Rad;

        Vector3 l_ForwardDirection = new Vector3(Mathf.Sin(l_YawPiRadians), 0.0f, Mathf.Cos(l_YawPiRadians));
        Vector3 l_RightDirection = new Vector3(Mathf.Sin(l_Yaw90PiRadians), 0.0f, Mathf.Cos(l_Yaw90PiRadians));

        if (Input.GetKey(m_RightKeyCode))
            l_Movement = l_RightDirection;
        else if (Input.GetKey(m_LeftKeyCode))
            l_Movement += -l_RightDirection;

        if (Input.GetKey(m_UpKeyCode))
            l_Movement += l_ForwardDirection;
        else if (Input.GetKey(m_DownKeyCode))
            l_Movement += -l_ForwardDirection;

        float l_SpeedMultiplier = 1.0f;
        if (Input.GetKey(m_RunKeyCode))
            l_SpeedMultiplier = m_RunningSpeedMultiplier;

        l_Movement.Normalize();
        m_movementDirection = l_Movement;
        l_Movement *= m_BaseMovementSpeed * l_SpeedMultiplier * Time.deltaTime;

        ManageGravity();
        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * m_CurrentGravityMultiplier * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        if (((m_VerticalSpeed < 0.0f) && ((l_CollisionFlags & CollisionFlags.Below) != 0)))
        {
            m_VerticalSpeed = 0.0f;
            m_CoyoteTimer = m_CoyoteTime;
        }
        else if (m_VerticalSpeed > 0.0f && (l_CollisionFlags & CollisionFlags.Above) != 0)
            m_VerticalSpeed = 0.0f;

        if (m_CoyoteTimer > 0.0f)
        {
            if (Input.GetKeyDown(m_JumpKeyCode))
            {
                m_VerticalSpeed = m_JumpSpeed;
                m_CoyoteTimer = 0.0f;
            }
        }

        if (Input.GetMouseButtonUp(m_BlueShootMouseButton) || Input.GetMouseButtonUp(m_OrangeShootMouseButton))
            m_ShootParticles.Play();

        //Shooting
        if (CanShoot())
        {
            if (Input.GetMouseButton(m_BlueShootMouseButton))
            {
                Shoot(m_BluePortal, true);
                m_BluePortal.gameObject.SetActive(false);
            }
            if (Input.GetMouseButton(m_OrangeShootMouseButton))
            {
                Shoot(m_OrangePortal, true);
                m_OrangePortal.gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonUp(m_BlueShootMouseButton))
            {
                Shoot(m_BluePortal, false);

                AudioManager.GetAudioManager().PlaySfxAudioClip(m_PortalGunAudioClip);

                m_BluePortal.m_RedPortalPreview.SetActive(false);
                m_BluePortal.m_GreenPortalPreview.SetActive(false);
            }
            if (Input.GetMouseButtonUp(m_OrangeShootMouseButton))
            {
                Shoot(m_OrangePortal, false);

                AudioManager.GetAudioManager().PlaySfxAudioClip(m_PortalGunAudioClip);

                m_OrangePortal.m_RedPortalPreview.SetActive(false);
                m_OrangePortal.m_GreenPortalPreview.SetActive(false);
            }
        }

        if (CanAttachObject())
        {
            InteractWithObject();
        }
        if (m_attachedObjectRigidbody != null)
        {
            UpdateAttachedObject();
        }

        //Timers
        m_bulletCooldownTimer -= Time.deltaTime;
        if (m_bulletCooldownTimer < 0) m_bulletCooldownTimer = 0;


        m_CoyoteTimer -= Time.deltaTime;
        if (m_CoyoteTimer < 0) m_CoyoteTimer = 0;

        m_footstepTimer -= Time.deltaTime;
        if (m_footstepTimer < 0) m_footstepTimer = 0;


        //AUDIO
        float l_stepLength = m_PlayerFootstepsAudioSource.clip.length;

        if (m_footstepTimer <= 0 && m_CharacterController.isGrounded && m_CharacterController.velocity.magnitude > m_MinimumVelocityFootsteps)
        {
            AudioManager.GetAudioManager().PlaySourceRegular(m_PlayerFootstepsAudioSource);
            float l_playerRelativeSpeed = m_CharacterController.velocity.magnitude / m_BaseMovementSpeed;
            m_footstepTimer = l_stepLength / l_playerRelativeSpeed;
        }
    }

    bool CanShoot()
    {
        if (m_attachedObjectRigidbody != null)
        {
            return false;
        }
        else
        {
            if (m_UseBulletCooldown)
            {
                if (m_bulletCooldownTimer > 0) return false;
            }
        }
        return true;
    }
    void Shoot(Portal _portal, bool preview)
    {
        Ray l_ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_ray, out RaycastHit l_raycastHit, m_ShootMaxDistance, m_ValidShootingLayerMask.value, QueryTriggerInteraction.Ignore))
        {
            if (preview)
            {
                if (_portal.IsValidPosition(l_raycastHit.point, l_raycastHit.normal))
                {
                    _portal.m_RedPortalPreview.SetActive(false);

                    _portal.m_GreenPortalPreview.transform.position = l_raycastHit.point + _portal.m_GreenPortalPreview.transform.forward * 0.01f;
                    _portal.m_GreenPortalPreview.transform.forward = l_raycastHit.normal;
                    _portal.m_GreenPortalPreview.SetActive(true);
                }
                else
                {
                    _portal.m_GreenPortalPreview.SetActive(false);

                    _portal.m_RedPortalPreview.transform.position = l_raycastHit.point + _portal.m_RedPortalPreview.transform.forward * 0.01f;
                    _portal.m_RedPortalPreview.transform.forward = l_raycastHit.normal;
                    _portal.m_RedPortalPreview.SetActive(true);
                }

                if (Input.GetAxisRaw(m_MouseWheelAxisName) > 0)
                {
                    _portal.ResizePortal(true);
                }
                else if (Input.GetAxisRaw(m_MouseWheelAxisName) < 0)
                {
                    _portal.ResizePortal(false);
                }
            }

            if (_portal.IsValidPosition(l_raycastHit.point, l_raycastHit.normal))
                _portal.gameObject.SetActive(true);
            else
                _portal.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
            Kill();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            Portal l_portal = other.GetComponent<Portal>();
            if (CanTeleport(l_portal))
            {
                gameObject.layer = LayerMask.NameToLayer("PlayerPortal");
                float l_playerFrontDistance = new Plane(l_portal.transform.forward, l_portal.transform.position).GetDistanceToPoint(transform.position);
                if (l_playerFrontDistance < 0.0f)
                    Teleport(l_portal);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Portal"))
            gameObject.layer = LayerMask.NameToLayer("PlayerDefault");
    }

    private bool CanTeleport(Portal l_portal)
    {
        float l_playerLeftDistance = new Plane(l_portal.m_LeftPortalPlane.transform.forward, l_portal.m_LeftPortalPlane.transform.position).GetDistanceToPoint(transform.position);
        float l_playerRightDistance = new Plane(l_portal.m_RightPortalPlane.transform.forward, l_portal.m_RightPortalPlane.transform.position).GetDistanceToPoint(transform.position);

        return ((l_playerLeftDistance > 0.0f && l_playerRightDistance > 0.0f) && !l_portal.IsAlone() && !(l_portal.m_currentPortalSize == Portal.portalSize.SMALL));
    }

    private void Teleport(Portal _portal)
    {
        AudioManager.GetAudioManager().PlaySfxAudioClip(m_TeleportAudioClip);

        Vector3 l_nextPosition = transform.position + m_movementDirection * m_PortalDistance;

        Vector3 l_localPosition = _portal.m_OtherPortalTransform.InverseTransformPoint(l_nextPosition);
        Vector3 l_worldPosition = _portal.m_MirrorPortal.transform.TransformPoint(l_localPosition);

        Vector3 l_worldForward = transform.forward;
        Vector3 l_localForward = _portal.m_OtherPortalTransform.InverseTransformDirection(l_worldForward);
        l_worldForward = _portal.m_MirrorPortal.transform.TransformDirection(l_localForward);

        m_CharacterController.enabled = false;
        transform.position = l_worldPosition;
        transform.rotation = Quaternion.LookRotation(l_worldForward);
        m_Yaw = transform.rotation.eulerAngles.y;
        m_CharacterController.enabled = true;
    }

    private bool CanAttachObject()
    {
        return (!m_attachedObject);
    }

    private void InteractWithObject()
    {
        if (Input.GetKeyDown(m_GrabKeyCode))
        {
            Ray l_ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(l_ray, out RaycastHit l_raycastHit, m_GrabMaxDistance, m_ValidInteractLayerMask.value, QueryTriggerInteraction.Ignore))
            {
                if (l_raycastHit.collider.CompareTag("Cube") || l_raycastHit.collider.CompareTag("Turret") || l_raycastHit.collider.CompareTag("RefractionCube"))
                    AttachObject(l_raycastHit.rigidbody);

                if (l_raycastHit.collider.CompareTag("InteractableButton"))
                    l_raycastHit.collider.GetComponent<CompanionSpawner>().Spawn();
            }
        }
    }

    private void AttachObject(Rigidbody _rigidbody)
    {
        m_attachingObject = true;
        m_attachedObjectRigidbody = _rigidbody;
        m_attachedObjectRigidbody.GetComponent<TeleportBase>().SetAttachedObject(true);
        m_startAttachingObjectPosition = _rigidbody.transform.position;
        m_startAttachingObjectRotation = _rigidbody.transform.rotation;
        m_attachingCurrentTime = 0.0f;
        m_attachedObject = false;
    }

    private void UpdateAttachedObject()
    {
        if (m_attachingObject)
        {
            m_attachingCurrentTime += Time.deltaTime;
            float l_pct = Mathf.Min(1.0f, m_attachingCurrentTime / m_AttachingTime);
            Vector3 l_position = Vector3.Lerp(m_startAttachingObjectPosition, m_GripTransform.position, l_pct);
            float l_distance = Vector3.Distance(l_position, m_GripTransform.position);
            float l_rotationPct = 1.0f - Mathf.Min(1.0f, l_distance / m_AttachingObjectRotationDistanceLerp);
            Quaternion l_rotation = Quaternion.Lerp(m_startAttachingObjectRotation, m_GripTransform.rotation, l_rotationPct);
            m_attachedObjectRigidbody.MovePosition(l_position);
            m_attachedObjectRigidbody.MoveRotation(l_rotation);
            if (l_pct == 1.0f)
            {
                m_attachingObject = false;
                m_attachedObject = true;
                m_attachedObjectRigidbody.transform.SetParent(m_GripTransform);
                m_attachedObjectRigidbody.transform.localPosition = Vector3.zero;
                m_attachedObjectRigidbody.transform.localRotation = Quaternion.identity;
                m_attachedObjectRigidbody.linearVelocity = Vector3.zero;
                m_attachedObjectRigidbody.angularVelocity = Vector3.zero;
                m_attachedObjectRigidbody.isKinematic = true;
            }

        }

        if (Input.GetMouseButtonDown(m_ThrowMouseButton))
        {
            ThrowObject(m_ThrowForce);
            m_bulletCooldownTimer = m_BulletCooldown;
        }
        else if (Input.GetMouseButtonDown(m_DropMouseButton))
        {
            ThrowObject(0.0f);
            m_bulletCooldownTimer = m_BulletCooldown;
        }
    }

    public void ThrowObject(float force)
    {
        if (m_attachedObjectRigidbody == null)
            return;

        m_attachedObjectRigidbody.isKinematic = false;
        m_attachedObjectRigidbody.AddForce(m_PitchController.forward * force, m_ForceMode);
        m_attachedObjectRigidbody.transform.SetParent(null);
        m_attachingObject = false;
        m_attachedObject = false;
        m_attachedObjectRigidbody.GetComponent<TeleportBase>().SetAttachedObject(false);
        m_attachedObjectRigidbody = null;
    }

    public void Kill()
    {
        if (!CanvasController.m_IsInGameOver)
        {
            AudioManager.GetAudioManager().PlaySfxAudioClip(m_DeathAudioClip);

            m_Camera.transform.localPosition = -Vector3.forward * 5;
            CanvasController.m_IsInGameOver = true;
            GameManager.GetGameManager().m_Fade.FadeIn(() =>
            {
                CanvasController.GetCanvasController().EnableGameOverCanvas();
                m_Camera.transform.localPosition = Vector3.zero;
            });
        }
    }

    public void HasTouchedCheckPoint()
    {
        CheckpointController.GetCheckPointController().IncreaseIndex();

        Vector3 l_InitialRotation = CheckpointController.GetCheckPointController().GetPlayerPosition().eulerAngles;
        m_InitialYaw = l_InitialRotation.y;
        m_InitialPitch = l_InitialRotation.x;

        m_RespawnPosition = CheckpointController.GetCheckPointController().GetPlayerPosition().position;
        m_StartRotation = CheckpointController.GetCheckPointController().GetPlayerPosition().rotation;
    }

    public void Set()
    {
        GameManager.GetGameManager().SetToRespawnableList(this);
    }
    public void Reset()
    {
        m_CharacterController.enabled = false;
        transform.position = m_RespawnPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;

        m_Yaw = m_InitialYaw;
        m_Pitch = m_InitialPitch;

        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);

        m_BluePortal.m_RedPortalPreview.SetActive(false);
        m_BluePortal.m_GreenPortalPreview.SetActive(false);
        m_OrangePortal.m_RedPortalPreview.SetActive(false);
        m_OrangePortal.m_GreenPortalPreview.SetActive(false);

        m_BluePortal.ResetPortalSize();
        m_OrangePortal.ResetPortalSize();
    }

    private void ManageGravity()
    {
        m_CurrentGravityMultiplier = 1.0f;

        if (m_VerticalSpeed < 0.0f)
        {
            m_CurrentGravityMultiplier = m_GravityMultiplier;
        }
    }

    public void SetSpawnpoint(Transform newPosition)
    {
        m_RespawnPosition = newPosition.transform.position;
    }

    public Vector3 GetSpawnpoint()
    {
        return m_RespawnPosition;
    }

    public void ResetBulletCooldownTimer()
    {
        m_bulletCooldownTimer = m_BulletCooldown;
    }

    public bool PlayerHasRigidbodyAttached(Rigidbody rigidbody)
    {
        return (m_attachedObjectRigidbody == rigidbody);
    }
}