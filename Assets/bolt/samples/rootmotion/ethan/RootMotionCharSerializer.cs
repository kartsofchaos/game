using UnityEngine;
using System.Collections;

public class RootMotionCharSerializer : BoltEntitySerializer<IRootMotionCharState> {
  public override void Attached (IRootMotionCharState state) {
    if (boltEntity.boltIsOwner) {
      // toggle root motion on
      GetComponent<Animator>().applyRootMotion = true;

      // enable movement script
      GetComponent<RootMotionCharMovement>().enabled = true;
    }
  }
}
