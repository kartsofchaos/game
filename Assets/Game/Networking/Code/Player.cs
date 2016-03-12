using UnityEngine;
using System.Collections;

public class Player : PlayerBase 
{
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
    }

    private float _health = 100.0f;
    public float Health
    {
        get
        {
            return _health;
        }
    }

    private float _armor = 0.0f;
    public float Armor
    {
        get
        {
            return _armor;
        }
    }

    private Team _team;
    public Team Team
    {
        get
        {
            return _team;
        }
    }

    public bool IsLocalPlayer
    {
        get
        {
          return PhotonView.isMine;
        }
    }

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        // Only read the properties for our own car
        if (IsLocalPlayer)
            readProperties();
    }

    // Read our name from the playerproperties and destroy it
    private void readProperties()
    {
        GameObject go = GameObject.Find("PlayerProperties");
        _name = go.GetComponent<PlayerProperties>().Name;
        Destroy(go);
    }

    public void SetTeam(Team team)
    {
        //This method gets called right after a car is created
        _team = team;
    }

    void OnHealthChanged()
    {
        if (_health <= 0)
        {
            _health = 0;

            //If our local ship dies, call the respawn function after 2 seconds
            if (PhotonView.isMine == true)
            {
                Invoke("SendRespawn", 2f);
            }
        }
    }
     
	/// <summary>
	/// This method gets called right after a GameObject is created through PhotonNetwork.Instantiate
	/// The fifth parameter in PhotonNetwork.instantiate sets the instantiationData and every client
	/// can access them through the PhotonView. In our case we use this to send which team the ship
	/// belongs to. This methodology is very useful to send data that only has to be sent once.
	/// </summary>
	/// <param name="info">Info.</param>
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!PhotonView.isMine)
        {
            SetTeam((Team)PhotonView.instantiationData[0]);
        }
    }

	/// <summary>
	/// Multiple components need to synchronize values over the network.
	/// The SerializeState methods are made up, but they're useful to keep
	/// all the data separated into their respective components
	/// </summary>
	/// <param name="stream">Stream.</param>
	/// <param name="info">Info.</param>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        SerializeState(stream, info);

        CarVisuals.SerializeState(stream, info);
        CarHandling.SerializeState(stream, info);
    }

    void SerializeState(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            stream.SendNext(_name);
            stream.SendNext(_health);
            stream.SendNext(_armor);
        }
        else
        {
            float oldHealth = _health;
            _name = (string)stream.ReceiveNext();
            _health = (float)stream.ReceiveNext();
            _armor = (float)stream.ReceiveNext();

            if (_health != oldHealth)
            {
                OnHealthChanged();
            }
        }
    }

	void OnGUI() 
	{
		float position = Screen.width - 100;
		GUI.Box(new Rect(position, 0, 100, 100), "");

		GUILayout.BeginArea (new Rect (position, 10, Screen.width, Screen.height));
		{
			GUILayout.Label ("Name: " + Name);
			GUILayout.Label ("Health: " + Health);
			GUILayout.Label ("Armor: " + Armor);
		}
		GUILayout.EndArea ();
	}
}
