using UdpKit;
using UnityEngine;

public class BoltDebugStartNonPro : MonoBehaviour {
  [SerializeField]
  Texture2D logo;

  void OnGUI () {
    UdpEndPoint serverEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, (ushort) BoltEditorSettings.instance.debugStartPort);
    UdpEndPoint clientEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, 0);

    if (logo) {
      GUI.DrawTexture(new Rect(10, Screen.height - 148, 256, 138), logo);
    }

    const int MARGIN = 5;

    BoltConfig config =
      BoltEditorSettings.instance.debugStartConfig
        ? BoltEditorSettings.instance.debugStartConfig.config
        : BoltNetwork.defaultConfig;

    GUILayout.BeginArea(new Rect(MARGIN, MARGIN, Screen.width - (MARGIN * 2), (Screen.height * 0.5f) - (MARGIN * 2)));
    GUILayout.BeginHorizontal();

    if (BoltEditorSettings.instance.debugStartMap) {
      if (GUILayout.Button("Start Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
        BoltNetwork.InitializeServer(serverEndPoint, config);
        BoltNetwork.LoadMap(BoltEditorSettings.instance.debugStartMap);
        GameObject.Destroy(gameObject);
      }

      if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
        BoltNetwork.InitializeClient(clientEndPoint, config);
        BoltNetwork.Connect(serverEndPoint);
        GameObject.Destroy(gameObject);
      }
    } else {
      GUI.color = Color.red;
      GUILayout.Label("No map specified.");
      GUI.color = Color.white;
    }

    GUILayout.EndHorizontal();
    GUILayout.EndArea();
  }
}
