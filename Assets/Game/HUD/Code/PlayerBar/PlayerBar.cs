using UnityEngine;
using System.Collections;

public class PlayerBar : HUDBase
{
    // Gameobject and transform of the playerbar and the target player
    private GameObject playerBarObject;
    public Transform playerBarTransform;

    // Set of variables used to display the text in the player bar
	private GUIText playerName;
	public Font TextFont;
	public int FontSize = 20;

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
        setup();
        instantiateHealthBar();
	}

    /// <summary>
    /// Instantiates the gameobject and puts it in the cars hierarchy.
    /// Sets up the GUIText
    /// </summary>
    private void setup()
    {
        playerBarObject = new GameObject("PlayerBar");
        playerBarTransform = playerBarObject.transform;
        playerBarTransform.parent = Player.transform;       // CHANGE TO HUD AS PARENT

        playerName = (GUIText)playerBarObject.AddComponent(typeof(GUIText));
        playerName.text = Player.Name;
        playerName.font = TextFont;
        playerName.fontSize = FontSize;
    }

    private void instantiateHealthBar()
    {
        GameObject healthBarObject = (GameObject)Instantiate(Resources.Load("HUD/HealthBars/HealthBarFriendly", typeof(GameObject)));
        healthBarObject.transform.parent = playerBarTransform;
    }
    
    /// <summary>
    /// Updates the position of the playerbar to move with the player and checks if the player is visible on the screen,
    /// if not disable its playerbar.
    /// </summary>
    private void setPositionAndVisibility()
    {
        playerBarTransform.position = Camera.WorldToViewportPoint(CarTransform.position + heightOffset);
        if (playerBarTransform.position.z < 0)
            playerBarObject.SetActive(false);
        else
            playerBarObject.SetActive(true);
    }

	void OnGUI()
	{
        setPositionAndVisibility();
        HealthBar.Redraw(Player.Health);
	}
}
