using UnityEngine;
using System.Collections;

public class MapDrawer : MonoBehaviour {

    private GameObject textureObject;
    private Transform textureTransform;
    public Texture2D texture;
    public bool navigatable = false;
    public Texture2D nagivationTexture;

    private GameObject cameraMapObject;
    private Camera cameraMap;
    private Transform targetTransform;

    // Use this for initialization
    void Start () {

        cameraMapObject = GameObject.FindGameObjectWithTag(CameraConstants.TAG_MAP_CAMERA);
        cameraMap = cameraMapObject.camera;
        targetTransform = cameraMapObject.GetComponent<MapCamera>().target.transform;

        // Create a cylinder game object and assign it to the map layer
        textureObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        textureObject.name = "MapItem";
        textureObject.layer = LayerMask.NameToLayer(HUDConstants.LAYER_MAP);

        // Transform properties
        textureTransform = textureObject.transform;

        // TODO: Fix player transform
        //textureTransform.parent = gameObject.transform;
        //textureTransform.localScale = new Vector3(4f, 0f, 4f);
        // For now:
        textureTransform.parent = gameObject.GetComponentInChildren<CarHandling>().transform;
        textureTransform.localScale = new Vector3(10f, 0f, 10f);

        textureTransform.localPosition = new Vector3(0f, 1f, 0f);

        // Add texture
        Material mat = textureObject.renderer.material;
        mat.SetTexture("_MainTex", texture);
        mat.shader = Shader.Find("Unlit/Transparent");
        Destroy(textureObject.collider);
    }
    
    // Update is called once per frame
    void Update () {
        if (!navigatable)
            return;

        if (!targetTransform) {
            Debug.LogWarning("Map camera has no target!");
            return;
        }
        
        if (Vector2.Distance(XZ(textureTransform.position), XZ(targetTransform.position)) > camera.orthographicSize)
        {
            Debug.Log("Out of view");
        }
    }

    public Vector2 XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

}
