using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------------------------------
// Name	:	CarVisuals
// Desc  :  This script should be attached to the top level car object.
//          It reads the forces applied to the car from the CarHandling and adds visual effects to the car object
//          such as smoke, wheel spinns etc.
// ------------------------------------------------------------------------------------------
public class CarVisuals : CarBase
{

    float steering = 0.0f;
    bool handbrakeOn = false;

    // ------------------------------------------------------------------------------
    // Name	:	Update
    // Desc	:	Updates the wheel graphics (rotate them, do skid marks, detect whether 
    //          they are on or off ground.
    // ------------------------------------------------------------------------------
    void Update()
    {
		if (PhotonView.isMine == true)
		{
			UpdateWheelGraphics();
		}
		else
		{
			UpdateNetworkWheelGraphics();
		}

    }

    void FixedUpdate()
    {
        steering = PlayerController.GetSteering();
        handbrakeOn = PlayerController.IsHandbrakeOn();
    }

    // ------------------------------------------------------------------------------
    // Name :	UpdateWheelGraphics
    // Desc	:	Rotates and turns the wheel graphics. Should be extended to check wether
    //          Or not a wheel is in the air, locked by breaks, suspension length etc.
    // ------------------------------------------------------------------------------
    void UpdateWheelGraphics()
    {
        if (CarHandling.wheels == null)
            return;

        foreach (CarWheel w in CarHandling.wheels)
        {
            // Y Rotate the front wheels based on steering angle
            if (w.frontWheel)
            {
                // Rotate front wheels around their y axis. For some reason our wheels are rotated 270 degrees from the start
                Vector3 ea = w.wheelGraphic.parent.localEulerAngles;
                ea.y = steering * CarHandling.maximumTurn;
                w.wheelGraphic.parent.localEulerAngles = ea;
            }

            // Rotate the wheel graphics
            if (w.frontWheel || (w.rearWheel && !handbrakeOn))
                w.wheelGraphic.Rotate(Vector3.right * w.collider.rpm * 2 * Mathf.PI / 60.0f * Time.deltaTime * Mathf.Rad2Deg);

            // Move the wheel along its local Y-axis according to the suspension
            RaycastHit hit;
            Vector3 origin = w.collider.transform.position;
            Vector3 direction = -w.collider.transform.up;
            float distance = w.collider.radius + w.collider.suspensionDistance;
            if (Physics.Raycast(origin, direction, out hit, distance))
            {
                w.wheelGraphic.position = hit.point + w.collider.transform.up * w.collider.radius;
            }
            else
                w.wheelGraphic.position = w.collider.transform.position - (w.collider.transform.up * w.collider.suspensionDistance);
        }
    }

	// ------------------------------------------------------------------------------
	// Name :	UpdateNetworkWheelGraphics
	// Desc	:	Rotates and turns the wheel graphics for network players
	// ------------------------------------------------------------------------------
	void UpdateNetworkWheelGraphics()
	{

	}

    internal static void SerializeState(PhotonStream stream, PhotonMessageInfo info)
    {
		/*
		if (stream.isWriting == true)
		{
			//stream.SendNext(steering);
			stream.SendNext(CarHandling.wheels[0].wheelGraphic.position);
			stream.SendNext(CarHandling.wheels[0].wheelGraphic.rotation);

			stream.SendNext(CarHandling.wheels[1].wheelGraphic.position);
			stream.SendNext(CarHandling.wheels[1].wheelGraphic.rotation);

			stream.SendNext(CarHandling.wheels[2].wheelGraphic.position);
			stream.SendNext(CarHandling.wheels[2].wheelGraphic.rotation);

			stream.SendNext(CarHandling.wheels[3].wheelGraphic.position);
			stream.SendNext(CarHandling.wheels[3].wheelGraphic.rotation);
		}
		else
		{
			CarHandling.wheels[0].wheelGraphic.position = (Vector3) stream.ReceiveNext();
			CarHandling.wheels[0].wheelGraphic.rotation = (Quaternion) stream.ReceiveNext();

			CarHandling.wheels[1].wheelGraphic.position = (Vector3) stream.ReceiveNext();
			CarHandling.wheels[1].wheelGraphic.rotation = (Quaternion) stream.ReceiveNext();

			CarHandling.wheels[2].wheelGraphic.position = (Vector3) stream.ReceiveNext();
			CarHandling.wheels[2].wheelGraphic.rotation = (Quaternion) stream.ReceiveNext();

			CarHandling.wheels[3].wheelGraphic.position = (Vector3) stream.ReceiveNext();
			CarHandling.wheels[3].wheelGraphic.rotation = (Quaternion) stream.ReceiveNext();
		}
			*/
    }
}
