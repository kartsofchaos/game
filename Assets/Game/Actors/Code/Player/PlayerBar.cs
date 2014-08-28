using UnityEngine;
using System.Collections;

public class PlayerBar : PlayerBase
{
    // Gameobject and transform of the playerbar and the target player
    private GameObject playerBarObject;
    public Transform playerBarTransform;

    // Set of variables used to display the text in the player bar
	private GUIText playerName;
	public Font TextFont;
	public int FontSize = 20;

	// Set of variables used by the healthbar
	public Texture2D healthBarBackground;
	public Texture2D healthBarSegment;

    // Screen placement variables for the player bar
	public Vector3 heightOffset = Vector3.up;		// Put it to one unit above the player for now
	public bool clampToScreen = false;			// if true the label will be visible even when the player it out of the frame
	public float clampBorderSize = 0.05f;		// How much viewport space to leave at the borders when label is clamped

    void Awake()
    {
        // Disable playerbar for our own car (maybe swap with some sort of destroy so we dont waste any calculations on this)
       // if (Player.IsLocalPlayer)
    //        this.enabled = false;
    }

	void Start()
	{
		playerBarObject = new GameObject("PlayerBar");
		playerBarTransform = playerBarObject.transform;
		playerBarTransform.parent = CarHandling.transform;       // CHANGE TO HUD AS PARENT

        setupName();
		loadTextures();
	}

    /// <summary>
	/// Sets up the GUIText for the name
    /// </summary>
	private void setupName()
    {
        playerName = (GUIText)playerBarObject.AddComponent(typeof(GUIText));
        playerName.text = Player.Name;
        playerName.font = TextFont;
        playerName.fontSize = FontSize;
		playerBarTransform.position = Camera.WorldToViewportPoint(playerBarTransform.position);
    }

	/// <summary>
	/// Loads the textures for the healthbar and player icon
	/// </summary>
	private void loadTextures()
	{
		healthBarBackground = Resources.Load ("Textures/HealthBarBackgroundTransparent", typeof(Texture2D)) as Texture2D;
		healthBarSegment = Resources.Load ("Textures/HealthBarSegmentGreen", typeof(Texture2D)) as Texture2D;
	}
    
    /// <summary>
    /// Updates the position of the playerbar to move with the player and checks if the player is visible on the screen,
    /// if not disable its playerbar.
    /// </summary>
    private void setPositionAndVisibility()
    {
        if (playerBarTransform.position.z < 0)
            playerBarObject.SetActive(false);
        else
            playerBarObject.SetActive(true);
    }

	public void DrawHealthBar()
	{
		Vector3 position = Camera.WorldToViewportPoint(playerBarTransform.position);
		GUI.DrawTexture(new Rect(position.x, position.y, healthBarBackground.width, healthBarBackground.height), healthBarBackground);
		
		for (int i = 0; i < (int)Player.Health; i++)
		{
			GUI.DrawTexture(new Rect(position.x + i * healthBarSegment.width, position.y, healthBarSegment.width, healthBarSegment.height), healthBarSegment);
		}
	}

	void OnGUI()
	{
        setPositionAndVisibility();
		DrawHealthBar ();
	}
}
