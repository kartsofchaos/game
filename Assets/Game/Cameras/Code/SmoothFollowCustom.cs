using UnityEngine;
using System.Collections;

// Place the script in the Camera-Control group in the component menu
[AddComponentMenu("Camera-Control/Smooth Follow Custom")]

public class SmoothFollowCustom : MonoBehaviour
{
    /*
    This camera smoothes out rotation around the y-axis and height.
    Horizontal Distance to the target is always fixed.
 
    There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.
 
    For every of those smoothed values we calculate the wanted value and the current value.
    Then we smooth it using the Lerp function.
    Then we apply the smoothed values to the transform's position.
    */

	private Transform MyTransform;
    // The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;
	public float lookAtDamping = 3.0f;
	public float lookAtHeightBoost = 1.6f;

	public bool smoothLookAt = true;

	void Start()
	{
		MyTransform = transform;
	}

    void FixedUpdate()
    {
        // Early out if we don't have a target
        if (!target)
            return;

        // Calculate the current rotation angles
        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;
		float currentRotationAngle = MyTransform.eulerAngles.y;
		float currentHeight = MyTransform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
		MyTransform.position = target.position;
		MyTransform.position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
		MyTransform.position = new Vector3(MyTransform.position.x, currentHeight, MyTransform.position.z);

		// Increase the looking height so that the cars roof ends about the middle of the screen. Otherwise
		// all we see is ground. This is an ugly fix for now and should be changed so that it is based on a
		// resolution percentage
		Vector3 targetPosition = target.position;
		targetPosition.y += lookAtHeightBoost;

        // Always look at the target
		if (smoothLookAt) 
		{
			Quaternion rotation = Quaternion.LookRotation (targetPosition - MyTransform.position);
			MyTransform.rotation = Quaternion.Slerp (MyTransform.rotation, rotation, Time.deltaTime * lookAtDamping);
		} 
		else 
		{
			transform.LookAt (targetPosition);
		}
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}