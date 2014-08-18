using UnityEngine;
using System.Collections;

public class PlayerBar : HUDBase
{
	private GameObject PlayerBarObject;
	private Transform PlayerBarTransform;

	private GUIText myName;
	public Font TextFont;
	public int FontSize = 20;
	GUIStyle TextStyle;

	private Transform target;
	public Vector3 heightOffset = Vector3.up;		// Put it to one unit above the player for now
	public bool clampToScreen = false;			// if true the label will be visible even when the player it out of the frame
	public float clampBorderSize = 0.05f;		// How much viewport space to leave at the borders when label is clamped

	private Camera cam;
	private Transform camTransform;

    void Awake()
    {
        // Disable playerbar for our own car
        if (Player.IsLocalPlayer)
            this.enabled = false;
    }

	void Start()
	{
		PlayerBarObject = new GameObject("PlayerBar");
		PlayerBarTransform = PlayerBarObject.transform;
        myName = (GUIText)PlayerBarObject.AddComponent(typeof(GUIText));
        myName.text = Player.Name;
        myName.font = TextFont;
        myName.fontSize = FontSize;

		cam = Camera.main;
		camTransform = cam.transform;

		// our target is the transform of the car
		target = transform.GetComponentInChildren<CarHandling> ().transform;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	
	void OnGUI()
	{
		PlayerBarTransform.position = cam.WorldToViewportPoint (target.position + heightOffset);
		if (PlayerBarTransform.position.z < 0)
			PlayerBarObject.SetActive (false);
		else
			PlayerBarObject.SetActive (true);
	}
}
