using UnityEngine;
using System.Collections;

public class SphereSerializer : BoltEntitySerializer<ISphereState> {
  float rate = 0f;

  void Awake () {
    rate = Random.Range(0.5f, 1f);
    transform.position = new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f));
  }

  void Update () {
    foreach (BoltConnection cn in BoltNetwork.clients) {
      float f = 1f - Mathf.Clamp01(cn.GetSkippedUpdates(boltEntity) / 10f);
      transform.GetChild(0).renderer.material.color = new Color(1, f, f, 1);
    }
  }

  public override void SimulateOwner () {
    transform.RotateAround(Vector3.zero, Vector3.up, rate);
  }
  
  public override void Attached (ISphereState state) {
    Debug.Log("ATTACHED");
    base.Attached(state);
  }
}
