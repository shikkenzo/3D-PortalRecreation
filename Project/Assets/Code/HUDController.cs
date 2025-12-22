using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Image m_Image;
    public Sprite m_FullPortals, m_EmptyPortals, m_EmptyOrange, m_EmptyBlue;
    GameObject m_bluePortal, m_orangePortal;

    private void OnEnable()
    {
        GameManager.OnPlayerSet += SetPlayer;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSet -= SetPlayer;
    }

    private void Start()
    {
        m_Image.sprite = m_FullPortals;
    }

    private void Update()
    {
        if (m_bluePortal == null || m_orangePortal == null)
            return;

        Sprite l_sprite = m_FullPortals;

        if (!m_bluePortal.activeInHierarchy && m_orangePortal.activeInHierarchy)
            l_sprite = m_EmptyOrange;

        else if (m_bluePortal.activeInHierarchy && !m_orangePortal.activeInHierarchy)
            l_sprite = m_EmptyBlue;

        else if (m_bluePortal.activeInHierarchy && m_orangePortal.activeInHierarchy)
            l_sprite = m_EmptyPortals;

        if (m_Image.sprite != l_sprite)
            m_Image.sprite = l_sprite;
    }


    void SetPlayer()
    {
        m_bluePortal = GameManager.GetGameManager().GetPlayer().m_BluePortal.gameObject;
        m_orangePortal = GameManager.GetGameManager().GetPlayer().m_OrangePortal.gameObject;
    }
}