using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public List<Transform> m_CheckpointsList = new List<Transform>();

    int m_Index = 0;

    static CheckpointController m_Instance;

    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
            m_Instance = this;
    }

    private void Start()
    {
        m_Index = 0;
    }

    public static CheckpointController GetCheckPointController()
    {
        return m_Instance;
    }

    public Transform GetPlayerPosition()
    {
        return m_CheckpointsList[m_Index].GetChild(0);
    }

    public void IncreaseIndex()
    {
        m_Index++;
    }

    public void ResetCheckpoint()
    {
        m_Index = -1;

        foreach (Transform t in m_CheckpointsList)
            t.GetComponent<CheckpointBase>().Reset();

        GameManager.GetGameManager().GetPlayer().HasTouchedCheckPoint();
    }
}