using UnityEngine;
using System.Collections;

public class MapCamera : HUDBase, IMapLoader {

    // Transforms
    public Transform target;
	private Transform cameraTransform;
	private Transform minimapMaskTransform;

    // Camera
    private Camera mapCamera;

    // Flags
    private bool fullscreen;
    private bool userWantsToToggle;
    private bool toggling;

    // Snapshots
    private Vector3 snapshotPosition;
    private Vector3 snapshotMaskPosition;
    private Vector3 snapshotRotation;

    // Durations
    private float rotationDuration = 0.3f;
    private float zoomDuration = 0.7f;
    private float moveDuration = 0.5f;

    // Map preferences
    private Vector2 minimapPosition;
    private Vector2 minimapSize;
    private float minimapZoom;
    private Vector2 mapPosition;
    private Vector2 mapSize;
	private float mapZoom;
	private Vector3 mapMaskScale;
	private Vector3 minimapMaskScale;

    // Auto-loading feature
    private MapHandler mapHandler;
    private float mapCheck = 1f;
    private float timer = 0;

    void Start()
    {
        // Set references
        cameraTransform = this.transform;

        // Set init values
        fullscreen = false;
        userWantsToToggle = false;
        toggling = false;

        // Setup fullscreen map preferences
        mapPosition.x = Screen.width * 0.2f;
        mapPosition.y = Screen.height * 0.1f;
        mapSize.x = Screen.width * 0.6f;
        mapSize.y = Screen.height * 0.8f;
        mapZoom = 106f;

		// Setup minimap preferences
		minimapMaskTransform = GameObject.FindGameObjectWithTag(CameraConstants.TAG_MINIMAP_MASK).transform;
        minimapPosition.x = CameraConstants.POSITION_MINIMAP_X;
        minimapPosition.y = CameraConstants.POSITION_MINIMAP_Y;
        minimapSize.x = CameraConstants.SIZE_MINIMAP_WIDTH;
        minimapSize.y = CameraConstants.SIZE_MINIMAP_HEIGHT;
		mapMaskScale = new Vector3(5f, 0f, 5f);
		minimapMaskScale = new Vector3(1f, 0f, 1f);
        minimapZoom = 30f;

        // Setup map camera
        mapCamera = gameObject.GetComponent<Camera>();
        mapCamera.clearFlags = CameraClearFlags.Depth;
        mapCamera.pixelRect = new Rect(minimapPosition.x, minimapPosition.y, minimapSize.x, minimapSize.y);

    }

    void Update() {
        if (!target)
            return;
        
        // Update position and size of camera every frame to adjust to resizing window
        if (!toggling)
        {
            if (fullscreen)
                mapCamera.pixelRect = new Rect(mapPosition.x, mapPosition.y, mapSize.x, mapSize.y);
            else
                mapCamera.pixelRect = new Rect(minimapPosition.x, minimapPosition.y, minimapSize.x, minimapSize.y);
        }

        // Toggle fullscreen map on user input
        if (Input.GetButtonDown(HUDConstants.KEY_MAP))
        {
            toggleFullscreenMap(target);
        }
        this.updateCamera(target);

        // Disable auto minimap for now
        /*if (this.mapHandler == null) {
            return;
        }
        this.timer += Time.deltaTime;
        if (timer > mapCheck) {
            this.mapHandler.UpdateMap(player.position);
            this.timer = 0;
        }*/
        
    }

    void updateCamera(Transform t)
    {
        Vector3 tmp;

        // Minimap snapshot used as reference during toggle (the fullscreen 
        // snapshot are known and do not need to be uptades every frame)
        if (!fullscreen) {
            // Camera and mask position
            tmp = t.position;
            tmp.y = cameraTransform.position.y;
            snapshotPosition = tmp;

            // Camera rotation
            tmp = cameraTransform.eulerAngles;
            tmp.y = t.transform.eulerAngles.y;
            snapshotRotation = tmp;
        }

        // Update transform if not in middle of a toggle
        if (!toggling) {
            cameraTransform.position = snapshotPosition;
            cameraTransform.eulerAngles = snapshotRotation;
        }
    }

