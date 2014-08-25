using UnityEngine;
using System.Collections;

// Anti roll bars taken from http://forum.unity3d.com/threads/50643-How-to-make-a-physically-real-stable-car-with-WheelColliders
// Anti-roll bars work by transfering some compression force from one spring to the opposite in the same axle. 
// The amount of the transfered force depends on the difference in the suspension travel among the wheels.
public class CarAntiRollBar
{
    public CarWheel wheelL;
    public CarWheel wheelR;
    public WheelHit hitL;
    public WheelHit hitR;

    public Rigidbody carRigidBody;
    public float antiRollVal = 6000f;

    // Should be called each fixed update
    public void ApplyAntiRollForce()
    {
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = wheelL.collider.GetGroundHit(out hitL);
        bool groundedR = wheelR.collider.GetGroundHit(out hitR);

        // First task is to calculate the suspension travel on each wheel, as well as determine whether is grounded or not. 
        // We want the travel value to be between 0.0 (fully compressed) and 1.0 (fully extended):
        if (groundedL)
            travelL = (-wheelL.collider.transform.InverseTransformPoint(hitL.point).y - wheelL.collider.radius) / wheelL.collider.suspensionDistance;
        if (groundedR)
            travelR = (-wheelR.collider.transform.InverseTransformPoint(hitR.point).y - wheelR.collider.radius) / wheelR.collider.suspensionDistance;

        // We multiply the travel diference by the AntiRoll value, which is the maximum force that the anti-roll bar can 
        // transfer among the springs (we could call this the Anti-roll bar's stiffness). This yields the amount of force that will be transfered:
        float antiRollForce = (travelL - travelR) * antiRollVal;

        // Finally, we must simply substract the force from one spring and add it to the other. 
        // We achieve this by adding opposite forces to the rigidbody at the positions of the WheelColliders:
        if (groundedL)
            carRigidBody.AddForceAtPosition(wheelL.collider.transform.up * -antiRollForce, wheelL.collider.transform.position);
        if (groundedR)
            carRigidBody.AddForceAtPosition(wheelR.collider.transform.up * antiRollForce, wheelR.collider.transform.position);
    }
}