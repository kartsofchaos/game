using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public Rigidbody rigidbody; 
	public Transform carTransform;
	public PhotonView photonView;
	public GameObject carGameObject; 

	// Network variables used to predict positions of cars over the network
	Vector3 m_NetworkPosition;
	Quaternion m_NetworkRotation;
	Vector3 m_NetworkVelocity;
	float m_NetworkAngularVelocity;
	double m_LastNetworkDataReceivedTime;
	Vector3 relativeVelocity;

	void Awake() 
	{
		Debug.Log ("Awake and init PhotonView");
		photonView = carTransform.parent.GetComponent<PhotonView> ();
	}

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
		// If this is "our" car we apply our forces to it as per normal
		if (photonView.isMine == true)
		{
			UpdateLocalPhysics();
		}

		// Otherwise we try to predict where the car is depending on the information we recieved from the latest photon package
		else
		{
			UpdateNetworkedPosition();
			UpdateNetworkedRotation();
		}
    }

	void UpdateLocalPhysics() 
	{
		float motor = maxMotorTorque * Input.GetAxis("Vertical");
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

		foreach (AxleInfo axleInfo in axleInfos)
		{
			if (axleInfo.steering)
			{
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if (axleInfo.motor)
			{
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			}

			// AntiRoll for car
			handleAntiRoll(axleInfo);

			// Handbrak för the car
			ApplyBrakeTorque(axleInfo);

			ApplyLocalPositionToVisuals(axleInfo.leftWheel);
			ApplyLocalPositionToVisuals(axleInfo.rightWheel);
		}
	}

	// --------------------------------------------------------------------------------------------
	// Name	:	ApplyBrakeTorque
	// Desc	:	Applies any braking torque if the controller is braking
	//          and applies engine braking when decelerating
	// ---------------------------------------------------------------------------------------------
	private void ApplyBrakeTorque(AxleInfo axleInfo)
	{
		bool handBreakOn = Input.GetKey(KeyCode.LeftShift);

		// Handbrake
		if (handBreakOn)
		{
			axleInfo.leftWheel.brakeTorque = 1000f;
			axleInfo.rightWheel.brakeTorque = 1000f;
		}
		else
		{
			axleInfo.leftWheel.brakeTorque = 0.0f;
			axleInfo.rightWheel.brakeTorque = 0.0f;
		}


	}

	private void handleAntiRoll(AxleInfo axleInfo) {
		WheelCollider WheelL = axleInfo.leftWheel;
		WheelCollider WheelR = axleInfo.rightWheel;
		WheelHit hit;

		float travelL = 1.0f;
		float travelR = 1.0f;
		float AntiRoll = 5000.0f;

		bool groundedL = WheelL.GetGroundHit(out hit);

		if (groundedL)
		{
			travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
		}

		bool groundedR = WheelR.GetGroundHit(out hit);

		if (groundedR)
		{
			travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
		}

		float antiRollForce = (travelL - travelR) * AntiRoll;

		if (groundedL)
		{
			rigidbody.AddForceAtPosition(WheelL.transform.up * -antiRollForce, WheelL.transform.position);
		}                

		if (groundedR)
		{
			rigidbody.AddForceAtPosition(WheelR.transform.up * antiRollForce, WheelR.transform.position);
		}
	}
		
	void Update() {
		// Flip back car if upside down
		if (carTransform.localEulerAngles.z > 180.0f) {
			carTransform.localEulerAngles = new Vector3 (
				carTransform.localEulerAngles.x,
				carTransform.localEulerAngles.y,
				0.0f
			);
		}
	}


	// We only need to synchronize a few variables to recreate a good approximation of the cars position on each client
	public void SerializeState(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting == true)
		{
			stream.SendNext(carTransform.position);
			stream.SendNext(carTransform.rotation);
			stream.SendNext(rigidbody.velocity);
			stream.SendNext(rigidbody.angularVelocity.magnitude);
		}
		else
		{
			m_NetworkPosition = (Vector3)stream.ReceiveNext();
			m_NetworkRotation = (Quaternion)stream.ReceiveNext();
			m_NetworkVelocity = (Vector3)stream.ReceiveNext();
			m_NetworkAngularVelocity = (float)stream.ReceiveNext();

			m_LastNetworkDataReceivedTime = info.timestamp;
		}
	}

	// ------------------------------------------------------------------------------
	// Name	:	UpdateNetworkedPosition
	// Desc	:	Try to predict where the car is depending on the data recieved from Photon
	// ------------------------------------------------------------------------------
	void UpdateNetworkedPosition()
	{
		// Calculate the time it has passed since the information was up-to-date
		float pingInSeconds = (float)PhotonNetwork.GetPing () * 0.001f;
		float timeSinceLastUpdate = (float)(PhotonNetwork.time - m_LastNetworkDataReceivedTime);
		float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

		// Estimate where the position has moved during this time, given the supplied position and velocity
		Vector3 exterpolatedTargetPosition;
		exterpolatedTargetPosition.x = m_NetworkPosition.x + m_NetworkVelocity.x * totalTimePassed;
		exterpolatedTargetPosition.y = m_NetworkPosition.y + m_NetworkVelocity.y * totalTimePassed;
		exterpolatedTargetPosition.z = m_NetworkPosition.z + m_NetworkVelocity.z * totalTimePassed;

		// Move smoothly between the current local position and the exterpolated position to reduce the feel of "lag"
		// So the cars do not suddenly jump to the exterpolated position. The maxDelta is calculated by taking the distance
		// between the local and exterpolated position, and dividing it with totalTimePassed. This way we get the speed
		// at which the position should moveTowards to reach the exterpolated position just in time when we receive the next
		// network update. There is no point in reaching it faster, and thus making the transition appear jerky, as we still 
		// have to wait for the new position update from Photon before we can move further.
		Vector3 newPosition;
		newPosition.x = Mathf.MoveTowards (carTransform.position.x, exterpolatedTargetPosition.x, 
			Mathf.Abs(carTransform.position.x - exterpolatedTargetPosition.x) / totalTimePassed * Time.deltaTime); 
		newPosition.y = Mathf.MoveTowards (carTransform.position.y, exterpolatedTargetPosition.y,
			Mathf.Abs(carTransform.position.y - exterpolatedTargetPosition.y) / totalTimePassed * Time.deltaTime);
		newPosition.z = Mathf.MoveTowards (carTransform.position.z, exterpolatedTargetPosition.z,
			Mathf.Abs (carTransform.position.z - exterpolatedTargetPosition.z) / totalTimePassed * Time.deltaTime);

		// If the distance however is very long (only possible if someone had a lagspike or froze for unknown reasons), we simply
		// set the new position to the exterpolated position so that the cars do not fly accross the map when someone lags.
		// 7f was calculated as the maximum a network car could move with our current topspeed when both players have 120ping each.
		if (Vector3.Distance(carTransform.position, exterpolatedTargetPosition) > 7f)
		{
			newPosition = exterpolatedTargetPosition;
		}

		// Update the local position
		carTransform.position = newPosition;
	}

	// ------------------------------------------------------------------------------
	// Name	:	UpdateNetworkedRotation
	// Desc	:	Try to predict where the car is depending on the data recieved from Photon
	// ------------------------------------------------------------------------------
	void UpdateNetworkedRotation()
	{
		// Calculate the time it has passed since the information was up-to-date
		float pingInSeconds = (float)PhotonNetwork.GetPing () * 0.001f;
		float timeSinceLastUpdate = (float)(PhotonNetwork.time - m_LastNetworkDataReceivedTime);
		float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

		float angDiff = Vector3.Distance (carTransform.rotation.eulerAngles, m_NetworkRotation.eulerAngles);

		if (angDiff < 2) 
		{
			carTransform.rotation = Quaternion.RotateTowards (transform.rotation, m_NetworkRotation, angDiff * totalTimePassed * Time.deltaTime);
		} 
		else 
		{
			carTransform.rotation = m_NetworkRotation;
		}
	}

	void OnGUI()
	{
		GUI.Box(new Rect(0, 0, 220, 200), "");

		if (photonView.isMine == true)
		{
			GUILayout.BeginArea (new Rect (10, 10, Screen.width, Screen.height));
			{
				GUILayout.Label ("Local car");
				GUILayout.Label ("Position: " + carTransform.position);
				GUILayout.Label ("Rotation: " + carTransform.rotation);
				GUILayout.Label ("Velocity: " + rigidbody.velocity);
				GUILayout.Label ("Angular Velocity: " + rigidbody.angularVelocity.magnitude);
				GUILayout.Label ("Ping: " + (float)PhotonNetwork.GetPing ());
			}
			GUILayout.EndArea ();
		} 
		else 
		{
			GUILayout.BeginArea (new Rect (10, 10, Screen.width, Screen.height));
			{
				GUILayout.Label ("Network Car");
				GUILayout.Label ("LNDTime: " + m_LastNetworkDataReceivedTime);
				GUILayout.Label ("------------------------");
				GUILayout.Label ("Received information");
				GUILayout.Label ("Position: " + m_NetworkPosition);
				GUILayout.Label ("Rotation: " + m_NetworkRotation);
				GUILayout.Label ("Velocity: " + m_NetworkVelocity);
				GUILayout.Label ("Angular Velocity: " + m_NetworkAngularVelocity);
				GUILayout.Label ("------------------------");
				GUILayout.Label ("Local information");
				GUILayout.Label ("Position: " + carTransform.position);
				GUILayout.Label ("Rotation: " + carTransform.rotation);
				GUILayout.Label ("Velocity: " + rigidbody.velocity);
				GUILayout.Label ("Angular Velocity: " + rigidbody.angularVelocity.magnitude);
			}
			GUILayout.EndArea ();
		}
	}
}
