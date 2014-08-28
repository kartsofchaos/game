using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------------------------------
// Name	:	HUDBase
// Desc  :  Every HUD script inherits from this one. It keeps all references to the player
//          and carscripts so that they can all inherit the references easily
// ------------------------------------------------------------------------------------------
public class HUDBase : MonoBehaviour
{
    private Player m_Player;
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
	
	private PlayerController m_PlayerController;
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

    private CarHandling m_CarHandling;
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

    private CarVisuals m_CarVisuals;
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

    private Transform m_CarTransform;
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
}
