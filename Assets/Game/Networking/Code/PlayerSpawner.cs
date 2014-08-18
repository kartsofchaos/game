using UnityEngine;
using System.Collections;


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

	// Use this for initialization
	void Start ()
    {
	    // If we are not connected then we probably pressed play in Medieval_Town in editor mode.
        // In this case go back to the main menu.
        if (PhotonNetwork.connected == false)
        {
            Application.LoadLevel("MainMenu");
            return;
        }
	}

    public void CreateLocalPlayer(Team team)
    {
        // Notice the difference from PhotonNetwork.Instantiate to Unitys GameObject.Instantiate
        GameObject newPlayerObject = PhotonNetwork.Instantiate("Actors/SportsCar", new Vector3(25f, 1f, -15f), Quaternion.identity, 0, new object[] { (int)team });

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

 //       Player newPlayer = newPlayerObject.GetComponentInChildren<Player>();
//        newPlayer.SetTeam( team );

        // Find the MenuCamera and deactivate it
		Camera.main.GetComponent<SmoothFollowCustom>().SetTarget( newPlayerObject.GetComponentInChildren<CarHandling>().transform );

		// Instantiate a HUD for the player and place it in the local players hierharky
		GameObject hud = Instantiate(Resources.Load("HUD/SportscarHUD")) as GameObject;
		hud.transform.parent = newPlayerObject.transform;
    }

}
