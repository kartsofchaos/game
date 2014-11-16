using UnityEngine;
using System.Collections;

public class TeleportElevator : MonoBehaviour {
  [SerializeField]
  Vector3 from;

  [SerializeField]
  Vector3 to;

  void OnTriggerEnter (Collider c) {
    BoltEntity entity = c.GetComponent<BoltEntity>();

    if (entity && entity.boltIsOwner) {
      entity.SetOrigin(transform);
    }
  }

  void OnTriggerExit (Collider c) {
    BoltEntity entity = c.GetComponent<BoltEntity>();

    if (entity && entity.boltIsOwner) {
      entity.SetOrigin(null);
    }
  }

  void FixedUpdate () {
    transform.position = Vector3.Lerp(from, to, Mathf.PingPong((BoltNetwork.serverTime + 15f) * 0.1f, 1f));
  }
}
