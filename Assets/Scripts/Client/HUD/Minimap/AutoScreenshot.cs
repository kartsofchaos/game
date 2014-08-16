using UnityEngine;
using System.Collections;
using System.IO;

public class AutoScreenshot : MonoBehaviour {

	public float SnapDelay = 1.0f;
	public string SegmentName = "segment";
	public float Aspect = 1.0f;
	public int SuperSize = 2;

	IEnumerator Start () {
		if (SnapDelay < 0.1f) {
			this.SnapDelay = 1.0f;
			Debug.LogWarning("Snap delay cannot be lower than 1 second.");
		}

		camera.enabled = true;
		var cameras = Camera.allCameras;
		var activeCameras = 0;
		for (int i = 0; i < cameras.Length; i++) {
			if(cameras[i].enabled) activeCameras++;
			if(activeCameras > 1) {
				Debug.LogError("Disable all other cameras before running AutoSnapshot");
				yield break;
			}
		}

		camera.aspect = Aspect;

		var xHalfUnit = camera.orthographicSize * Aspect;
		var zHalfUnit = camera.orthographicSize;

		this.moveCamera(xHalfUnit, zHalfUnit);
		var xInc = xHalfUnit * 2;
		var zInc = zHalfUnit * 2;

		// Center 0,0,0
		var xTerrainMax = 300;//Terrain.activeTerrain.terrainData.size.x/2;
		var zTerrainMax = 300;//Terrain.activeTerrain.terrainData.size.z/2;
		Debug.Log ("(" + xTerrainMax + "," + zTerrainMax + ")");

		EditorHelper.CreateAssetFolderIfNotExists("StaticAssets/Textures/Minimap");
		int xImage = 0;
		int zImage;
		for (float x = xTerrainMax*-1; x < xTerrainMax + xHalfUnit; x += xInc) {
			zImage = 0;
			for (float z = xTerrainMax*-1; z < zTerrainMax + zHalfUnit; z += zInc) {
				this.moveCamera(x, z);
				Application.CaptureScreenshot(string.Format("Assets/StaticAssets/Textures/Minimap/{0}-{1}.{2}.png", x, z, SegmentName));
				yield return new WaitForSeconds(SnapDelay);
				zImage++;
			}
			xImage++;
		}

		using (var writer = new StreamWriter("Assets/Scripts/Client/Hud/Minimap/MinimapSettings.txt")) {
			writer.WriteLine(string.Format("name={0}", SegmentName));
			writer.WriteLine(string.Format("length={0}", xInc));
			writer.WriteLine(string.Format("width={0}", zInc));
			writer.WriteLine(string.Format("xMin={0}", xTerrainMax*-1));
			writer.WriteLine(string.Format("xMax={0}", xTerrainMax));
			writer.WriteLine(string.Format("zMin={0}", zTerrainMax*-1));
			writer.WriteLine(string.Format("zMax={0}", zTerrainMax));
		}
	}
	
	void Update () {
	
	}

	private void moveCamera(float x, float z) {
		this.transform.position = new Vector3(RoundToNearestPixel(x, camera), this.transform.position.y, RoundToNearestPixel(z, camera));
	}

	private float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
	{
		float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * Aspect)) * unityUnits;
		valueInPixels = Mathf.Round(valueInPixels);
		float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * Aspect));
		return adjustedUnityUnits;
	}
}
