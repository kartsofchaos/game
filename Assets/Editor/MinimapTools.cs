using UnityEngine;
using UnityEditor;

using System.Collections;

public class Utils : MonoBehaviour {

	[MenuItem("Minimap/Create Map Planes From Selection")]
	private static void CreateMapPlanesFromSelection() {

		// Get minimap data from file
		var settingsData = AssetDatabase.LoadMainAssetAtPath("Assets/Game/HUD/Code/Minimap/MinimapSettings.txt") as TextAsset;
		var mapSettings = new MapSettings(settingsData.text);

		var length = mapSettings.length;
		var width = mapSettings.width;

		var mesh = CreatePlanesMesh(length, width);

		// Create missing folders
		EditorHelper.CreateAssetFolderIfNotExists("Game/HUD/Models/Minimap");
		EditorHelper.CreateAssetFolderIfNotExists("Game/HUD/Materials/Minimap");
		EditorHelper.CreateAssetFolderIfNotExists("Game/HUD/Prefabs/Minimap");

		// Create minimap plane mesh
		AssetDatabase.CreateAsset(mesh, "Assets/Game/HUD/Models/Minimap/MinimapSegment.mesh");

		// Get all files from selection
		var selection = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
		if (selection != null) {
			for (int i = 0; i < selection.Length; i++) {
				// Parse texture
				var texture = selection[i] as Texture;
				if (texture != null) {
					var mat = CreateMaterial(texture, texture.name);

					// Create minimap plane material
					AssetDatabase.CreateAsset(mat, string.Format("Assets/Game/HUD/Materials/Minimap/{0}.mat", texture.name));

					// Create minimap prefab
					var prefab = PrefabUtility.CreatePrefab(string.Format("Assets/Game/HUD/Prefabs/Minimap/{0}.prefab", texture.name), CreatePrefab(mesh, texture.name));

					// Set prefab properties
					prefab.GetComponent<Renderer>().sharedMaterial = mat;
					prefab.GetComponent<MeshFilter>().sharedMesh = mesh;
					prefab.GetComponent<MeshRenderer>().castShadows = false;
					prefab.GetComponent<MeshRenderer>().receiveShadows = false;

				}
			}
			AssetDatabase.Refresh();
		}

	}

	// Creates prefab
	private static GameObject CreatePrefab(Mesh mesh, string name)
	{
		var gameObject = new GameObject(name);
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.transform.Rotate(new Vector3(1, 0, 0), 90);
		return gameObject;
	}
	
	// Creates material
	private static Material CreateMaterial(Texture texture, string name)
	{
		var material = new Material(Shader.Find("Unlit/Texture"));
		material.mainTexture = texture;
		material.name = name;
		return material;
	}
	
	// Creates a low-ploy plane mesh
	private static Mesh CreatePlanesMesh(int length, int width) {
		var mesh = new Mesh();
		mesh.name = "low-poly-mesh";
		mesh.vertices = new Vector3[6] {
						new Vector3 (0, 0, 0),
						new Vector3 (0, width, 0),
						new Vector3 (length, width),
						new Vector3 (length, width),
						new Vector3 (length, 0, 0),
						new Vector3 (0, 0, 0)
		};
		mesh.uv = new Vector2[6] {
						new Vector2 (0, 0),
						new Vector2 (0, 1),
						new Vector2 (1, 1),
						new Vector2 (1, 1),
						new Vector2 (1, 0),
						new Vector2 (0, 0)
		};
		mesh.triangles = new int[6] {0, 1, 2, 3, 4, 5};
		return mesh;
	}

	[MenuItem("Minimap/Export Selection to Map Asset")]
	private static void SaveSelectionToMapAsset() {
		// Get all files from selection
		var selection = Selection.GetFiltered (typeof(GameObject), SelectionMode.DeepAssets);
		if (selection != null) {
			var path = EditorUtility.SaveFilePanel("Save Asset", "", "New Data", "dat");
			if(!string.IsNullOrEmpty(path)) {
				// Get minimap data from file
				var settingsAsset = (TextAsset) AssetDatabase.LoadAssetAtPath("Assets/Game/HUD/Code/Minimap/MinimapSettings.txt", typeof(TextAsset));
				if(settingsAsset == null) {
					Debug.LogError("Settings file not found");
					return;
				}
				BuildPipeline.BuildAssetBundle(settingsAsset, selection, path, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
				Selection.objects = selection;
			}
		}
	}
}
