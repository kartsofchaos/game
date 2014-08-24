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

    public Character Character;

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

    void Start()
    {
  //      PhotonNetwork.sendRate = 60;
 //       PhotonNetwork.sendRateOnSerialize = 60;

        startPosition = transform.position;
        startRotation = transform.rotation;
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

    public void OnHealthChanged()
    {
        if (_health <= 0)
        {
            _health = 0;
     //       Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
     //       SetVisibility(false);

            //If our local ship dies, call the respawn function after 2 seconds
            if (PhotonView.isMine == true)
            {
                Invoke("SendRespawn", 2f);
            }
        }
    }
     
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //This method gets called right after a GameObject is created through PhotonNetwork.Instantiate
        //The fifth parameter in PhotonNetwork.instantiate sets the instantiationData and every client
        //can access them through the PhotonView. In our case we use this to send which team the ship
        //belongs to. This methodology is very useful to send data that only has to be sent once.

        if (!PhotonView.isMine)
        {
            SetTeam((Team)PhotonView.instantiationData[0]);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Multiple components need to synchronize values over the network.
        //The SerializeState methods are made up, but they're useful to keep
        //all the data separated into their respective components

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
}
