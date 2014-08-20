using UnityEngine;
using System.Collections;

public class MapLoader : HUDBase, IMapLoader {

	private Transform player;		// Used to fetch the position of the player
	public Transform playerIcon;
	public Transform mask;
	
	internal bool fullscreen;
	public bool toggleFullscreen;
	private bool toggling;

	private MapHandler mapHandler;

	private float mapCheck = 1f;
	private float timer = 0;

	private Vector3 newPosition;
	private Vector3 newMaskPosition;
	private Vector3 newRotation;

	private float rotationDuration = 0.3f;
	private float zoomDuration = 0.7f;
	private float moveDuration = 0.5f;

	private Camera mapCamera;
	private GameObject mapMask;
	
	private Vector2 minimapPosition;
	private Vector2 minimapSize;
	private float minimapZoom;
	private Vector2 mapPosition;
	private Vector2 mapSize;
	private float mapZoom;

	void Start() {
		// Set init values
		fullscreen = false;
		toggleFullscreen = false;
		toggling = false;

		// Setup map camera
		mapCamera = gameObject.camera;
		mapCamera.clearFlags = CameraClearFlags.Depth;
		mapMask = GameObject.FindGameObjectWithTag (HUDConstants.TAG_MINIMAP_MASK);
		mapMask.gameObject.SetActive(true);
		
		// Setup map
		mapPosition.x = Screen.width * 0.2f;
		mapPosition.y = Screen.height * 0.1f;
		mapSize.x = Screen.width * 0.6f;
		mapSize.y = Screen.height * 0.8f;
		mapZoom = 106f;

		// Setup minimap
		minimapPosition.x = mapCamera.pixelRect.x;
		minimapPosition.y = mapCamera.pixelRect.y;
		minimapSize.x = mapCamera.pixelRect.width;
		minimapSize.y = mapCamera.pixelRect.height;
		minimapZoom = mapCamera.orthographicSize;

		// Get player
		player = CarHandling.transform;

		var bundle = AssetBundle.CreateFromFile(string.Format("{0}/{1}", System.IO.Directory.GetCurrentDirectory(), "Data/mapData.dat"));
		if (bundle == null) {
			Debug.Log("Settings data not found!");
			return;
		}
		var settingsData = bundle.mainAsset as TextAsset;
		//string s = "name=segment\nlength=100\nwidth=100\nxMin=-50\nxMax=50\nzMin=-50\nzMax=50";
		var mapSettings = new MapSettings(settingsData.text);

		this.mapHandler = new MapHandler(this, bundle, mapSettings, LayerMask.NameToLayer("Map"));
	    // Disable auto minimap for now
        //this.mapHandler.Start(player.position);
	}

	// Optmization, only move camera when needed.
	void Update() {
		// Get the transform from the player. If we have not yet been placed in the player hierarki, return and wait
		// for the next update
		if (player == null) 
			player = CarHandling.transform;
		if (player == null)
			return;

		if (Input.GetButtonDown (HUDConstants.KEY_MAP)) {
			toggleFullscreen = true;
			if (!fullscreen) {
				showMapWindow();
			} else {
				hideMapWindow();
			}
		}
		this.moveCamera (player);
        // Disable auto minimap for now
		if (this.mapHandler == null) {
			return;
		}
		this.timer += Time.deltaTime;
		if (timer > mapCheck) {
			this.mapHandler.UpdateMap(player.position);
			this.timer = 0;
		}
        
	}

	void moveCamera (Transform player)
	{
		if (!fullscreen) {
			newPosition = new Vector3 (player.position.x, transform.position.y, player.position.z);
			newMaskPosition = new Vector3 (player.position.x, mask.transform.position.y, player.position.z);
			newRotation = new Vector3 (this.transform.eulerAngles.x, player.transform.eulerAngles.y, this.transform.eulerAngles.z);
		}
		playerIcon.transform.position = new Vector3 (player.position.x, playerIcon.position.y, player.position.z);
		playerIcon.transform.eulerAngles = new Vector3 (playerIcon.transform.eulerAngles.x, player.eulerAngles.y, playerIcon.transform.eulerAngles.z);
		if (toggleFullscreen) {
			StopCoroutine("toggle");
			StartCoroutine("toggle");
			toggleFullscreen = false;
		} else if (!toggling) {
			mask.transform.position = newMaskPosition;
			this.transform.position = newPosition;
			this.transform.eulerAngles = newRotation;
		}
	}

