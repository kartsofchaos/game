using UnityEngine;
using System.Collections;

public class MapDrawer : MonoBehaviour {

	// Map object
	private GameObject _gameObject;
    private Transform _transform;
	private Transform _parentTransform;
	public Texture2D mainTexture;

	// Camera
	private GameObject cameraMapObject;
	private MapCamera mapCamera;
    private Camera cameraMap;

	// Navigation
	public bool navigatable = false;
	public Texture2D navigationTexture;
	private float minimapViewRange;
	private Transform targetTransform;
	private bool lastSeenInsideViewRange = true;

    // Use this for initialization
    void Start () {

        cameraMapObject = GameObject.FindGameObjectWithTag(CameraConstants.TAG_MAP_CAMERA);
        cameraMap = cameraMapObject.camera;
		mapCamera = cameraMapObject.GetComponent<MapCamera>();

        // Create a cylinder game object and assign it to the map layer
        _gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _gameObject.name = "MapItem";
        _gameObject.layer = LayerMask.NameToLayer(HUDConstants.LAYER_MAP);

        // Transform properties
		_transform = _gameObject.transform;
		_transform.parent = gameObject.transform;
		_parentTransform = _transform.parent.transform;

        _transform.localScale = new Vector3(5f, 0f, 5f);
		_transform.localPosition = Vector3.up;
		_transform.localEulerAngles = Vector3.zero;

        // Add texture
        Material mat = _gameObject.renderer.material;
        mat.SetTexture("_MainTex", mainTexture);
        mat.shader = Shader.Find("Unlit/Transparent");
        Destroy(_gameObject.collider);
    }
    
    // Update is called once per frame
    void OnGUI () {

		// Only for navigatable items
		if (!navigatable)
			return;

		if (!targetTransform) {
			if (mapCamera.target != null)
				targetTransform = mapCamera.target.transform;
			return;
		}

		Vector3 heading = targetTransform.position - _parentTransform.position;
		heading.y = 0;
		float distance = Mathf.Abs(heading.magnitude);

		minimapViewRange = cameraMap.orthographicSize;
		// Render a navigation object if outside view range
		if (distance > minimapViewRange)
		{
			// If still outside view range, do not update texture unnecessarily
			if (lastSeenInsideViewRange)
				_gameObject.renderer.material.SetTexture("_MainTex", navigationTexture);
			lastSeenInsideViewRange = false;
			Vector3 direction = heading / distance;
			_transform.position = new Vector3(targetTransform.position.x - direction.x * (minimapViewRange - HUDConstants.MINIMAP_BOUND_PADDING), 1f, targetTransform.position.z - direction.z * (minimapViewRange - HUDConstants.MINIMAP_BOUND_PADDING));
			_transform.LookAt(_parentTransform);
		}
		// If still within view range, do not update texture
		else
		{
            if (!lastSeenInsideViewRange)
                _gameObject.renderer.material.SetTexture("_MainTex", mainTexture);
			lastSeenInsideViewRange = true;
            _transform.position = new Vector3(_transform.position.x, 1f, _transform.position.z);
			_transform.eulerAngles = Vector3.zero;
		}
    }	
}
