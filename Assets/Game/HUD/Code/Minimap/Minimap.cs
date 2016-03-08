using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {

	//For placing the image of the mini map.
	public GUIStyle minimap;
	//Two transform variables, one for the player's and the enemy's, 
	public Transform player;
	//Icon images for the player and enemy(s) on the map. 
	public GUIStyle playerIcon;
	public GUIStyle enemyIcon;
	//Offset variables (X and Y) - where you want to place your map on screen.
	private float mapOffSetX;
	private float mapOffSetY;
	//The width and height of your map as it'll appear on screen,
	private float mapWidth;
	private float mapHeight;
	//Width and Height of your scene, or the resolution of your terrain.
	private float sceneWidth;
	private float sceneHeight;
	//The size of your player's and enemy's icon on the map. 
	private int iconSize;

	public int scaleFactor = 4;
	private int iconHalfSize;

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.FindWithTag("Terrain");
		Terrain terrain = go.GetComponent<Terrain>();
		Vector3 terrainSize = terrain.terrainData.size;
		sceneWidth = terrainSize.z;
		sceneHeight = terrainSize.x;
		Debug.Log("("+sceneWidth+", "+sceneHeight+")");
		mapWidth = minimap.normal.background.width/scaleFactor;
		mapHeight = minimap.normal.background.height/scaleFactor;
		iconSize = playerIcon.normal.background.height/scaleFactor;
		mapOffSetX = 0f;
		mapOffSetY = Screen.height - mapHeight;
	}
	
	// Update is called once per frame
	void Update () { //So that the pivot point of the icon is at the middle of the image.
		//You'll know what it means later...
		iconHalfSize = iconSize/2;
	}

	float GetMapPos(float pos, float mapSize, float sceneSize) {
		return pos * mapSize/sceneSize;
	}

	void OnGUI() {
		GUI.BeginGroup(new Rect(mapOffSetX, mapOffSetY, mapWidth, mapHeight), minimap);
		var pX = GetMapPos(transform.position.x+100, mapWidth, sceneWidth);
		var pZ = GetMapPos(-1*transform.position.z+100, mapHeight, sceneHeight);
		var playerMapX = pX - iconHalfSize;
		var playerMapZ = ((pZ * -1) - iconHalfSize) + mapHeight;
		GUI.Box(new Rect(playerMapZ, playerMapX, iconSize, iconSize), "", playerIcon);
		GUI.EndGroup();
	}
}
