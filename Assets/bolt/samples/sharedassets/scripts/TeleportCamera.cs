using UnityEngine;

public class TeleportCamera : BoltSingletonPrefab<TeleportCamera> {
  float targetYaw;
  float targetPitch;
  float targetDistance;

  float currentYaw;
  float currentPitch;
  float currentDistance;
  float currentMinDistance;
  float currentMaxDistance;

  float realDistance = 0f;

  public bool DisplayDebugGizmos = true;

  public Camera cam = null;
  public Transform target = null;

  public float MinDistance = 1f;
  public float MaxDistance = 32f;
  public float MinPitch = -80f;
  public float MaxPitch = 80f;
  public float ZoomSpeed = 16f;
  public float RotationMouseSpeed = 4f;

  public bool SmoothZoom = true;
  public float SmoothZoomSpeed = 8f;

  public bool SmoothRotation = true;
  public float SmoothRotationSpeed = 8f;

  public bool SmoothAutoRotation = true;
  public float SmoothAutoRotationSpeed = 4f;

  public LayerMask Obstacles = 0;
  public Vector3 TargetOffset = Vector3.zero;

  public string ZoomAxis = "Mouse ScrollWheel";
  public string YawAxis = "Mouse X";
  public string PitchAxis = "Mouse Y";
  public string MouseRotateButton = "Fire1";
  public string MouseLookButton = "Fire2";

  public bool lockCameraBehindTarget { get; set; }
  public bool rotateCameraBehindTarget { get; set; }

  public bool HasCamera { get { return cam != null; } }
  public bool HasTarget { get { return target != null; } }
  public Vector3 TargetPosition { get { return HasTarget ? target.position + TargetOffset : TargetOffset; } }

  void Start () {
    if (!HasCamera) {
      cam = GetComponentInChildren<Camera>();
    }

    if (!HasTarget) {
      try {
        target = GameObject.FindGameObjectWithTag("CameraTarget").transform;
      } catch {

      }
    }

    if (HasCamera) {
      cam.transform.localPosition = Vector3.zero;
      cam.transform.localRotation = Quaternion.identity;
    }

    MinPitch = Mathf.Clamp(MinPitch, -85f, 0f);
    MaxPitch = Mathf.Clamp(MaxPitch, 0f, 85f);
    MinDistance = Mathf.Max(0f, MinDistance);

    currentMinDistance = MinDistance;
    currentMaxDistance = MaxDistance;

    currentYaw = targetYaw = 0f;
    currentPitch = targetPitch = Mathf.Lerp(MinPitch, MaxPitch, 0.6f);
    currentDistance = targetDistance = realDistance = Mathf.Lerp(MinDistance, MaxDistance, 0.5f);
  }

  void Update () {
    if (!HasCamera) {
      return;
    }

    if (!HasTarget) {
      return;
    }

    bool mouseLook = GetButtonSafe(MouseLookButton, false);
    bool rotate = GetButtonSafe(MouseRotateButton, false);

    Screen.lockCursor = mouseLook;
    Screen.showCursor = !mouseLook;

    bool smoothRotation = SmoothRotation || SmoothAutoRotation;
    float smoothRotationSpeed = SmoothRotationSpeed;

    rotateCameraBehindTarget = GetButtonSafe(MouseLookButton, false);

    // This defines our "real" distance to the player
    realDistance -= GetAxisRawSafe(ZoomAxis, 0f) * ZoomSpeed;
    realDistance = Mathf.Clamp(realDistance, MinDistance, MaxDistance);

    // This is the distance we want to (clamped to what is viewable)
    targetDistance = realDistance;
    targetDistance = Mathf.Clamp(targetDistance, currentMinDistance, currentMaxDistance);

    // This is our current distance
    if (SmoothZoom) {
      currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * SmoothZoomSpeed);
    } else {
      currentDistance = targetDistance;
    }

    // Calculate offset vector
    Vector3 offset = new Vector3(0, 0, -currentDistance);

    // LMB is not down, but we should rotate camera behind target
    if (!rotate && rotateCameraBehindTarget) {
      targetYaw = SignedAngle(offset.normalized, -target.forward, Vector3.up);
      smoothRotation = SmoothAutoRotation;
      smoothRotationSpeed = SmoothAutoRotationSpeed;
    }