    public void setTarget(Transform target)
    {
        this.target = target;

        var bundle = AssetBundle.LoadFromFile(string.Format("{0}/{1}", System.IO.Directory.GetCurrentDirectory(), HUDConstants.PATH_MAP_BUNDLE));
        if (bundle == null)
        {
            Debug.Log("Settings data not found!");
            return;
        }
        var settingsData = bundle.mainAsset as TextAsset;
        var mapSettings = new MapSettings(settingsData.text);

        this.mapHandler = new MapHandler(this, bundle, mapSettings, LayerMask.NameToLayer(HUDConstants.LAYER_MAP));
        this.mapHandler.Start(target.position);
    }

    public void Unload() {
        this.mapHandler.Unload();
        this.mapHandler = null;
    }

    public void StartAsyncMethod(IEnumerator method) {
        this.StartCoroutine (method);
    }

    private void toggleFullscreenMap(Transform t)
    {
        // Lerp player position
        StopCoroutine("CameraSmoothTransition");
        StartCoroutine("CameraSmoothTransition");
        if (fullscreen)
        {
            // Lerp camera
            StopCoroutine("CameraZoomFullscrenMap");
            StopCoroutine("CameraMoveMap");
            StartCoroutine("CameraZoomMinimap");
            StartCoroutine("CameraMoveMinimap");
        }
        else
        {
            Vector3 tmp;
            // Fullscreen map snapshot used as reference during toggle

            // Camera and mask position
            tmp = cameraTransform.transform.position;
            tmp.x = 0f;
            tmp.z = 0f;
            snapshotPosition = tmp;

            // Camera rotation
            tmp = cameraTransform.eulerAngles;
            tmp.y = 0f;
            snapshotRotation = tmp;

            // Lerp camera
            StopCoroutine("CameraZoomMinimap");
            StopCoroutine("CameraMoveMinimap");
            StartCoroutine("CameraZoomFullscrenMap");
            StartCoroutine("CameraMoveMap");
        }
        fullscreen = !fullscreen;

    }

    private IEnumerator CameraSmoothTransition()
    {
        toggling = true;
        float deltaT = 0;
        while (deltaT < zoomDuration)
        {
            deltaT += Time.deltaTime;
            yield return true;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, snapshotPosition, deltaT / zoomDuration);
            cameraTransform.eulerAngles = Vector3.Lerp(cameraTransform.eulerAngles, snapshotRotation, deltaT / rotationDuration);
        }
        toggling = false;
    }

    private IEnumerator CameraZoomMinimap() {
        float deltaT = 0;
        if (mapCamera.orthographicSize <= mapZoom) {
            while ( deltaT < zoomDuration) {
                deltaT += Time.deltaTime;
                yield return true;
                mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, minimapZoom, deltaT / zoomDuration);
				minimapMaskTransform.localScale = Vector3.Lerp(minimapMaskTransform.localScale, minimapMaskScale, deltaT / zoomDuration);
            }
        }
    }

    private IEnumerator CameraZoomFullscrenMap()
    {
        float deltaT = 0;
        while (deltaT < zoomDuration) {
            deltaT += Time.deltaTime;
            yield return true;
			mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, mapZoom, deltaT / zoomDuration);
			minimapMaskTransform.localScale = Vector3.Lerp(minimapMaskTransform.localScale, mapMaskScale, deltaT / zoomDuration);
        }
    }
    
    private IEnumerator CameraMoveMap() {
        float deltaT = 0;
        while (deltaT < moveDuration) {
            deltaT += Time.deltaTime;
            yield return true;
            mapCamera.pixelRect = createMinimapLerp(mapPosition.x, mapPosition.y, mapSize.x, mapSize.y, deltaT , moveDuration);
        }
    }

    private IEnumerator CameraMoveMinimap()
    {
        float deltaT = 0;
        while (deltaT < moveDuration) {
            deltaT += Time.deltaTime;
            yield return true;
            mapCamera.pixelRect = createMinimapLerp(minimapPosition.x, minimapPosition.y, minimapSize.x, minimapSize.y, deltaT, moveDuration);
        }
    }

    private Rect createMinimapLerp(float x, float y, float width, float height, float deltaTime, float duration) {
        return new Rect(Mathf.Lerp(mapCamera.pixelRect.x, x, deltaTime / duration), Mathf.Lerp(mapCamera.pixelRect.y, y, deltaTime / duration), Mathf.Lerp(mapCamera.pixelRect.width, width, deltaTime / duration), Mathf.Lerp(mapCamera.pixelRect.height, height, deltaTime / duration));
    }

}
