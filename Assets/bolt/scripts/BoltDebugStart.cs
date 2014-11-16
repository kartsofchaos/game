using UdpKit;
using UnityEngine;
using Process = System.Diagnostics.Process;

public partial class BoltDebugStart : BoltCallbacksBase {
  [SerializeField]
  Texture2D logo;

  void Awake () {
    DontDestroyOnLoad(gameObject);
  }

  void Start () {
#if UNITY_EDITOR_OSX
    Process p = new Process();
    p.StartInfo.FileName = "osascript";
    p.StartInfo.Arguments = 

@"-e 'tell application """ + UnityEditor.PlayerSettings.productName + @"""
	activate
end tell'";

    p.Start();
#endif

    BoltConfig config =
      BoltEditorSettings.instance.debugStartConfig
        ? BoltEditorSettings.instance.debugStartConfig.config
        : BoltNetwork.defaultConfig;

    UdpEndPoint _serverEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, (ushort) BoltEditorSettings.instance.debugStartPort);
    UdpEndPoint _clientEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, 0);

    if (BoltEditorSettings.instance.debugStartMap) {
      if (BoltDebugStartSettings.startServer) {
        BoltNetwork.InitializeServer(_serverEndPoint, config);
        BoltNetwork.LoadMap(BoltEditorSettings.instance.debugStartMap);
      } else if (BoltDebugStartSettings.startClient) {
        BoltNetwork.InitializeClient(_clientEndPoint, config);
        BoltNetwork.Connect(_serverEndPoint);
      }

      BoltDebugStartSettings.PositionWindow();
    } else {
      BoltLog.Error("No map found to start from");
    }

    if (!BoltNetwork.isClient && !BoltNetwork.isServer) {
      BoltLog.Error("failed to start debug mode");
    }
  }

  void OnGUI () {
    if (logo) {
      GUI.DrawTexture(new Rect(10, Screen.height - 148, 256, 138), logo);
    }
  }

  public override void MapLoadDone (BoltMapAsset arg) {
    Destroy(gameObject);
  }
}
