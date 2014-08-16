using UnityEngine;
using System.Collections;

/// <summary>
/// This class defines the specific pickup behavior for the flag
/// It also handles flag drops, returns and captures
/// </summary>
public class PickupFlag : PickupBase
{

    /// <summary>
    /// We assume there is only one flag per team here. This property is for easy access to the red flag
    /// </summary>
    public static PickupFlag RedFlag;

    /// <summary>
    /// And we do the same for the blue flag
    /// </summary>
    public static PickupFlag BlueFlag;

    /// <summary>
    /// Gets the flag of the specified team
    /// </summary>
    /// <param name="team">The team.</param>
    /// <returns>Flag object of the specified team</returns>
    public static PickupFlag GetFlag(Team team)
    {
        if (team == Team.Blue)
        {
            return BlueFlag;
        }

        return RedFlag;
    }

    /// <summary>
    /// Gets the opposing team for the one specified
    /// </summary>
    /// <param name="team">The team we want the opponent for</param>
    /// <returns>Opposing team</returns>
    public static Team GetOtherTeam(Team team)
    {
        if (team == Team.Blue)
        {
            return Team.Red;
        }

        return Team.Blue;
    }

    /// <summary>
    /// Define which team this flag belongs to
    /// </summary>
    public Team Team;

    /// <summary>
    /// How long should the flag remain dropped in the field before being returned automatically
    /// </summary>
    public float ReturnTime;

    float m_ReturnTimer;
    Vector3 m_HomePosition;
    Player m_CarryingPlayer;

    void Awake()
    {
        m_HomePosition = transform.position;

        if (Team == Team.Blue)
        {
            BlueFlag = this;
        }
        else
        {
            RedFlag = this;
        }
    }

    void LateUpdate()
    {
        HandleFlagDrop();
        UpdatePosition();
        UpdateReturnTimer();
    }

    /// <summary>
    /// This handles the automatic return of a flag after a set amount of time
    /// </summary>
    void UpdateReturnTimer()
    {
        if (m_ReturnTimer == -1f)
        {
            return;
        }

        m_ReturnTimer -= Time.deltaTime;

        if (m_ReturnTimer <= 0f)
        {
            m_ReturnTimer = -1f;
            ReturnFlag();
        }
    }

    /// <summary>
    /// This updates the position of the flag when it is carried by a player
    /// The flag will be always behind the carrying player
    /// </summary>
    void UpdatePosition()
    {
        if (m_CarryingPlayer == null)
        {
            return;
        }

        transform.GetChild(0).position = m_CarryingPlayer.transform.GetChild(0).position;
    }

    /// <summary>
    /// If the carrying player died, send the drop flag event to all players
    /// </summary>
    void HandleFlagDrop()
    {
        if (m_CarryingPlayer == null)
        {
            return;
        }

        if (m_CarryingPlayer.Health <= 0)
        {
            DropFlag();
        }
    }


    /// <summary>
    /// Determines whether this instance is at the home base
    /// </summary>
    /// <returns></returns>
    public bool IsHome()
    {
        return transform.position == m_HomePosition;
    }

    void DropFlag()
    {
        if (PhotonNetwork.offlineMode == true)
        {
            OnDrop(transform.position);
        }
        else
        {
            if (PhotonNetwork.isMasterClient == true)
            {
                PhotonView.RPC("OnDrop", PhotonTargets.AllBuffered, new object[] { transform.position });
            }
        }
    }

    void ReturnFlag()
    {
        if (PhotonNetwork.offlineMode == true)
        {
            OnReturn();
        }
        else
        {
            if (PhotonNetwork.isMasterClient == true)
            {
                PhotonView.RPC("OnReturn", PhotonTargets.AllBuffered);
            }
        }
    }

    [RPC]
    void OnDrop(Vector3 position)
    {
        m_CarryingPlayer = null;
        transform.position = position;
        m_ReturnTimer = ReturnTime;
    }

    [RPC]
    void OnReturn()
    {
        transform.position = m_HomePosition;
    }

    public override bool CanBePickedUpBy(Player p)
    {
        //If the flag is at its home position, only the enemy team can grab it
        if (IsHome() == true)
        {
            return p.Team != Team;
        }

        //If another player is already carrying the flag, no one else can grab it
        if (m_CarryingPlayer != null)
        {
            return false;
        }

        return true;
    }

    public override void OnPickup(Player p)
    {
        if (p.Team == Team)
        {
            //Since the flag and the ship are in the same team, it should be returned in case it is not at its home position
            if (IsHome() == false)
            {
                ReturnFlag();
            }
        }
        else
        {
            m_CarryingPlayer = p;
        }
    }
}