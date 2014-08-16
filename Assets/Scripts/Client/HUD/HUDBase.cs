using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------------------------------
// Name	:	HUDBase
// Desc  :  Every HUD script inherits from this one. It keeps all references to the player
//          and carscripts so that they can all inherit the references easily
// ------------------------------------------------------------------------------------------
public class HUDBase : MonoBehaviour
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
	
	PlayerController m_Controller;
	public PlayerController PlayerController
	{
		get
		{
			if (m_Controller == null)
			{
				m_Controller = transform.root.GetComponentInChildren<PlayerController>();
			}
			return m_Controller;
		}
	}
	
	CarHandling m_Handling;
	public CarHandling CarHandling
	{
		get
		{
			if (m_Handling == null)
			{
				m_Handling = transform.root.GetComponentInChildren<CarHandling>();
			}
			return m_Handling;
		}
	}
	
	CarVisuals m_Visuals;
	public CarVisuals CarVisuals
	{
		get
		{
			if (m_Visuals == null)
			{
				m_Visuals = transform.root.GetComponentInChildren<CarVisuals>();
			}
			return m_Visuals;
		}
	}
}
