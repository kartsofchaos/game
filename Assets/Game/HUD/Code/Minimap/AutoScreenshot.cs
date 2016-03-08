using UnityEngine;
using System.Collections;
using System.IO;

public class AutoScreenshot : MonoBehaviour {

	public float SnapDelay = 0.1f;
	public string SegmentName = "segment";
	public float Aspect = 1.0f;
	public int SuperSize = 2;
	private float mapMaxX;
	private float mapMaxZ;
	private float xInc;
	private float zInc;

	IEnumerator Start() {
		if (SnapDelay < 0.1f) {
			this.SnapDelay = 1.0f;
			Debug.LogWarning("Snap delay cannot be lower than 1 second.");
		}

		// Make sure that the game is not active
		GameObject game = GameObject.FindGameObjectWithTag("Game");
		game.SetActive(false);

		// Make sure all other cameras are disabled
		var cameras = Camera.allCameras;
		for (int i = 0; i < cameras.Length; i++) {
			if(cameras[i].enabled) {
				GetComponent<Camera>().enabled = false;
				//Debug.LogError("Disable all other cameras before running AutoSnapshot");
				//yield break;
			}
		}

		// TODO: Disable all unwanted environment

		// Create destination folder if not exists
		EditorHelper.CreateAssetFolderIfNotExists("Game/HUD/Textures/Minimap");
		
		// Setup minimap camera
		GetComponent<Camera>().enabled = true;
		GetComponent<Camera>().aspect = Aspect;

		var xHalfUnit = GetComponent<Camera>().orthographicSize * Aspect;
		var zHalfUnit = GetComponent<Camera>().orthographicSize;

		xInc = xHalfUnit * 2;
		zInc = zHalfUnit * 2;

		// Center 0,0,0
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		mapMaxX = terrainData.size.x / 2;
		mapMaxZ = terrainData.size.z / 2;

		for (float x = mapMaxX * -1; x < mapMaxX + xHalfUnit; x += xInc) {
			for (float z = mapMaxZ * -1; z < mapMaxZ + zHalfUnit; z += zInc) {
				this.moveCamera(x, z);
				//var coords = GetSegmentCoorsForPosition(x, z);
				//Debug.Log(coords);
				Application.CaptureScreenshot(string.Format(HUDConstants.PATH_MINIMAP_TEXTURES + "/" + HUDConstants.MINIMAP_SEGMENT_PATTERN + ".png", x, z, SegmentName));
				yield return new WaitForSeconds(SnapDelay);
			}
		}

		// Write map data
		using (var writer = new StreamWriter(HUDConstants.PATH_MINIMAP_SETTINGS)) {
			writer.WriteLine(string.Format("name={0}", SegmentName));
			writer.WriteLine(string.Format("length={0}", xInc));
			writer.WriteLine(string.Format("width={0}", zInc));
			writer.WriteLine(string.Format("xMin={0}", mapMaxX*-1));
			writer.WriteLine(string.Format("xMax={0}", mapMaxX));
			writer.WriteLine(string.Format("zMin={0}", mapMaxZ*-1));
			writer.WriteLine(string.Format("zMax={0}", mapMaxZ));
		}

		// Disable camera
		GetComponent<Camera>().enabled = false;
	}
	// Gets the "upper" corner-coordinate of the closets segment
	private Vector2 GetSegmentCoorsForPosition(float x, float z)
	{
		// Get distance from top, xMin = -xMax
		var distOrigoBottom = mapMaxX + x;
		
		// Get distance from left, zMin = -zMax
		var distOrigoLeft = mapMaxZ + z;
		
		
		//Debug.Log("GetSegmentCoorsForPosition: (" + x + "," + z + ")");
		//Debug.Log("Origo coords: (" + distOrigoBottom + "," + distOrigoLeft + ")");
		/*float offX = mapOffset.x, offZ = mapOffset.z;
	    if (x < 0)
	        offX *= -1;
	    if (z < 0)
	        offZ *= -1;

    	x = Mathf.Clamp(x + offX, mapSettings.xMin, mapSettings.xMax);
		z = Mathf.Clamp(z + offZ, mapSettings.xMin, mapSettings.xMax);
		Debug.Log("GetSegmentCoorsForPosition: (" + x + "," + z + ")");
*/
		var segmentX = (int) (distOrigoBottom / xInc);
		var segmentZ = (int) (distOrigoLeft / zInc);
		var newX = segmentX * xInc;
		var newZ = segmentZ * zInc;
		//Debug.Log("New-GetSegmentCoorsForPosition: (" + newX + "," + newZ + ")");
		return new Vector2(newX, newZ);
	}

	// Move camera to (x, z)
	private void moveCamera(float x, float z) {
		this.transform.position = new Vector3(RoundToNearestPixel(x, GetComponent<Camera>()), this.transform.position.y, RoundToNearestPixel(z, GetComponent<Camera>()));
	}
	
	// Round a position to not overlap in pixels (TODO: Not working)
	private float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
	{
		float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
		valueInPixels = Mathf.Round(valueInPixels);
		float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
		return adjustedUnityUnits;
	}
}
