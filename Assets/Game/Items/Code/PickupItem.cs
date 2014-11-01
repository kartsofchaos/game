using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider c)
    {
        Debug.Log(c.transform.tag);
        Debug.Log(this.tag);
        Destroy(gameObject);
    }
}
