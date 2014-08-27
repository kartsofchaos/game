using UnityEngine;
using System.Collections;

public class MapDrawer : MonoBehaviour {

    private GameObject textureObject;
    private Transform textureTransform;
    public Texture2D texture;
    public bool navigatable = false;
    public Texture2D nagivationTexture;

    // Use this for initialization
    void Start () {

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
    
    }

}
