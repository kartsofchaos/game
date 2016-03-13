using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates the player objects and spawns them on the spawnpoints assigned from
/// the unity inspector
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    /// <summary>
    /// Positions where the red players may spawn
    /// </summary>
    public Transform[] RedSpawns;

    /// <summary>
    /// Positions where the blue players may spawn
    /// </summary>
    public Transform[] BlueSpawns;

	public bool isShowing; 
	public GameObject menuCanvas; 

	// Use this for initialization
	void Start ()
    {
	    // If we are not connected then we probably pressed play in Medieval_Town in editor mode.
        // In this case go back to the main menu.
        if (PhotonNetwork.connected == false)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
	}

	void Update() {
		bool escape = Input.GetKeyDown ("escape");
		if (escape) {
			isShowing = !isShowing; 
			menuCanvas.SetActive (isShowing);
		}
	}

	public void ChooseRedTeam() {
		CreateLocalPlayer (Team.Red);
	}

	public void ChooseBlueTeam() {
		CreateLocalPlayer (Team.Blue);
	}

    public void CreateLocalPlayer(Team team)
    {
        // Notice the difference from PhotonNetwork.Instantiate to Unitys GameObject.Instantiate
        GameObject newPlayerObject = PhotonNetwork.Instantiate("ClumsyKnight", new Vector3(25f, 1f, -15f), 
			Quaternion.identity, 0, new object[] { (int)team });

        // Spawn at the right place
        if (team == Team.Red)
        {
            newPlayerObject.transform.position = RedSpawns[0].position;
            newPlayerObject.transform.rotation = RedSpawns[0].rotation;
        }
        else
        {
            newPlayerObject.transform.position = BlueSpawns[0].position;
            newPlayerObject.transform.rotation = BlueSpawns[0].rotation;
        }

        // Find the MenuCamera and deactivate it
		Camera.main.GetComponent<SmoothFollowCustom>().SetTarget( newPlayerObject.GetComponentInChildren<CarHandling>().transform );
    }

}
