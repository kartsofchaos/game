using UnityEngine;
using System.Collections;

public class TeleportServerCallbacks : BoltCallbacks {
  void Awake () {
    GameObject.DontDestroyOnLoad(gameObject);
  }

  public override void ClientDisconnected (BoltConnection arg) {
    BoltEntity entity = arg.userToken as BoltEntity;

    if (entity) {
      BoltNetwork.Destroy(entity);
    }
  }

  public override void ClientConnected (BoltConnection arg) {
    BoltEntity entity = SpawnAvatar();
    arg.userToken = entity;
    entity.GiveControl(arg);
  }

  public override void MapLoadDone (BoltMapAsset arg) {
    SpawnAvatar().TakeControl();
  }

  BoltEntity SpawnAvatar () {
    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.TeleportPlayer);

    entity.transform.position = new Vector3(
      Random.Range(-8f, 8f),
      0f,
      Random.Range(-8f, 8f)
    );

    return entity;
  }

}
