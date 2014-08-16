using UnityEngine;
using System.Collections;

/// <summary>
/// Draws a simple main menu and connects to Photon when a button is pressed.
/// </summary>
public class MainMenu : MonoBehaviour
{
    // The font used to write in the menu
    public Font TextFont;

    GUIStyle TextStyle;

	void Start ()
    {
	}
	
	void Update ()
    {
        // If we are not connected, and any button is pressed, connect to Photon via our MultiplayerConnector
        if (PhotonNetwork.connectionState == ConnectionState.Disconnected && Input.anyKeyDown == true)
        {
            MultiplayerConnector.Instance.Connect();
        }
	}

    /// <summary>
    /// Sets up the text style for the menu
    /// </summary>
    void LoadStyles()
    {
        if (TextStyle == null)
        {
            TextStyle = new GUIStyle("Label");
            TextStyle.font = TextFont;
            TextStyle.fontSize = 40;
            TextStyle.alignment = TextAnchor.MiddleCenter;
        }
    }

    void OnGUI()
    {
        LoadStyles();

        // Make the color fade in and out to add an animation to the text in then menu
        GUI.color = new Color(1f, 1f, 1f, Mathf.Sin(Time.realtimeSinceStartup * 4f) * 0.4f + 0.6f);

        float labelWidth = 600;
        float labelHeight = 100;

        string label = "";

        // While the player isnt connected type the press any key text. Otherwise print the connection status
        switch (PhotonNetwork.connectionState)
        {
            case ConnectionState.Disconnected:
                label = "Press any key to connect";
                break;
            default:
                label = "Connecting...\n" + PhotonNetwork.connectionStateDetailed;
                break;
        }

        // Add the label to the screen
        GUI.Label(new Rect((Screen.width - labelWidth) * 0.5f, (Screen.height - labelHeight) * 0.5f + 250, labelWidth, labelHeight), label, TextStyle);
    }
}
