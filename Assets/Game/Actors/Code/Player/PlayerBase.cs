using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------------------------------
// Name	:	PlayerBase
// Desc  :  Every player script inherits from this one. It keeps all references to player scripts
//          So that they can all inherit the references easily
// ------------------------------------------------------------------------------------------
public class PlayerBase : MonoBehaviour
{
    Player m_Player;
    public Player Player
    {
        get
        {
            if (m_Player == null)
            {
				m_Player = transform.root.GetComponentInChildren<Player>();
            }

            return m_Player;
        }
    }

    PlayerController m_PlayerController;
    public PlayerController PlayerController
    {
        get
        {
            if (m_PlayerController == null)
            {
				m_PlayerController = transform.root.GetComponentInChildren<PlayerController>();
            }
            return m_PlayerController;
        }
    }

	PlayerBar m_PlayerBar;
	public PlayerBar PlayerBar
	{
		get
		{
			if (m_PlayerBar == null)
			{
				m_PlayerBar = transform.root.GetComponentInChildren<PlayerBar>();
			}
			
			return m_PlayerBar;
		}
	}

    CarHandling m_CarHandling;
    public CarHandling CarHandling
    {
        get
        {
            if (m_CarHandling == null)
            {
				m_CarHandling = transform.root.GetComponentInChildren<CarHandling>();
            }
            return m_CarHandling;
        }
    }

    CarVisuals m_CarVisuals;
    public CarVisuals CarVisuals
    {
        get
        {
            if (m_CarVisuals == null)
            {
				m_CarVisuals = transform.root.GetComponentInChildren<CarVisuals>();
            }
            return m_CarVisuals;
        }
    }

	Transform m_CarTransform;
	public Transform CarTransform
	{
		get
		{
			if (m_CarTransform == null)
			{
				m_CarTransform = CarHandling.transform;
			}
			return m_CarTransform;
		}
	}

    PhotonView m_PhotonView;
    public PhotonView PhotonView
    {
        get
        {
            if (m_PhotonView == null)
            {
				m_PhotonView = transform.root.GetComponentInChildren<PhotonView>();
            }
            return m_PhotonView;
        }
    }

	// Camera variable used to determine wether or not the player is in our view.
	// If it is outside of our view, we may decide to not display it.
	private Camera m_Camera;
	public Camera Camera
	{
		get
		{
			if (m_Camera == null)
			{
				m_Camera = Camera.main;  // Select the current camera which is being used by the player.
			}
			return m_Camera;
		}
	}


}
