using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour {

	public Rigidbody carRigidBody; 
	public Transform carTransform;
	public float timeBetweenJumps = 1.0f;
	public float timestamp; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		bool jump = Input.GetButtonDown ("Jump");

		if (jump && Time.time >= timestamp) {
			carRigidBody.AddForce(carTransform.up * 450000);
			timestamp = Time.time + timeBetweenJumps;
		}
	}
}
