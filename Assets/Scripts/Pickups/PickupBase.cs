using UnityEngine;
using System.Collections;

/// <summary>
/// All pickups (flag, health) inherit from this class
/// It defines the basic pickup functionality between the pickup and the ship and passes it on
/// </summary>
public abstract class PickupBase : MonoBehaviour
{
    /// <summary>
    /// Determines whether this instance [can be picked up by the specified ship.
    /// THis will be defined by the specific pickup script
    /// </summary>
    /// <param name="ship">The ship which is trying to pick up this object</param>
    /// <returns>Can the defined script pick up the object?</returns>
    public abstract bool CanBePickedUpBy(Player p);

    /// <summary>
    /// Called when a ship successfully picked up the object
    /// </summary>
    /// <param name="ship">The ship which picked up the object</param>
    public abstract void OnPickup(Player p);

    PhotonView m_PhotonView;
    protected PhotonView PhotonView
    {
        get
        {
            if (m_PhotonView == null)
            {
                m_PhotonView = PhotonView.Get(this);
            }

            return m_PhotonView;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            Player p = collider.transform.root.gameObject.GetComponent<Player>();
            
            //We check if the ship can pickup this object two times, once here
            //and once after the event propagated through the network, to ensure that
            //possible lag doesn't interfere with the verification
            if (CanBePickedUpBy(p) == true)
            {
                PickupObject(p);
            }
        }
    }

    void PickupObject(Player p)
    {
        //As with all RPC methods, we branch between online and offline mode here

        if (PhotonNetwork.offlineMode == true)
        {
            OnPickup(p);
        }
        else
        {
            //We use PhotonTargets.AllBufferedViaServer here to avoid two ships picking up the
            //same object before one of the pickup events has reached the server
            //Check out Part 1 Lesson 4 http://youtu.be/Wn9P4d1KwoQ for more detailed explanations            
            PhotonView.RPC(
                    "OnPickup"
                , PhotonTargets.AllBufferedViaServer
                , new object[] { p.PhotonView.viewID }
            );
        }
    }

    [RPC]
    protected void OnPickup(int viewId)
    {
        PhotonView view = PhotonView.Find(viewId);

        if (view != null)
        {
            Player p = view.GetComponent<Player>();

            //This is the second time we check if the pickup can be collected by the ship
            //In online mode this happens after the event has been received from a remote ship
            if (CanBePickedUpBy(p) == true)
            {
                OnPickup(p);
            }
        }
    }
}