    // Only LMB down and no lock
    if ((rotate || mouseLook) && !lockCameraBehindTarget) {
      if (!mouseLook)
        targetYaw += (GetAxisRawSafe(YawAxis, 0f) * RotationMouseSpeed);

      targetPitch -= (GetAxisRawSafe(PitchAxis, 0f) * RotationMouseSpeed);
      targetPitch = Mathf.Clamp(targetPitch, MinPitch, MaxPitch);
      smoothRotation = SmoothRotation;
      smoothRotationSpeed = SmoothRotationSpeed;
    }

    // RMB 
    if (mouseLook && lockCameraBehindTarget) {
      targetPitch -= (GetAxisRawSafe(PitchAxis, 0f) * RotationMouseSpeed);
      targetPitch = Mathf.Clamp(targetPitch, MinPitch, MaxPitch);
    }

    // Lock camera behind target, this overrides everything
    if (lockCameraBehindTarget) {
      targetYaw = SignedAngle(offset.normalized, -target.transform.forward, Vector3.up);
      smoothRotation = false;
    }

    // Clamp targetYaw to -180, 180
    targetYaw = Mathf.Repeat(targetYaw + 180f, 360f) - 180f;

    if (!smoothRotation) {
      currentYaw = targetYaw;
      currentPitch = targetPitch;
    } else {
      // Clamp smooth currentYaw to targetYaw and clamp it to -180, 180
      currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, Time.deltaTime * smoothRotationSpeed);
      currentYaw = Mathf.Repeat(currentYaw + 180f, 360f) - 180f;

      // Smooth pitch
      currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, Time.deltaTime * smoothRotationSpeed);
    }

    // Rotate offset vector
    offset = Quaternion.Euler(currentPitch, currentYaw, 0f) * offset;

    // Position camera holder correctly
    transform.position = TargetPosition + offset;

    // And then have the camera look at our target
    cam.transform.LookAt(TargetPosition);

    currentMinDistance = MinDistance;
    currentMaxDistance = MaxDistance;

    // Clear this flag
    lockCameraBehindTarget = false;
    rotateCameraBehindTarget = false;
  }

  bool AvoidCollision (Vector3 point, ref float closest) {
    RaycastHit hit;
    Vector3 direction = (point - TargetPosition).normalized;

    if (Physics.Raycast(TargetPosition, direction, out hit, MaxDistance, Obstacles)) {
      float calculatedDistance = (hit.point - target.position).magnitude;

      if (calculatedDistance < closest) {
        closest = calculatedDistance;
      }

      return false;
    }

    return true;
  }

  void OnDrawGizmos () {
    if (DisplayDebugGizmos && HasTarget) {
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(TargetPosition, currentMinDistance);

      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(TargetPosition, currentMaxDistance);

      Gizmos.color = Color.magenta;
      Gizmos.DrawLine(TargetPosition, cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)));
      Gizmos.DrawLine(TargetPosition, cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane)));
      Gizmos.DrawLine(TargetPosition, cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.nearClipPlane)));
      Gizmos.DrawLine(TargetPosition, cam.ScreenToWorldPoint(new Vector3(0, Screen.height, cam.nearClipPlane)));
      Gizmos.DrawLine(TargetPosition, transform.position);
    }
  }

  public static float SignedAngle (Vector3 v1, Vector3 v2, Vector3 n) {
    return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
  }

  public static bool GetButtonSafe (string name, bool @default) {
    try {
      return Input.GetButton(name);
    } catch {
      Debug.LogError("The button '" + name + "' isn't defined in the input manager");
      return @default;
    }
  }

  public static bool GetButtonDownSafe (string name, bool @default) {
    try {
      return Input.GetButtonDown(name);
    } catch {
      Debug.LogError("The button '" + name + "' isn't defined in the input manager");
      return @default;
    }
  }

  public static float GetAxisRawSafe (string name, float @default) {
    try {
      return Input.GetAxisRaw(name);
    } catch {
      Debug.LogError("The axis '" + name + "' isn't defined in the input manager");
      return @default;
    }
  }
}