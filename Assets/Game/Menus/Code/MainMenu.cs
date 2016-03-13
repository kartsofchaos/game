using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Draws a simple main menu and connects to Photon when a button is pressed.
/// </summary>
public class MainMenu : MonoBehaviour
{
    private GameObject playerPropertiesGO;
    private PlayerProperties playerProperties;

	public Text connectionText;
	public Text nameText; 

	void Start ()
    {
        CreatePlayerProperties();
    }

    /// <summary>
    /// Creates the playerproperties object which holdes the information and 
    /// does not get destroyed when loading the maps
    /// </summary>
    private void CreatePlayerProperties()
    {
        playerPropertiesGO = new GameObject("PlayerProperties");
        playerProperties = playerPropertiesGO.AddComponent<PlayerProperties>();
    }

	private void Connect ()
    {
        // If we are not connected, connect to Photon via our MultiplayerConnector
        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            MultiplayerConnector.Instance.Connect();
        }
	}

    private void DrawNameTextField()
    {
		playerProperties.Name = nameText.text;
        // Filter all unallowed characters
        playerProperties.Name = Regex.Replace(playerProperties.Name, @"[^a-zA-Z0-9]", "");
    }

    private void DrawMenuLabel()
    {
        // While the player isnt connected type the press any key text. Otherwise print the connection status
        switch (PhotonNetwork.connectionState)
        {
            case ConnectionState.Disconnected:
				connectionText.text = "Pick a name and press enter to connect";
				break;
            default:
				connectionText.text = "Connecting...\n" + PhotonNetwork.connectionStateDetailed;
                break;
        }
    }

    void OnGUI()
    {
        // Check if return/enter has been pressed
        Event e = Event.current;
        if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
            Connect();
		
        DrawMenuLabel();
		DrawNameTextField ();
    }
}
