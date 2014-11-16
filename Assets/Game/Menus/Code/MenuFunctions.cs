using UnityEngine;
using System.Collections;

public class MenuFunctions : MonoBehaviour {

	// Use this for initialization
	void Start () {	    
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void BlueTeamButton()
    {
        ChooseTeam(Team.Blue);
    }

    public void RedTeamButton()
    {
        ChooseTeam(Team.Red);
    }


    private void ChooseTeam(Team team)
    {
        GameObject.Find("GameManager").GetComponent<PlayerSpawner>().CreateLocalPlayer(team);
        //enabled = false;
    }

    public void Connect()
    {
        // If we are not connected, connect to Photon via our MultiplayerConnector
        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            MultiplayerConnector.Instance.Connect();
        }
    }


}
