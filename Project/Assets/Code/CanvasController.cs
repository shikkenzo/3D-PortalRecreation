using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject m_StopPanel, m_GameOverPanel, m_HudPanel, m_FinalPanel;
    public static bool m_IsInGameOver, m_IsInPause;

    static CanvasController m_canvasController;

    private void OnEnable()
    {
        FinalTrigger.OnGameCompleted += OnPlayerFinished;
    }

    private void OnDisable()
    {
        FinalTrigger.OnGameCompleted -= OnPlayerFinished;
    }

    private void Awake()
    {
        if (m_canvasController != null)
        {
            Destroy(gameObject);
            return;
        }

        m_canvasController = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_IsInGameOver = false;
        m_IsInPause = false;

        if (m_StopPanel != null)
            DisableStopCanvas();

        if (m_GameOverPanel != null)
            DisableFinalsCanvas();
    }

    private void Update()
    {
        if (m_StopPanel != null && m_GameOverPanel != null)
            if (Input.GetKeyDown(KeyCode.Escape) && !m_IsInGameOver)
            {
                if (!m_IsInPause)
                    EnableStopCanvas();
                else
                    DisableStopCanvas();
            }
    }

    public static CanvasController GetCanvasController()
    {
        return m_canvasController;
    }

    public void EnableStopCanvas()
    {
        AudioManager.GetAudioManager().SetMusicVolume(0.5f);

        GameManager.GetGameManager().GetPlayer().ResetBulletCooldownTimer();
        if (m_StopPanel != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0.0f;
            m_IsInPause = true;

            m_HudPanel.SetActive(false);
            m_StopPanel.SetActive(true);
        }
    }

    public void DisableStopCanvas()
    {
        AudioManager.GetAudioManager().SetMusicVolume(1.0f);

        if (m_StopPanel != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1.0f;
            m_IsInPause = false;

            m_HudPanel.SetActive(true);
            m_StopPanel.SetActive(false);
        }
    }

    public void EnableGameOverCanvas()
    {
        AudioManager.GetAudioManager().SetMusicVolume(0.5f);

        GameManager.GetGameManager().GetPlayer().ResetBulletCooldownTimer();
        if (m_GameOverPanel != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0.0f;

            m_HudPanel.SetActive(false);
            m_GameOverPanel.SetActive(true);
        }
    }

    public void DisableFinalsCanvas()
    {
        AudioManager.GetAudioManager().SetMusicVolume(1.0f);

        if (m_GameOverPanel != null && m_FinalPanel != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1.0f;

            m_HudPanel.SetActive(true);
            m_StopPanel.SetActive(false);
            m_GameOverPanel.SetActive(false);
            m_FinalPanel.SetActive(false);
        }
    }

    public void EnableFinalCanvas()
    {
        AudioManager.GetAudioManager().SetMusicVolume(0.5f);

        GameManager.GetGameManager().GetPlayer().ResetBulletCooldownTimer();
        if (m_FinalPanel != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0.0f;
            m_IsInGameOver = true;

            m_HudPanel.SetActive(false);
            m_FinalPanel.SetActive(true);
        }
    }

    void OnPlayerFinished()
    {
        GameManager.GetGameManager().m_Fade.FadeIn(() =>
        {
            EnableFinalCanvas();
        });
    }

    public void Restart()
    {
        m_IsInPause = false;
        m_IsInGameOver = false;
        DisableFinalsCanvas();

        GameManager.GetGameManager().RestartLevel();
    }

    public void ResetLevel()
    {
        CheckpointController.GetCheckPointController().ResetCheckpoint();
        Restart();
    }

    public void Exit()
    {
        GameManager.GetGameManager().Exit();
    }
}