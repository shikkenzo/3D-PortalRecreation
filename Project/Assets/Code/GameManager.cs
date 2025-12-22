using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform m_DestroyObjects;
    public Fade m_Fade;

    PlayerController m_Player = null;
    List<IRespawnable> m_RespawnableObjects;

    public delegate void OnRespawnDelegate();
    public static OnRespawnDelegate OnSetRespawn;

    static GameManager m_GameManager;

    public static Action OnPlayerSet;

    private void Awake()
    {
        if (m_GameManager != null)
        {
            GameManager.Destroy(gameObject);
            return;
        }
        m_GameManager = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_RespawnableObjects = new List<IRespawnable>();
        OnSetRespawn?.Invoke();
    }

    public static GameManager GetGameManager()
    {
        return m_GameManager;
    }

    void DestroyInGameObjects()
    {
        for (int i = 0; i < m_DestroyObjects.childCount; ++i)
            GameObject.Destroy(m_DestroyObjects.GetChild(i).gameObject);
    }

    public void RestartLevel()
    {
        DestroyInGameObjects();

        foreach (IRespawnable r in m_RespawnableObjects)
            r.Reset();

        m_Fade.FadeOut(() =>
        {
            m_Fade.gameObject.SetActive(false);
        });
    }

    public PlayerController GetPlayer()
    {
        return m_Player;
    }

    public void SetPlayer(PlayerController Player)
    {
        m_Player = Player;
        OnPlayerSet?.Invoke();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetToRespawnableList(IRespawnable _object)
    {
        m_RespawnableObjects.Add(_object);
    }
}