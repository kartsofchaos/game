using UnityEngine;
using System.Collections;

public class BulletLifeTime : MonoBehaviour {

    private float startTime;
    public float SecondsUntilDestroy;
    public float speed;
    public Transform car;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        rigidbody.velocity = transform.up * speed;
	}

    void OnTriggerEnter(Collider otherObject)
    {
        Destroy(this.gameObject);
        // From who is the bullet shot 
        // what object is hit 
        // if other player is hit then decrease player health or shield
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time-startTime >= SecondsUntilDestroy)
        {
            Destroy(this.gameObject);
        }        
	}
}
