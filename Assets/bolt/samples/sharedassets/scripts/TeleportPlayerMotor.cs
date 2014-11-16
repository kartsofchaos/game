using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TeleportPlayerMotor : MonoBehaviour {

  bool _grounded;
  Vector3 _velocity;
  CharacterController _cc;

  [SerializeField]
  float skinWidth = 0.08f;

  [SerializeField]
  float gravityForce = -9.81f;

  [SerializeField]
  float movementSpeed = 4f;

  float sphereRadius {
    get { return _cc.radius - (skinWidth * 0.2f); }
  }

  Vector3 feetPosition {
    get {
      Vector3 p = transform.position;

      p.y += _cc.radius;
      p.y += _cc.center.y;
      p.y -= (_cc.height * 0.5f);
      p.y -= (skinWidth * 1.25f);

      return p;
    }
  }

  public Vector3 position {
    get { return transform.localPosition; }
  }

  public Vector3 velocity {
    get { return _velocity; }
    set { _velocity = value; }
  }

  public bool grounded {
    get { return _grounded; }
  }

  public Collider ground {
    get;
    private set;
  }

  public void SetState (Vector3 position, Vector3 velocity, bool grounded) {
    transform.localPosition = position;

    _velocity = velocity;
    _grounded = grounded;
  }

  public void Move (Vector3 direction) {

    // clamp to ground
    RaycastHit hit;

    if (Physics.Raycast(feetPosition, Vector3.down, out hit, sphereRadius * 1.1f)) {
      Vector3 p = hit.point;
      p.y += skinWidth;

      ground = hit.collider;

      transform.position = p;
    } else {
      ground = null;
    }

    // update grounding
    if (Physics.OverlapSphere(feetPosition, sphereRadius).Count(x => x.isTrigger == false && (x is CharacterController) == false) > 0) {
      if (_grounded == false) {
        _grounded = true;

        // reset velocity to 0 when we reach the ground
        _velocity.y = 0f;
      }
    } else {
      _grounded = false;
    }

    // if we are grounded (walking)
    if (_grounded) {
      if (direction != Vector3.zero) {
        _velocity = direction.normalized * movementSpeed;
        _cc.Move(_velocity * BoltNetwork.frameDeltaTime);

      } else {
        _velocity = Vector3.zero;
      }
    }

    // we are not grounded (falling)
    else {
      _velocity.y += (gravityForce * BoltNetwork.frameDeltaTime);
      _cc.Move(_velocity * BoltNetwork.frameDeltaTime);
    }
  }

  void Awake () {
    _cc = GetComponent<CharacterController>();
    _cc.center = new Vector3(0, 1, 0);
  }

  void OnDrawGizmos () {
    if (Application.isPlaying && _cc) {
      Gizmos.color = _grounded ? Color.green : Color.red;
      Gizmos.DrawWireSphere(feetPosition, sphereRadius);

      Vector3 fp = feetPosition;
      fp.y -= sphereRadius;

      Gizmos.color = Color.white;
      Gizmos.DrawSphere(fp, 0.1f);
    }
  }
}
