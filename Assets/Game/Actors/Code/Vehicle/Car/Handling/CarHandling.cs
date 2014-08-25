using UnityEngine;
using System.Collections;

// Enums used by the controller
public enum Controller_GearStatus { Neutral, Engaged }
public enum ControllerDevice { Keyboard }

// ------------------------------------------------------------------------------------------
// Name	:	CarHandling
// Desc  :  This script should be attached to the top level car object.
//          It translates input from the InputController and interprets them to forces that
//          are applied to the car
// ------------------------------------------------------------------------------------------
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class CarHandling : CarBase    
{
    // Wheel Settings
    public float SlipFactor                 = 0.7f;                             // The amount of grip the wheels offer on a perfect surface
    public int maximumTurn                  = 50;                               // Maximum turning angle in degrees 
    public int minimumTurn                  = 12;                               // The minimum turning angle in degrees
    public bool rearWheelDrive              = false;                            // Rear wheel drive
    public bool frontWheelDrive             = false;                            // Front wheel drive
    public Transform[] frontWheels;
    public Transform[] rearWheels;
    internal CarWheel[] wheels;
    private float wheelCount;

    // Wheel friction Curves
    private WheelFrictionCurve forwardsWFC;                                     // Friction Curve for when driving straight
    private WheelFrictionCurve sidewaysWFC;                                     // Friction Curve for when sliding sideways
    private WheelFrictionCurve brakeWFC;                                        // Friction Curve for when braking
    private WheelFrictionCurve handbrakeRWFC;                                   // Friction Curve for the rear wheels when handbraking
    private WheelFrictionCurve handbrakeFWFC;                                   // Friction Curve for the front wheels when handbraking
    private float handbrakeRWStiffness      = 0.1f;
    private float handbrakeFWStiffness      = 0.3f;

    // Suspension Settings
    public float suspensionRange            = 0.01f;                            // How far the suspension can extend
    public float suspensionDamper           = 50.0f;                            
    public float suspensionSpringFront      = 18500.0f;
    public float suspensionSpringRear       = 9250.0f;

    // Car Performance Characteristics
    public float topSpeed                   = 110f;                             // Top speed in KM/H
    public float topReverseSpeed            = 40f;                              // The top speed the car can achieve in reverse
    public AnimationCurve EngineTorqueCurve;                                    // The curve that translates RPM into torque (pulling power)
    public Transform centerOfMass;	                                            // Center of mass for our rigid body, should be set depending on where the engine is placed
    public float brakeTorque                = 1000f;                            // The power of the brakes (1000 N works good as a base)
    public float handbrakeTorque            = 300f;                             // The power of the handbrakes (300 N works good as a base)
    public float coefEngineBraking          = 50f;                              // How fast the car decelerates when the throttle is not used (Engine braking)
    public float diffRatio                  = 3.42f;
    public float[] gearRatios               = new float[] { 2f, 1.7f, 1.3f, 1.0f, 0.7f };   // The torque multiplier from each gear. Lower gears are stronger and should generate more torque
    public float[] changeGearAtRPM          = new float[] { 2000f, 2500f, 2500f, 2500f};    // The RPM limit of which the car will shift gears
    public float timeToShiftGear            = 0.2f;                             // The time it takes to shift gear (causes delay in throttle)

    // Anti roll-bars
    public bool antiRollBars                = true;                             // Boolean that determines wether or not the car has anti-roll bars
    public float antiRollBarStiffness       = 6000f;                            // The maximum force that the antiroll bars can transfer to the opposite wheel
    private CarAntiRollBar[] antiRollBarList;
    
    // Aero Dynamics
    public float frontalArea                = 2.0f;                             // The frontal area of the car that is taken into account for air resistance
    public float sideAreaMultiplier         = 3.0f;                             // When sliding sideways, the Side area is calculated by the frontal area * a multiplier
    public float coefDrag                   = 0.25f;                                              
    public float coefRollingResistance      = 0.05f;                            // Rolling resistance/friction of the surface the car is driving on
    public float downForce                  = 10f;                              // The downforce coefficient that is used to calculated the generated downforce of the car

    // Components
    private Transform MyTransform           = null;		                        // Reference to the Transform of this object
    private Rigidbody MyRigidBody           = null;		                        // Reference to the rigidbody of this objectFApply

    // Controller input states
    private float throttle                  = 0.0f;
    private float steering                  = 0.0f;
    private bool handbrakeOn                = false;

    // Current Operations
    private float rpm                       = 1000;                             // Current RPM of the engine
    private bool canSteer                   = false;                            // True if atleast one steering wheel is touching the ground
    private bool canDrive                   = false;                            // True if atleast one driving wheel is touching the ground
    private int currentGear                 = 0;                                // Our current gear
    private float gearChange                = 0;                                // Is the car currently doing a gear change
    Vector3 relativeVelocity;                                                   // Holds the relative velocity of our car in all 3 axis
    private float wheelRadius               = 0;                                // Used to calculate the wheel speed in calculate RPM


    // Network variables used to predict positions of cars over the network
    Vector3 m_NetworkPosition;
    Quaternion m_NetworkRotation;
	Vector3 m_NetworkVelocity;
	float m_NetworkAngularVelocity;

    double m_LastNetworkDataReceivedTime; 


    // ------------------------------------------------------------------------
    // Name	:	Awake
    // Desc	:	Called once in the scripts lifetime. Caches references to its 
    //          components
    // ------------------------------------------------------------------------
    void Awake()
    {
        // Cache Components
        MyRigidBody = rigidbody;										       // Cache rigidbody component
        MyTransform = transform;										       // Cache transform component
        wheelRadius = 0;
    }

    // ------------------------------------------------------------------------
	// Name	:	Start
	// Desc	:	Sets up initial state of the car.
	// ------------------------------------------------------------------------
    void Start()
    {
        wheels = new CarWheel[frontWheels.Length + frontWheels.Length];
        SetupWheelColliders();
        if (antiRollBars) SetupAntiRollBars();
        SetupCenterOfMass();
    }

    // ------------------------------------------------------------------------------
    // Name	:	FixedUpdate
    // Desc	:	Calls functions that update the position and rotation of the car
    // ------------------------------------------------------------------------------
    void FixedUpdate()
    {
        // If this is "our" car we apply our forces to it as per normal
        if (PhotonView.isMine == true)
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

	// ------------------------------------------------------------------------------
	// Name	:	UpdateLocalPhysics
	// Desc	:	Does the actual physics updates and applies the forces to the car
	// ------------------------------------------------------------------------------
	private void UpdateLocalPhysics()
	{
		throttle = 0.0f;
		steering = 0.0f;
		throttle = PlayerController.GetThrottle();
		steering = PlayerController.GetSteering();
		handbrakeOn = PlayerController.IsHandbrakeOn();
		
		// Subtract time from the gear change delay
		if (gearChange > 0)
			gearChange -= Time.deltaTime;
		
		// Downforce
		MyRigidBody.AddForce(-MyTransform.up * GetCurrentKPH() * 100);
		
		// Calculate the relative velocity of our transform (car)
		relativeVelocity = MyTransform.InverseTransformDirection(MyRigidBody.velocity);
		
		// Apply antiRollForce
		if (antiRollBars)
			foreach (CarAntiRollBar arb in antiRollBarList)
				arb.ApplyAntiRollForce();
		
		// Adjust friction curves based on track conditions
		UpdateFriction();
		
		// Apply Drag forces and Rolling Resistance
		ApplyResistanceForces();
		
		// If we are not in the middle of a gear change, apply motor torque
		if (gearChange <= 0)
			ApplyMotorTorque();
		
		ApplyBrakeTorque();
		
		// Calculate RPM of engine back from the drive wheels RPM.
		CalculateRPM();
		
		// Update gear according to our engine RPM
		UpdateGear();
		
		// Are we grounded, in the air etc
		CalculateState();
		
		// Apply any controller steering to our wheels
		ApplySteering(canSteer);
	}

    // We only need to synchronize a few variables to recreate a good approximation of the cars position on each client
    public void SerializeState(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            stream.SendNext(MyTransform.position);
            stream.SendNext(MyTransform.rotation);
            stream.SendNext(MyRigidBody.velocity);
			stream.SendNext(MyRigidBody.angularVelocity.magnitude);
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
		newPosition.x = Mathf.MoveTowards (MyTransform.position.x, exterpolatedTargetPosition.x, 
		                                   Mathf.Abs(MyTransform.position.x - exterpolatedTargetPosition.x) / totalTimePassed * Time.deltaTime); 
		newPosition.y = Mathf.MoveTowards (MyTransform.position.y, exterpolatedTargetPosition.y,
		                                   Mathf.Abs(MyTransform.position.y - exterpolatedTargetPosition.y) / totalTimePassed * Time.deltaTime);
		newPosition.z = Mathf.MoveTowards (MyTransform.position.z, exterpolatedTargetPosition.z,
		                                   Mathf.Abs (MyTransform.position.z - exterpolatedTargetPosition.z) / totalTimePassed * Time.deltaTime);

		// If the distance however is very long (only possible if someone had a lagspike or froze for unknown reasons), we simply
		// set the new position to the exterpolated position so that the cars do not fly accross the map when someone lags.
		// 7f was calculated as the maximum a network car could move with our current topspeed when both players have 120ping each.
		if (Vector3.Distance(MyTransform.position, exterpolatedTargetPosition) > 7f)
        {
            newPosition = exterpolatedTargetPosition;
        }

		// Update the local position
        MyTransform.position = newPosition;
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

		float angDiff = Vector3.Distance (MyTransform.rotation.eulerAngles, m_NetworkRotation.eulerAngles);

		if (angDiff < 2) 
		{
			MyTransform.rotation = Quaternion.RotateTowards (transform.rotation, m_NetworkRotation, angDiff * totalTimePassed * Time.deltaTime);
		} 
		else 
		{
			MyTransform.rotation = m_NetworkRotation;
		}
    }


    // ------------------------------------------------------------------------------
    // Name	:	SetupCenterOfMass
    // Desc	:	Sets the center of mass of the cars rigid body to the position of the
    //          transform 'CenterOfMass' set in the inspector (Unity)
    // ------------------------------------------------------------------------------
    void SetupCenterOfMass()
    {
        if (centerOfMass != null)
        {
            MyRigidBody.centerOfMass = centerOfMass.localPosition;
        }
    }

    // ------------------------------------------------------------------------------
    // Name	:	SetupAntiRollBars
    // Desc	:	Pairs up and creates a anti roll bar for the wheels on each axis
    // ------------------------------------------------------------------------------
    private void SetupAntiRollBars()
    {
        antiRollBarList = new CarAntiRollBar[2];
        CarWheel[] frontPair = new CarWheel[2];
        CarWheel[] rearPair = new CarWheel[2];
        int frontIndex = 0;
        int rearIndex = 0;

        // Divide the wheels into pairs that should be connected via anti roll bars
        foreach (CarWheel w in wheels)
        {
            if (w.frontWheel)
                frontPair[frontIndex++] = w;
            else
                rearPair[rearIndex++] = w;
        }

        // Connect the wheels pair wise
        CarAntiRollBar arbFront = new CarAntiRollBar();
        arbFront.wheelL = frontPair[0];
        arbFront.wheelR = frontPair[1];

        CarAntiRollBar arbRear = new CarAntiRollBar();
        arbRear.wheelL = rearPair[0];
        arbRear.wheelR = rearPair[1];

        // Add the rigidid body of the car that the anti roll bars will add forces to
        arbFront.carRigidBody = MyRigidBody;
        arbRear.carRigidBody = MyRigidBody;

        // Add the strength of the anti roll bars as set in the inspector
        arbFront.antiRollVal = antiRollBarStiffness;
        arbRear.antiRollVal = antiRollBarStiffness;

        antiRollBarList[0] = arbFront;
        antiRollBarList[1] = arbRear;
    }

    // --------------------------------------------------------------------------------------------
	// Name	:	ApplyMotorTorque
	// Desc	:	Calculate our motor torque and apply it to the wheel
	//          colliders do generate the force that moves us forward.
	// --------------------------------------------------------------------------------------------
    void ApplyMotorTorque()
    {
        // Get our current engine RPM
        float tRpm = Mathf.Abs(rpm);

        // 900 is the minimum. If it is too low, the car will never move.
        if (tRpm < 900) 
            tRpm = 900;

        // What is our maximum Engine Torque for the current RPM
        float MaxEngineTorque = 0.0f;

        float currSpeed = GetCurrentKPH();

        // If we reached our top speed, in reverse or normal drive, set the torque to 0
        // otherwise set it per normal
        if ( currSpeed >= topSpeed || (IsReversing() && (currSpeed >= topReverseSpeed)) )
            MaxEngineTorque = 0;
        else
            MaxEngineTorque = EngineTorqueCurve.Evaluate(tRpm);

        // To make the car weaker in reverse, only utilize 'ReverseCoeff' % of the torque in reverse
        float ReverseCoeff = IsReversing() ? 0.8f : 1f;

        // What is our actual Engine Torque based on Throttle position
        float EngineTorque = MaxEngineTorque * throttle * 0.7f * 0.7f * ReverseCoeff;

        // To make it fair, and simulate the effects of added weights etc of 4wd, lower the efficiency by 10%
        if (frontWheelDrive && rearWheelDrive)
            EngineTorque *= 0.9f;

        // MotorTorque = enginetorque * gearRatio * transmissionEfficiency
        float motorTorque = EngineTorque * gearRatios[currentGear] * 0.7f;

        // Apply the final motor torque to our wheel colliders so they can generate
        // the traction force to push us forwards (using the wheel friction curves)
        foreach (CarWheel w in wheels)
        {
            if (w.frontWheel && frontWheelDrive || (w.rearWheel && (rearWheelDrive && !handbrakeOn)))
                w.collider.motorTorque = motorTorque;
        }
    }

    // --------------------------------------------------------------------------------------------
	// Name	:	CalculateRPM
	// Desc	:	Calculate the current engine RPM from wheel RPM.
	// --------------------------------------------------------------------------------------------
    void CalculateRPM()
    {
        float wheelRPM = 0;
        float WheelSpeed = 0;
        bool first = true;

        // Get the RPM of any of the drive wheels
        foreach (CarWheel w in wheels)
        {
            wheelRPM = w.collider.rpm;

            // Is this a drive wheel, calculate the engine RPM
            if (first && (w.rearWheel && rearWheelDrive) || (w.frontWheel && frontWheelDrive))
            {
                // Calculate engine rpm from wheelRPM * gearRatio * diffRatio
                rpm = wheelRPM * gearRatios[currentGear] * diffRatio;

                // We only need one wheel
                first = false;
            }

            // wheelRPM * Circumference (2PIr) = MPM and divide by 60 to get MPS
            WheelSpeed = (wheelRPM * 2 * Mathf.PI * wheelRadius) / 60.0f;

            // If there is a significant speed differance between the car and the wheel, the wheel is skipping.
            // This is used to play sound effects and create smoke / skidmarks
            w.isSlipping = (Mathf.Abs(WheelSpeed - relativeVelocity.z) > 1.5f) ? true : false;

            //if (w.isSlipping)
            //    Debug.Log(fr + " is slipping");
        }

        // Calculate wheel speed and compare against actual speed to find our slip speed

    }

    // --------------------------------------------------------------------------------------------
    // Name	:	ApplyBrakeTorque
    // Desc	:	Applies any braking torque if the controller is braking
    //          and applies engine braking when decelerating
    // ---------------------------------------------------------------------------------------------
    void ApplyBrakeTorque()
    {
        // If we are already at a standstill don't apply braking   
        if (Mathf.Abs(relativeVelocity.z) < 0.01) 
            return;

        // Apply to each wheel (4 wheel drive)
        foreach (CarWheel w in wheels)
        {
            // Handbrake
            if (handbrakeOn && w.rearWheel)
            {
                w.collider.brakeTorque = handbrakeTorque;
            }
            else
            {
                // If we are accelerating don't apply any braking torque else apply brake torque and engine braking
                if (throttle == 0.0f)
                    w.collider.brakeTorque = coefEngineBraking;
                else if (HaveTheSameSign(throttle, relativeVelocity.z))
                    w.collider.brakeTorque = 0.0f;
                else if (GetCurrentKPH() > 5f)
                    w.collider.brakeTorque = (brakeTorque * Mathf.Abs(throttle)) + (coefEngineBraking * (Mathf.Abs(rpm) / 60.0f));
                else
                    w.collider.brakeTorque = 0.0f;

            }

        }

    }

    // ----------------------------------------------------------------------------------------
    // Name	:	ApplyResistanceForces
    // Desc	:	Applies Drag and Rolling Resistance. 
    // ---------------------------------------------------------------------------------------
    void ApplyResistanceForces()
    {
        Vector3 relDrag, sqrDrag;

        // At low speeds the drag force is proportionate to V, while at high speed to V^2.
        // Therefore lerp between the two values depending on our current speed
        sqrDrag = new Vector3(-relativeVelocity.x * Mathf.Abs(relativeVelocity.x), -relativeVelocity.y * Mathf.Abs(relativeVelocity.y), -relativeVelocity.z * Mathf.Abs(relativeVelocity.z));
        relDrag = sqrDrag; //Vector3.Lerp(-relativeVelocity, sqrDrag, Mathf.Clamp01(Mathf.Abs(GetCurrentKPH()) / 60.0f));

        // Air density for a temperature of 20 degrees C 
        float airDensity = 1.2041f;

        // Calculate Drag Equation
        float CDrag = 0.5f * coefDrag * frontalArea * airDensity;

        // Scale by velocity/velocity^2 and multiply X/Y by side area multiplier
        // if sliding sideways
        relDrag.x *= CDrag * sideAreaMultiplier;
        relDrag.z *= CDrag;
        relDrag.y *= CDrag * sideAreaMultiplier;

        // Calculate the Rolling Resistance and if we need to apply it
        // Calculate Coefficient of Rolling Resistance * Gravity
        float CrG = coefRollingResistance * 9.81f;

        // Multiply CRG by mass to get the full Rolling Resistance Force Magnitude and orient opposite to 
        // our direction of travel.
        Vector3 rollingResistanceForce = -Mathf.Sign(relativeVelocity.z) * MyTransform.forward * (CrG * MyRigidBody.mass);

        // If our velocity is too low, do not apply rolling resistance to keep us from rolling the opposite way
        if (Mathf.Abs(relativeVelocity.z) < (CrG)) rollingResistanceForce *= 0.0f;
         
        // Apply Drag
        MyRigidBody.AddForce(MyTransform.TransformDirection(relDrag), ForceMode.Impulse);

        // Only apply rolling resistance if our wheels are on the ground (at least one of them)
        if (canDrive) MyRigidBody.AddForce(rollingResistanceForce, ForceMode.Impulse);
    }

    // --------------------------------------------------------------------------------------------
    // Name	:  UpdateFriction
    // Desc	:  Adjusts friction curves to track conditions and car performance characteristics
    // --------------------------------------------------------------------------------------------
    void UpdateFriction()
    {
        float trackSlipModifier = 0.8f; // Modify this value for different materials to increase/decrease slip
    
        // Increase stiffness at very low speeds to keep car from being able to drift off sideways from stationary position
        if (GetCurrentKPH() < 3.0f && GetCurrentSidewaysKPH() < 4.0f)
        {
            sidewaysWFC.stiffness = 20.0f;
            handbrakeRWFC.stiffness = 20.0f;
            handbrakeFWFC.stiffness = 8.0f;
            
        }
        // Set the stiffness to the car's traction property (SlipFactor) scaled by the
        // track's slip factor (trackSlipModifier)
        else
        {
            // apply stiffness based on car and track parameters
            sidewaysWFC.stiffness = SlipFactor * trackSlipModifier;

            // Set the brake stiffness curve
            brakeWFC.stiffness = trackSlipModifier * SlipFactor;

            handbrakeRWFC.stiffness = trackSlipModifier * handbrakeRWStiffness;
            handbrakeFWFC.stiffness = trackSlipModifier * handbrakeFWStiffness;
        }

        // Update the curves of the wheels
        foreach (CarWheel w in wheels)
        {
            // If handbrake is on
            if (handbrakeOn)
            {
                w.collider.sidewaysFriction = (w.rearWheel) ? handbrakeRWFC : handbrakeFWFC;
            }
            else
                w.collider.sidewaysFriction = sidewaysWFC;

            // If we are accelerating (or cruising) apply standard forwards friction
            if (Mathf.Sign(throttle) == Mathf.Sign(relativeVelocity.z))
            {
                w.collider.forwardFriction = forwardsWFC;
            }
            // Otherwise we are braking so swap in a curve thats going to give us 
            // less traction and makes the car skid more
            else
            {
                w.collider.forwardFriction = brakeWFC;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Name	:	ApplySteering
    // Desc	:	Turns the front wheels based on steering axis
    // --------------------------------------------------------------------------------------------
    void ApplySteering(bool canSteer)
    {
        // Scale the angular drag of our rigidbody based on our velocity. This makes the
        // car easier to keep straight at high speeds but gives us more turn ability at low speeds
        MyRigidBody.angularDrag = Mathf.Abs(relativeVelocity.z) / 5.0f;

        // If we are braking set angular drag of rigidbody to zero making the car body easier to spin.
        // This allows you to use standard braking and gives a subtle back spinning out effect
        if (!HaveTheSameSign(throttle, relativeVelocity.z)) MyRigidBody.angularDrag = 0;

        // If our front wheels are on the ground calculate our steering
        if (canSteer)
        {
            float minMaxTurn = CalculateSteerAngle(MyRigidBody.velocity.magnitude);

            // Clamp our steer axis value to zero if very small to create a dead zone
            if (Mathf.Abs(steering) < 0.01f) steering = 0.0f;

            // Loop through each wheel
            foreach (CarWheel w in wheels)
            {
                // If its a front wheel, set its angle
                if (w.frontWheel)
                {
                    w.collider.steerAngle = steering * minMaxTurn;
                }
                else
                {
                    w.collider.steerAngle = 0;
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Name	:	ApplySteering
    // Desc	:	Turns the front wheels based on steering axis
    // --------------------------------------------------------------------------------------------
    private float CalculateSteerAngle(float speed)
    {
        //if (speed > topSpeed / 2.5f)
        //    return minimumTurn;

        float speedIndex = 1 - (speed / (topSpeed / 2.5f));

        return minimumTurn + speedIndex * (maximumTurn - minimumTurn);
    }

    private void SetupWheelColliders()
    {
        // Setup our friction curves used by our wheels
        SetupWheelFrictionCurve();
        int wheelCount = 0;

        // Loop through and create each front wheel
        foreach (Transform t in frontWheels)
        {
            wheels[wheelCount] = SetupWheel(t, true);
            wheelCount++;
        }

        // Loop through and create each rear wheel
        foreach (Transform t in rearWheels)
        {
            wheels[wheelCount] = SetupWheel(t, false);
            wheelCount++;
        }
    }

    // ------------------------------------------------------------------------------
    // Name	:	SetupWheel
    // Desc	:	This function creates the game objects and wheel colliders for
    //			each wheel. It receives the transform (Game object assigned in the 
    //          inspector) and a boolean determening if it is a front wheel (turn wheel)
    // ------------------------------------------------------------------------------
    CarWheel SetupWheel(Transform wheelTransform, bool isFrontWheel)
    {
        // Create a new game object to contain a wheel collider component
        GameObject go = new GameObject(wheelTransform.name + " Collider");

        // Set its position to the wheel's transform
        go.transform.position = wheelTransform.position;

        // Parent the wheel collider game object to this
        go.transform.parent = transform;

        // Copy the rotation from reference wheel transform as well
        go.transform.rotation = wheelTransform.rotation;

        // Add a wheel collider component and set its parent to the wheelTransform
        WheelCollider wc = go.AddComponent(typeof(WheelCollider)) as WheelCollider;
        wc.transform.parent = wheelTransform.parent;


        // Set the suspension distance
        wc.suspensionDistance = suspensionRange;

        // Fetch the wheel collider's suspension spring to we can set its properties
        JointSpring js = wc.suspensionSpring;

        // Set the suspension settings based on whether it is front or rear wheel
        if (isFrontWheel)
            js.spring = suspensionSpringFront;
        else
            js.spring = suspensionSpringRear;

        // Set Suspension Damper
        js.damper = suspensionDamper;

        // Set suspension spring
        wc.suspensionSpring = js;

        // Set mass of collider
        wc.mass = 3.2f;

        // Set Friction Curves
        wc.sidewaysFriction = sidewaysWFC;
        wc.forwardFriction = forwardsWFC;

        // Create a new Wheel structure to contain our info for this wheel
        CarWheel wheel = new CarWheel();
        wheel.collider = wc;

        // The true game object should be in a child object first transform
        wheel.wheelGraphic = wheelTransform;
        wheel.collider.radius = wheel.wheelGraphic.renderer.bounds.size.y / 2;
        wheelRadius = wheel.collider.radius;

        // If its a front wheel then insert a steer column parent so we can YAW it and let it
        // roll independantly
        if (isFrontWheel)
        {
            wheel.frontWheel = true;
            go = new GameObject(wheelTransform.name + " Steer Column");
            go.transform.position = wheelTransform.position;
            go.transform.rotation = wheelTransform.rotation;
            go.transform.parent = wheelTransform.parent;
            wheelTransform.parent = go.transform;
        }
        else
            wheel.rearWheel = true;

        // Return the newly created wheel structure
        return wheel;
    }

    // ----------------------------------------------------------------------------------
    // Name	:	SetupWheelFrictionCurve
    // Desc	:	Sets up all the wheel friction curves used by
    //			our wheels to turn slip force into traction force
    // ----------------------------------------------------------------------------------
    void SetupWheelFrictionCurve()
    {
        // This controls the laterial friction of our wheels
        sidewaysWFC = new WheelFrictionCurve();
//        sidewaysWFC.extremumSlip = 1;
//        sidewaysWFC.extremumValue = 350;
//        sidewaysWFC.asymptoteSlip = 2;
//        sidewaysWFC.asymptoteValue = 150;
        sidewaysWFC.extremumSlip = 1f;
        sidewaysWFC.extremumValue = 6000;
        sidewaysWFC.asymptoteSlip = 2.0f;
        sidewaysWFC.asymptoteValue = 400;

        // This controls our normal longitudinal friction
        forwardsWFC = new WheelFrictionCurve();
//        forwardsWFC.extremumSlip = 0.5f;
//        forwardsWFC.extremumValue = 6000;
//        forwardsWFC.asymptoteSlip = 2.0f;
//        forwardsWFC.asymptoteValue = 400;
        forwardsWFC.extremumSlip = 1f;
        forwardsWFC.extremumValue = 20000;
        forwardsWFC.asymptoteSlip = 2.0f;
        forwardsWFC.asymptoteValue = 10000;
        forwardsWFC.stiffness = 0.82f;

        // I swap this friction curve for the normal forwards curve 
        // when the car is braking.
        brakeWFC = new WheelFrictionCurve();
        brakeWFC.extremumSlip = 1;
        brakeWFC.extremumValue = 500;
        brakeWFC.asymptoteSlip = 2;
        brakeWFC.asymptoteValue = 250;
        brakeWFC.stiffness = 1.0f;

        // Handbrake friction curve for the rear wheels, to make the car drift easier while handbraking
        handbrakeRWFC = new WheelFrictionCurve();
        handbrakeRWFC.extremumSlip = 1;
        handbrakeRWFC.extremumValue = 350;
        handbrakeRWFC.asymptoteSlip = 2;
        handbrakeRWFC.asymptoteValue = 150;

        // Handbrake friction curve for the rear wheels, to make the car drift easier while handbraking
        handbrakeFWFC = new WheelFrictionCurve();
        handbrakeFWFC.extremumSlip = 1;
        handbrakeFWFC.extremumValue = 350;
        handbrakeFWFC.asymptoteSlip = 2;
        handbrakeFWFC.asymptoteValue = 150;
        handbrakeFWFC.stiffness = 0.3f;
    }

    // ----------------------------------------------------------------------------------
    // Name	:	CalculateState
    // Desc	:	Do we have driving and steering ability
    // -----------------------------------------------------------------------------------
    void CalculateState()
    {
        canDrive = false;
        canSteer = false;
        foreach (CarWheel w in wheels)
        {
            if (w.collider.isGrounded)
            {
                canDrive = true;
                if (w.frontWheel) canSteer = true;
            }
        }
        //Debug.Log("CanDrive = " + canDrive + ", CanSteer = " + canSteer);
    }

    // ------------------------------------------------------------------------------
    // Name :	UpdateGear
    // Desc :   Changes gear based on current engine RPM
    // ------------------------------------------------------------------------------
    void UpdateGear()
    {
        // If we already are in the middle of a gear change, return
        if (gearChange > 0)
            return;

        // If we aren't in our top gear, have more rpm than the shift-up threshhold and are accelerating,
        // Shift up!
        if (currentGear < changeGearAtRPM.Length && rpm > changeGearAtRPM[currentGear] && throttle > 0.0f)
        {
            currentGear++;

            // Gearchange delay
            gearChange = timeToShiftGear;
        }
        // If we are not in the lowest gear, we are below 2500 rpm and braking or just below 1000 rpm
        // in general. It is time to shift down.
        else if (currentGear > 0 && ((rpm < 2500 && throttle <= 0.0f) || rpm < 2200))
        {
            currentGear--;

            // Gearchange delay
            gearChange = timeToShiftGear;
        }
    }

    // ===========================================================================================
    // 	HELPER FUNCTIONS
    // ============================================================================================

    // --------------------------------------------------------------------------------------------
    // Name : GetWheels()
    // Desc	: Returns an array with the Wheels
    // --------------------------------------------------------------------------------------------
    public CarWheel[] GetWheels()
    {
        return wheels;
    }

    // --------------------------------------------------------------------------------------------
    // Name : IsReversing()
    // Desc	: Return true if the car is in reverse
    // --------------------------------------------------------------------------------------------
    public bool IsReversing()
    {
        // If the car is moving backwards and the player is pressing reverse
        return (relativeVelocity.z < 0.0f);
    }

    // --------------------------------------------------------------------------------------------
    // Name : GetCurrentKPH()
    // Desc	: Return the current forward speed of the car in Km/h
    // --------------------------------------------------------------------------------------------
    public float GetCurrentKPH()
    {
        return Mathf.Abs(Convert_Meters_Per_Second_To_Kilometers_Per_Hour( Vector3.Dot(MyRigidBody.velocity, MyTransform.forward) ));
    }

    // --------------------------------------------------------------------------------------------
    // Name : GetCurrentSidewaysKPH()
    // Desc	: Return the current sideways speed of the car in Km/h
    // --------------------------------------------------------------------------------------------
    public float GetCurrentSidewaysKPH()
    {
        return Mathf.Abs(Convert_Meters_Per_Second_To_Kilometers_Per_Hour(Vector3.Dot(MyRigidBody.velocity, MyTransform.right)));
    }

	// --------------------------------------------------------------------------------------------
	// Name : GetTopSpeed()
	// Desc	: Return the top speed
	// --------------------------------------------------------------------------------------------
	public float GetTopSpeed()
	{
		return topSpeed;
	}

    // --------------------------------------------------------------------------------------------
    // Name : GetCurrentRPM()
    // Desc	: Return the current rpm of the engine
    // --------------------------------------------------------------------------------------------
    public float GetCurrentRPM()
    {
        return Mathf.Abs(rpm);
    }

    // --------------------------------------------------------------------------------------------
    // Name : GetCurrentGear()
    // Desc	: Return the current gear. Since the code uses 0-indexed gear ratio arrays the real gear
    //        Is always currentGear + 1
    // --------------------------------------------------------------------------------------------
    public float GetCurrentGear()
    {
        return currentGear + 1;
    }

    // --------------------------------------------------------------------------------------------
    // Name : GetPosition
    // Desc	: Return our position
    // --------------------------------------------------------------------------------------------
    public Vector3 GetPosition()
    {
        return MyTransform.position;
    }

    // --------------------------------------------------------------------------------------------
    // Name : Convert_Kilometerss_Per_Hour_To_Meters_Per_Second
    // --------------------------------------------------------------------------------------------	
    float Convert_Kilometers_Per_Hour_To_Meters_Per_Second(float value)
    {
        return value * 0.27778f;
    }

    // --------------------------------------------------------------------------------------------
    // Name : Convert_Meters_Per_Second_To_Kilometers_Per_Hour
    // --------------------------------------------------------------------------------------------	
    float Convert_Meters_Per_Second_To_Kilometers_Per_Hour(float value)
    {
        return value * 3.6f;
    }

    // --------------------------------------------------------------------------------------------
    // Name : HaveTheSameSign
    // Desc	: Returns true if both input parameters have the same sign
    // --------------------------------------------------------------------------------------------	
    bool HaveTheSameSign(float first, float second)
    {
        if (Mathf.Sign(first) == Mathf.Sign(second))
            return true;
        else
            return false;
    }

	//Debug Display of test variables
	void OnGUI()
	{
		if (PhotonView.isMine == true)
		{
			GUILayout.BeginArea (new Rect (10, 10, Screen.width, Screen.height));
			{
					GUILayout.Label ("Local car");
					GUILayout.Label ("RPM: " + GetCurrentRPM());
					GUILayout.Label ("Position: " + MyTransform.position);
					GUILayout.Label ("Rotation: " + MyTransform.rotation);
					GUILayout.Label ("Velocity: " + MyRigidBody.velocity);
					GUILayout.Label ("Angular Velocity: " + MyRigidBody.angularVelocity.magnitude);
					GUILayout.Label ("Ping: " + (float)PhotonNetwork.GetPing ());
			}
			GUILayout.EndArea ();
		} 
		else 
		{
			GUILayout.BeginArea (new Rect (1050, 10, Screen.width, Screen.height));
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
				GUILayout.Label ("Position: " + MyTransform.position);
				GUILayout.Label ("Rotation: " + MyTransform.rotation);
				GUILayout.Label ("Velocity: " + MyRigidBody.velocity);
				GUILayout.Label ("Angular Velocity: " + MyRigidBody.angularVelocity.magnitude);
			}
			GUILayout.EndArea ();
		}
	}
}
