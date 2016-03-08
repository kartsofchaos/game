using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------------------------------
// Name	:	SkillBase
// Desc  :  Every Skill script inherits from this one. It keeps all references to player,
//          all different transforms etc.
// ------------------------------------------------------------------------------------------
public class SkillBase : MonoBehaviour
{
    Player m_Player;
    public Player Player
    {
        get
        {
            if (m_Player == null)
            {
                m_Player = transform.GetComponentInChildren<Player>();
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
                m_Controller = transform.GetComponentInChildren<PlayerController>();
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
                m_Handling = transform.GetComponentInChildren<CarHandling>();
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
                m_Visuals = transform.GetComponentInChildren<CarVisuals>();
            }
            return m_Visuals;
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

    Rigidbody m_CarRigidBody;
    public Rigidbody CarRigidBody
    {
        get
        {
            if (m_CarRigidBody == null)
            {
                m_CarRigidBody = CarHandling.GetComponent<Rigidbody>();
            }
            return m_CarRigidBody;
        }
    }
}
