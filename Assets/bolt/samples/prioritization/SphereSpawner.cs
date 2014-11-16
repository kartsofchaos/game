using UnityEngine;
using System.Collections;

public class SphereSpawner : BoltCallbacks {
  public override void MapLoadDone (BoltMapAsset arg) {
    for (int i = 0; i < 128; ++i) {
      BoltNetwork.Instantiate(BoltPrefabs.Sphere);
    }
  }
}
