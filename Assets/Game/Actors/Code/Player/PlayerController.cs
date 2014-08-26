using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------------------------------
// Name	:	PlayerController
// Desc  :  Controlls the input of the player for the Car script to read
// ------------------------------------------------------------------------------------------
public class PlayerController : MonoBehaviour 
{
    // Variables that smoothen the throttle and steering
    public float SteerSmoothing     = 5.5f;

    // Controller specicic members
    private float Steering          = 0.0f;
    private float Throttle          = 0.0f;
    private bool HandbrakeOn        = false;

	void Start()
	{
		// If it is not our car, we disable the playercontroller script
		if (!GetComponent<PhotonView> ().isMine)
			enabled = false;
	}

    // ---------------------------------------------------------------------------------------
    // Name	:	GetThrottle
    // Desc	:   Called by the Car component attached to this object to query the current 
    //			Throttle position in the -1 to +1 range.
    // ---------------------------------------------------------------------------------------
    public float GetThrottle()
    {
        return Throttle;
    }

    // ---------------------------------------------------------------------------------------
    // Name	:	GetSteering
    // Desc	:   Called by the Car component attached to this object to query the current 
    //			sterring position in the -1 to +1 range.
    // ---------------------------------------------------------------------------------------
    public float GetSteering()
    {
        return Steering;
    }

    // ---------------------------------------------------------------------------------------
    // Name	:	IsHandbrakeOn
    // Desc	:   Called by the Car component attached to this object to query if the handbrake is
    //          activated
    // ---------------------------------------------------------------------------------------
    public bool IsHandbrakeOn()
    {
        return HandbrakeOn;
    }

	// Update is called once per frame
	void Update () 
    {
        handleInput();  // Read the player input
	}

    // ---------------------------------------------------------------------------------------
    // Name	:	HandleInput
    // Desc	:   Handles the player input from keyboard/joystick etc.
    // ---------------------------------------------------------------------------------------
    private void handleInput()
    {
        float steer, throt;

        throt = Input.GetAxis("Vertical");

        Throttle = throt;//= Mathf.Lerp(Throttle, throt,  0.5f*Time.deltaTime);	
        Throttle = Mathf.Clamp(Throttle, -1.0f, 1.0f);

        steer = Input.GetAxis("Horizontal");

        Steering = Mathf.Lerp(Steering, steer, SteerSmoothing * Time.deltaTime);
        Steering = Mathf.Clamp(Steering, -1.0f, 1.0f);

        // Handbrake
        HandbrakeOn = Input.GetKey(KeyCode.Space);
    }
}
