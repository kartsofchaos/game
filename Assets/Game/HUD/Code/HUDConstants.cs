using UnityEngine;
using System.Collections;

public class HUDConstants {

	// Tags
	public static string KEY_MAP = "Map";
	public static string TAG_MINIMAP_CAMERA = "MinimapCamera";
	public static string TAG_MINIMAP_MASK = "MinimapMask";

	public static string PATH_HUD = Constants.PATH_GAME + "/HUD";

	// Paths
	public static string PATH_MINIMAP_SETTINGS = PATH_HUD + "/Code/Minimap/MinimapSettings.txt";
	public static string PATH_MINIMAP_MATERIALS = PATH_HUD + "/Materials/Minimap";
	public static string PATH_MINIMAP_MODELS = PATH_HUD + "/Models/Minimap";
	public static string PATH_MINIMAP_PREFABS = PATH_HUD + "/Prefabs/Minimap";
	public static string PATH_MINIMAP_TEXTURES = PATH_HUD + "/Textures/Minimap";

	// Minimap
	public static string MINIMAP_SEGMENT_PATTERN = "{0}-{1}.{2}";

}
