using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------------------------------
// Name	:	PlayerProperties
// Desc :   Holds player information that was set in the menu/lobby.
//          This object is not destroyed when the map is loaded and is read from by the
//          local player script when instantiated.
// ------------------------------------------------------------------------------------------
public class PlayerProperties : MonoBehaviour
{
    public string Name = "Player";

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
