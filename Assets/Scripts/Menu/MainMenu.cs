using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Draws a simple main menu and connects to Photon when a button is pressed.
/// </summary>
public class MainMenu : MonoBehaviour
{
    // The font used to write in the menu
    public Font TextFont;
    private float labelWidth = 600;
    private float labelHeight = 100;
    private float nameWidth = 500;
    private float nameHeight = 70;

    private Rect labelRect;
    private Rect nameRect;

    private GUIStyle labelStyle;
    private GUIStyle textFieldStyle;

    private GameObject playerPropertiesGO;
    private PlayerProperties playerProperties;

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

    /// <summary>
    /// Sets up the text style for the menu
    /// </summary>
    private void LoadStyles()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle("Label");
            labelStyle.font = TextFont;
            labelStyle.fontSize = 40;
            labelStyle.alignment = TextAnchor.MiddleCenter;
        }
        if (textFieldStyle == null)
        {
            textFieldStyle = new GUIStyle("Label");
            textFieldStyle.font = TextFont;
            textFieldStyle.fontSize = 35;
            textFieldStyle.alignment = TextAnchor.MiddleCenter;
            // Set background
            textFieldStyle.normal.background = GUI.skin.textField.normal.background;
            textFieldStyle.border = GUI.skin.textField.border;
        }
    }

    private void DrawNameTextField()
    {
        playerProperties.Name = GUI.TextField(nameRect, playerProperties.Name, 16, textFieldStyle);
        // Filter all unallowed characters
        playerProperties.Name = Regex.Replace(playerProperties.Name, @"[^a-zA-Z0-9]", "");
    }

    private void DrawMenuLabel()
    {
        string label = "";

        // While the player isnt connected type the press any key text. Otherwise print the connection status
        switch (PhotonNetwork.connectionState)
        {
            case ConnectionState.Disconnected:
                label = "Pick a name and press enter to connect";
                break;
            default:
                label = "Connecting...\n" + PhotonNetwork.connectionStateDetailed;
                break;
        }

        // Add the label to the screen
        GUI.Label(labelRect, label, labelStyle);
    }

    void OnGUI()
    {
        // Check if return/enter has been pressed
        Event e = Event.current;
        if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
            Connect();

        // Placement variables
        float labelLeft = (Screen.width - labelWidth) * 0.5f;
        float labelTop = (Screen.height - labelHeight) * 0.5f + 250;
        float nameLeft = labelLeft + (labelWidth - nameWidth) / 2;
        float nameTop = labelTop - labelHeight * 1.5f;

        labelRect = new Rect(labelLeft, labelTop, labelWidth, labelHeight);
        nameRect = new Rect(nameLeft, nameTop, nameWidth, nameHeight);

        LoadStyles();

        // Make the color fade in and out to add an animation to the text in then menu
        // GUI.color = new Color(1f, 1f, 1f, Mathf.Sin(Time.realtimeSinceStartup * 4f) * 0.4f + 0.6f);

        DrawMenuLabel();
        DrawNameTextField();
    }
}
