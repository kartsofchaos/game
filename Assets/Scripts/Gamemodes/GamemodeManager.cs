using UnityEngine;
using System.Collections;

public enum Gamemode
{
    CaptureTheFlag,
    //Deathmatch,
    //TeamDeathmatch,
    Count,
}

/// <summary>
/// The GamemodeManager handles loading and unloading of the required game modes
/// and provides an easy access to the currently played game mode
/// </summary>
public class GamemodeManager : MonoBehaviour
{
    /// <summary>
    /// Quick access to the one instance of the GamemodeManager that exists in the scene
    /// </summary>
    public static GamemodeManager Instance;

    /// <summary>
    /// The currently played game mode
    /// </summary>
    public Gamemode SelectedGamemode;

    /// <summary>
    /// These are all the game modes that exist
    /// </summary>
    GamemodeBase[] m_Gamemodes;

    /// <summary>
    /// Quick access to the currently played game mode component
    /// </summary>
    public static GamemodeBase CurrentGamemode
    {
        get
        {
            if (Instance == null)
            {
                return null;
            }

            return Instance.GetCurrentGamemode();
        }
    }

    void Awake()
    {
        Instance = this;

        FindGamemodes();
        //FindCurrentMapMode();

        InitiateSelectedGamemode();
    }

    /// <summary>
    /// Find the references to all the game modes that exist, so we can start working with them
    /// </summary>
    void FindGamemodes()
    {
        m_Gamemodes = new GamemodeBase[(int)Gamemode.Count];
        m_Gamemodes[0] = GetComponent<GamemodeCaptureTheFlag>();
        //m_Gamemodes[1] = GetComponent<GamemodeDeathmatch>();
        //m_Gamemodes[2] = GetComponent<GamemodeTeamDeathmatch>();
    }

    /// <summary>
    /// This function checks the current room for it's custom properties to see which game mode 
    /// is currently being played
    /// </summary>
    void FindCurrentMapMode()
    {
        //Check if we are actually connected to a room
        if (PhotonNetwork.room == null)
        {
            return;
        }

        MapQueueEntry map = MapQueue.GetCurrentMap();

        if (map.Equals(MapQueueEntry.None) == false)
        {
            SelectedGamemode = map.Mode;
        }
    }

    /// <summary>
    /// Initiates the selected gamemode.
    /// </summary>
    void InitiateSelectedGamemode()
    {
        for (int i = 0; i < m_Gamemodes.Length; ++i)
        {
            if (i == (int)SelectedGamemode)
            {
                m_Gamemodes[i].OnSetup();
            }
            else
            {
                m_Gamemodes[i].OnTearDown();
            }
        }
    }

    /// <summary>
    /// Retrieve any of the game mode components
    /// </summary>
    public GamemodeBase GetGamemode(Gamemode mode)
    {
        return m_Gamemodes[(int)mode];
    }

    /// <summary>
    /// Quick access to the currently played game mode component
    /// </summary>
    public GamemodeBase GetCurrentGamemode()
    {
        return GetGamemode(SelectedGamemode);
    }
}
