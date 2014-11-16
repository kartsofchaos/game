using UnityEngine;

public class RootMotionCharMovement : BoltEntityBehaviour<RootMotionCharSerializer, IRootMotionCharState> {
  Animator anim;

  public float turnSmoothing = 15f;

  void Awake () {
    anim = GetComponent<Animator>();
  }

  void FixedUpdate () {
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    MovementManagement(h, v, Input.GetKey(KeyCode.LeftShift));
  }

  void MovementManagement (float horizontal, float vertical, bool sneaking) {
    boltState.mecanim.Sneaking = sneaking;

    if (horizontal != 0f || vertical != 0f) {
      Rotating(horizontal, vertical);
      boltState.mecanim.Speed = 3.5f;
    } else
      boltState.mecanim.Speed = 0f;
  }

  void Rotating (float horizontal, float vertical) {
    Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
    Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
    Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
    rigidbody.MoveRotation(newRotation);
  }
}
