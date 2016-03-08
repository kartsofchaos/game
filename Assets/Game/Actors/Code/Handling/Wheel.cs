using UnityEngine;
using System.Collections;

// Class that represents a wheel and its collider
public class Wheel
{
    public Transform wheelGraphic;
    public WheelCollider collider;
    public bool frontWheel = false;
    public bool rearWheel = false;
    public Vector3 lastEmitPosition = Vector3.zero;
    public float lastEmitTime = Time.time;
    public Vector3 wheelVelo = Vector3.zero;
    public Vector3 groundSpeed = Vector3.zero;
    public bool isSlipping = false;                            // Used to check wether or not the wheel is skipping, and should generate smoke and skidmarks      
}
