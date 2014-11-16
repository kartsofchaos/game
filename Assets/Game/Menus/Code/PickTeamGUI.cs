using UnityEngine;
using System.Collections;

/// <summary>
/// This class draws the team selection menu from the beginning of a match
/// </summary>
public class PickTeamGUI : MonoBehaviour
{
    public Font ButtonFont;
    public Texture2D ButtonBackground;

    GUIStyle m_PickButtonStyle;
    /*
    void Update()
    {
        if (Input.GetButtonDown("BlueTeamButton") == true)
        {
            ChooseTeam(Team.Blue);
        }

        if (Input.GetButtonDown("RedTeamButton") == true)
        {
            ChooseTeam(Team.Red);
        }
    }
    */
    void ChooseTeam(Team team)
    {
        GetComponent<PlayerSpawner>().CreateLocalPlayer(team);
        enabled = false;
    }

    void OnGUI()
    {
        LoadStyles();

        GUILayout.BeginArea(new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.5f));
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(GetButtonLabel(Team.Blue), m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f - 20), GUILayout.Height(Screen.height * 0.5f - 20)))
                {
                    ChooseTeam(Team.Blue);
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(GetButtonLabel(Team.Red), m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f - 20), GUILayout.Height(Screen.height * 0.5f - 20)))
                {
                    ChooseTeam(Team.Red);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    string GetButtonLabel(Team team)
    {
        //    GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        int playerCount = 0;
        /*
                for (int i = 0; i < shipObjects.Length; ++i)
                {
                    if (shipObjects[i].GetComponent<Ship>() != null && shipObjects[i].GetComponent<Player>().Team == team)
                    {
                        playerCount++;
                    }
                }
        */
        string label = team.ToString() + " Team\n";
        label += playerCount.ToString();

        if (playerCount == 1)
        {
            label += " Player";
        }
        else
        {
            label += " Players";
        }

        return label;
    }

    void LoadStyles()
    {
        if (m_PickButtonStyle == null)
        {
            m_PickButtonStyle = new GUIStyle(GUI.skin.button);
            m_PickButtonStyle.font = ButtonFont;
            m_PickButtonStyle.fontSize = 30;
            m_PickButtonStyle.alignment = TextAnchor.MiddleCenter;

            m_PickButtonStyle.normal.background = ButtonBackground;
            m_PickButtonStyle.normal.textColor = Color.grey;

            m_PickButtonStyle.hover.background = ButtonBackground;
            m_PickButtonStyle.hover.textColor = Color.white;

            m_PickButtonStyle.active.background = ButtonBackground;
        }
    }
}