using UnityEngine;
using System.Collections;

public class SpawnItem : MonoBehaviour {
    public float resetTimer;
    float time;
    bool destroyed = false;

    GameObject[] items = new GameObject[8]; 

    public Vector3 item1;
    public Vector3 item2;
    public Vector3 item3;
    public Vector3 item4;
    public Vector3 item5;
    public Vector3 item6;
    public Vector3 item7;
    public Vector3 item8;

    void CreateItem(Vector3 item, int index)
    {
         GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
         o.transform.position = item;
         o.collider.isTrigger = true;
         o.tag = "Item";
         o.AddComponent<PickupItem>();
         items[index] = o;
    }

	// Use this for initialization
	void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            CreateItem(item1, 0);
            CreateItem(item2, 1);
            CreateItem(item3, 2);
            CreateItem(item4, 3);
            CreateItem(item5, 4);
            CreateItem(item6, 5);
            CreateItem(item7, 6);
            CreateItem(item8, 7);

            time = resetTimer;
        }

	}
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;
        Debug.Log(time);
        if (time <= 1 && !destroyed)
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("Item");
            foreach (GameObject g in list)
            {
                Destroy(g.gameObject);
            }
            destroyed = true;
        }
        else if (time < -4 && destroyed)
        {
            items = new GameObject[8];
            time = resetTimer;
            CreateItem(item1, 0);
            CreateItem(item2, 1);
            CreateItem(item3, 2);
            CreateItem(item4, 3);
            CreateItem(item5, 4);
            CreateItem(item6, 5);
            CreateItem(item7, 6);
            CreateItem(item8, 7);

            destroyed = false;
        }
	}
}
