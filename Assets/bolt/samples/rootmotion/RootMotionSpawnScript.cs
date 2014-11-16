using UnityEngine;

public class RootMotionSpawnScript : BoltCallbacks {
  void Awake () {
    DontDestroyOnLoad(gameObject);
  }

  public override void MapLoadDone (BoltMapAsset arg) {
    Debug.Log("Meep");
    BoltNetwork.Instantiate(BoltPrefabs.RootMotionChar);
  }
}
