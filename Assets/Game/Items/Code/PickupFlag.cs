using UnityEngine;
using System.Collections;

public class PickupFlag : Photon.MonoBehaviour {

    private Transform personWithFlag;
    public bool isFlagStolen = false;
    private string currentFlagTag;
    private Vector3 homePosition;
    private Vector3 blueTeam;
    private Vector3 redTeam;


    void Start()
    {
        if ( PhotonNetwork.isMasterClient )
        {
            homePosition = transform.position;
            blueTeam = GameObject.Find("BlueFlag").transform.position;
            redTeam = GameObject.Find("RedFlag").transform.position;

        }
    }

    // Get opposite team
    Vector3 getOtherTeam()
    {
        if (currentFlagTag == "Blue")
        {
            return redTeam;
        }
        return blueTeam;
    }

    bool isNotEqualTeam(Team team)
    {
        Team t = Team.None;

        if ( transform.tag == "Blue" ) t = Team.Blue;
        if( transform.tag == "Red" ) t = Team.Red;

        return t != team;
    }

    
	// Update is called once per frame
	void Update () {
        HandleFlag();

        if (isFlagStolen)
        {
            transform.position = personWithFlag.position;
        }
	}

    void HandleFlag()
    {
        //Debug.Log(Vector3.Distance(transform.position, getOtherTeam()));

        if ( isFlagStolen && Vector3.Distance(transform.position, getOtherTeam()) < 2f)
        {
            transform.position = homePosition;
            isFlagStolen = false;
        }        
    }

   void OnTriggerEnter(Collider c)
    {
        Player p = c.transform.root.GetComponent<Player>();
        //Debug.Log("Player in team: " + p.Team);
        //Debug.Log("Tranform tag: " + transform.tag);

        if ( isNotEqualTeam(p.Team) )
        {            
            currentFlagTag = transform.tag;
            personWithFlag = c.transform.root.GetChild(0);
            isFlagStolen = true;

            //Debug.Log(currentFlagTag);
        }
    }

    void OnSerializedView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            stream.SendNext(transform.position);
        }
    }
}