	private IEnumerator toggle() {
		toggling = true;
		float deltaT = 0;
		while (deltaT < zoomDuration) {
			deltaT += Time.deltaTime;
			yield return true;
			this.transform.position = Vector3.Lerp (this.transform.position, newPosition, deltaT / zoomDuration);
			mask.transform.position = Vector3.Lerp (mask.transform.position, newMaskPosition, deltaT / zoomDuration);
			this.transform.eulerAngles = Vector3.Lerp (this.transform.eulerAngles, newRotation, deltaT / rotationDuration);
		}
		toggling = false;
	}

	public void Unload() {
		this.mapHandler.Unload ();
		this.mapHandler = null;
	}

	public void StartAsyncMethod(IEnumerator method) {
		this.StartCoroutine (method);
	}
	
	private void hideMapWindow() {
		fullscreen = false;
		StopCoroutine("zoomOut");
		StopCoroutine("moveToMap");
		StartCoroutine("zoomIn");
		StartCoroutine("moveToMinimap");
	}
	
	private void showMapWindow() {
		fullscreen = true;
		newPosition = new Vector3 (0f, transform.position.y, 0f);
		newMaskPosition = new Vector3 (0f, mask.transform.position.y, 0f);
		newRotation = new Vector3 (this.transform.eulerAngles.x, 0f, this.transform.eulerAngles.z);
		StopCoroutine("zoomIn");
		StopCoroutine("moveToMinimap");
		StartCoroutine("zoomOut");
		StartCoroutine("moveToMap");
	}
	
	private IEnumerator zoomIn() {
		float deltaT = 0;
		if (mapCamera.orthographicSize <= mapZoom) {
			while ( deltaT < zoomDuration) {
				deltaT += Time.deltaTime;
				yield return true;
				mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, minimapZoom, deltaT / zoomDuration);
				mapMask.transform.localScale = Vector3.Lerp(mapMask.transform.localScale, new Vector3(3000f, mapMask.transform.localScale.y, 3000f), deltaT / zoomDuration);
			}
		}
	}
	
	private IEnumerator zoomOut() {
		float deltaT = 0;
		while (deltaT < zoomDuration) {
			deltaT += Time.deltaTime;
			yield return true;
			mapCamera.orthographicSize = Mathf.Lerp (mapCamera.orthographicSize, mapZoom, deltaT / zoomDuration);
			mapMask.transform.localScale = Vector3.Lerp (mapMask.transform.localScale, new Vector3 (24000f, mapMask.transform.localScale.y, 24000f), deltaT / zoomDuration);
		}
	}
	
	private IEnumerator moveToMap() {
		float deltaT = 0;
		while (deltaT < moveDuration) {
			deltaT += Time.deltaTime;
			yield return true;
			mapCamera.pixelRect = createMinimapLerp(mapPosition.x, mapPosition.y, mapSize.x, mapSize.y, deltaT , moveDuration);
		}
	}
	
	private IEnumerator moveToMinimap() {
		float deltaT = 0;
		while (deltaT < moveDuration) {
			deltaT += Time.deltaTime;
			yield return true;
			mapCamera.pixelRect = createMinimapLerp(minimapPosition.x, minimapPosition.y, minimapSize.x, minimapSize.y, deltaT, moveDuration);
		}
	}

	private Rect createMinimapLerp(float x, float y, float width, float height, float deltaTime, float duration) {
		return new Rect (Mathf.Lerp (mapCamera.pixelRect.x, x, deltaTime / duration), Mathf.Lerp (mapCamera.pixelRect.y, y, deltaTime / duration), Mathf.Lerp (mapCamera.pixelRect.width, width, deltaTime / duration), Mathf.Lerp (mapCamera.pixelRect.height, height, deltaTime / duration));
	}

}
