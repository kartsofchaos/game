using UnityEngine;
using System.Collections;

public class TeleportCallbacks : BoltCallbacks {
  public override void ControlOfEntityGained (BoltEntity arg) {
    TeleportCamera.instance.target = arg.transform;
  }
}
