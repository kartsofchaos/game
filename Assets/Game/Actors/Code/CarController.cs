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
    public float AntiRoll = 5000.0f;
    public Rigidbody rigidbody; 
	public Transform carTransform; 

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

            float travelL = 1.0f;
            float travelR = 1.0f;


            // AntiRoll for car
            WheelCollider WheelL = axleInfo.leftWheel;
            WheelCollider WheelR = axleInfo.rightWheel;

            WheelHit hit; 

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

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

	void Update() {
		if (carTransform.localEulerAngles.z > 180) {
			carTransform.localEulerAngles = new Vector3 (
				carTransform.localEulerAngles.x,
				carTransform.localEulerAngles.y,
				0.0f
			);
		}
	}
}
