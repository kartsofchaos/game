using UnityEngine;
using System.Collections;

public class HUDConstants {

    // Keys
    public static string KEY_MAP = "Map";

    // Layers
    public static string LAYER_MAP = "Map";

    // Paths
    public static string PATH_HUD = Constants.PATH_GAME + "/HUD";
    public static string PATH_MAP_BUNDLE = "Data/mapData.dat";
    public static string PATH_MAP_SETTINGS = PATH_HUD + "/Code/Map/MapSettings.txt";
    public static string PATH_MAP_MATERIALS = PATH_HUD + "/Resources/Materials/Map";
	public static string PATH_MAP_MODELS = PATH_HUD + "/Resources/Models/Map";
	public static string PATH_MAP_PREFABS = PATH_HUD + "/Resources/Prefabs/Map";
	public static string PATH_MAP_TEXTURES = PATH_HUD + "/Resources/Textures/Map";

    // Map
    public static string MAP_SEGMENT_NAME = "MapSegment.mesh";
    public static string MAP_SEGMENT_PATTERN = "{0}-{1}.{2}";
    public static float MAP_HEIGHT = 0;

}
