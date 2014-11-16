using UnityEngine;
using System.Collections;

public class TeleportTeleporter : MonoBehaviour {
  [SerializeField]
  Transform target;

  void OnTriggerEnter (Collider c) {
    BoltEntity entity = c.GetComponent<BoltEntity>();

    if (entity && entity.boltIsOwner) {
      entity.Teleport(target.position);
      entity.GetComponent<TeleportPlayerMotor>().velocity = Vector3.zero;
    }
  }

}
