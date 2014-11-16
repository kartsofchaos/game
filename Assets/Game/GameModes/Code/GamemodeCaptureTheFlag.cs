using UnityEngine;
using System.Collections;

public class GamemodeCaptureTheFlag : MonoBehaviour
{
    private GameObject master, flag, red, blue;
    public int totalFlags = 5;
    public float TotalRoundTime;

    // Create primitive type for base ground and then add flag prefabs

    void Start()
    {
        OnSetup();
    }

    public void OnSetup()
    {
        if (PhotonNetwork.isMasterClient == true)
        {
            master = new GameObject();
            master.transform.name = "FlagPlatform";

            blue = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            blue.transform.name = "BlueGround";
            blue.transform.position = new Vector3(30, 0.001f, 0f);
            blue.transform.localScale = new Vector3(1, 0.001f, 1);
            blue.collider.isTrigger = true;
            blue.transform.parent = master.transform;

            red = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            red.transform.name = "RedGround";
            red.transform.position = new Vector3(-30, 0.001f, 0f);
            red.transform.localScale = new Vector3(1, 0.001f, 1);
            red.collider.isTrigger = true;
            red.transform.parent = master.transform;

            flag = PhotonNetwork.InstantiateSceneObject("Prefabs/AntlerFlag", new Vector3(30, 0f, 0f), Quaternion.identity, 0, null);
            flag.transform.tag = "Blue";
            flag.transform.parent = blue.transform;

            flag = PhotonNetwork.InstantiateSceneObject("Prefabs/EagleFlag", new Vector3(-30, 0f, 0f), Quaternion.identity, 0, null);
            flag.transform.tag = "Red";
            flag.transform.parent = red.transform;
        }
    }

    void Update()
    {
        TotalRoundTime -= Time.deltaTime;
    }

    public void IncreaseTeamScore(Team team)
    {
        if (team == Team.Red)
        {
            // increase red score
        } else if(team == Team.Blue) 
        {
            // increase blue score
        }
    }

    public bool IsRoundFinished()
    {
        return TotalRoundTime <= 0;
    }
}
