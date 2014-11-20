using UnityEngine;
using System.Collections;


/// <summary>
/// This script wraps photons connect functionality and defines several of the callbacks Photon invokes
/// It is used to connect to rooms and games
/// </summary>
public class MultiplayerConnector : MonoBehaviour
{
    public static bool IsHost = false;
    public static bool QuitOnLogout = false;

    public static bool IsConnected
    {
        get
        {
            return PhotonNetwork.offlineMode == false && PhotonNetwork.connectionState == ConnectionState.Connected;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Connect()
    {
        if (PhotonNetwork.connectionState != ConnectionState.Disconnected)
        {
            return;
        }

        try
        {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }
        catch
        {
            Debug.LogWarning("Couldn't connect to server");
        }
    }

    void OnConnectedToPhoton()
    {
        if (IsHost == true)
        {
            return;
        }
    }

    /// <summary>
    /// When we joined the lobby after connecting to Photon, we want to immediately join the demo room, or create it if it doesn't exist
    /// </summary>
    void OnJoinedLobby()
    {
        if (IsHost == true)
        {
            return;
        }

        if (QuitOnLogout == true)
        {
            Application.Quit();
            return;
        }

        Debug.Log("OnJoinedLobby");

        if (Application.loadedLevelName == "MainMenu")
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.maxPlayers = 20;

            PhotonNetwork.JoinOrCreateRoom("Tutorial", roomOptions, TypedLobby.Default);
        }
        else
        {
            //If we join the lobby while not being in the MainMenu scene, something went wrong and we disconnect from Photon
            PhotonNetwork.Disconnect();
        }
    }

    void OnPhotonCreateRoomFailed()
    {
        if (IsHost == true)
        {
            return;
        }
    }

    void OnPhotonJoinRoomFailed()
    {
        if (IsHost == true)
        {
            return;
        }
    }

    /// <summary>
    /// Called when we successfully joined a room. It is also called if we created the room.
    /// </summary>
    void OnJoinedRoom()
    {
        //Pause the message queue. While unity is loading a new level, updates from Photon are skipped.
        //So we have to tell Photon to wait until we resume the queue again after the level is loaded. See MultiplayerConnector.OnLevelWasLoaded
        PhotonNetwork.isMessageQueueRunning = false;
        Application.LoadLevel("MedievalTown");
    }

    // <summary>
    /// Called when we created a Photon room.
    /// </summary>
    void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");

        //When we create the room we set several custom room properties that get synchronized between all players
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

        //If we don't set the scores to 0 here, players would get errors when trying to access the score properties
        properties.Add(NetworkingConstants.BlueScore, (int)0);
		properties.Add(NetworkingConstants.RedScore, (int)0);

        //PhotonNetwork.time is synchronized between all players, so by using it as the start time here, all players can calculate how long the game ran
		properties.Add(NetworkingConstants.StartTime, PhotonNetwork.time);

        PhotonNetwork.room.SetCustomProperties(properties);
    }

    /// <summary>
    /// Called by Unity after Application.LoadLevel is completed
    /// </summary>
    /// <param name="level">The index of the level that was loaded</param>
    void OnLevelWasLoaded(int level)
    {
        //Resume the Photon message queue so we get all the updates.
        //All updates that were sent during the level load were cached and are dispatched now so we can handle them properly.
        PhotonNetwork.isMessageQueueRunning = true;
    }

    void OnDisconnectedFromPhoton()
    {
        if (Application.loadedLevelName != "MainMenu")
        {
            Application.LoadLevel("MainMenu");
        }
    }

    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        if (IsHost == true)
        {
            return;
        }

        Debug.LogWarning("OnFailedToConnectToPhoton: " + cause);
    }

    void OnConnectionFail(DisconnectCause cause)
    {
        if (IsHost == true)
        {
            return;
        }

        Debug.LogWarning("OnConnectionFail: " + cause);
    }

    void OnLeftRoom()
    {
        if (IsHost == true)
        {
            return;
        }

        Debug.Log("OnLeftRoom");
    }

    public static MultiplayerConnector _instance;
    public static MultiplayerConnector Instance
    {
        get
        {
            if (_instance == null)
            {
                CreateInstance();
            }

            return _instance;
        }
    }

    public static void CreateInstance()
    {
        if (_instance == null)
        {
            GameObject connectorObject = GameObject.Find("MultiplayerConnector");

            if (connectorObject == null)
            {
                connectorObject = new GameObject("MultiplayerConnector");
                connectorObject.AddComponent<MultiplayerConnector>();
            }

            _instance = connectorObject.GetComponent<MultiplayerConnector>();
        }
    }
}
