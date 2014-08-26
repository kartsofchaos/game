using UnityEngine;
using System.Collections;

public class SpawnFlag : MonoBehaviour {
    private GameObject master, flag, red, blue;

    // Use this for initialization
    void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            master = new GameObject();
            master.transform.name = "FlagPlatform";

            blue = GameObject.CreatePrimitive(PrimitiveType.Plane);
            blue.transform.name = "BlueFlag";
            blue.transform.position = new Vector3(30, 0.01f, 0f);
            blue.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            blue.transform.parent = master.transform;

            red = GameObject.CreatePrimitive(PrimitiveType.Plane);
            red.transform.name = "RedFlag";
            red.transform.position = new Vector3(-30, 0.01f, 0f);
            red.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            red.transform.parent = master.transform;

            flag = PhotonNetwork.InstantiateSceneObject("Prefabs/Flag", new Vector3(30,0.5f,0f), Quaternion.identity, 0, null);
            flag.transform.tag = "Blue";
            flag.transform.parent = blue.transform;

            flag = PhotonNetwork.InstantiateSceneObject("Prefabs/Flag", new Vector3(-30, 0.5f, 0f), Quaternion.identity, 0, null);
            flag.transform.tag = "Red";
            flag.transform.parent = red.transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
