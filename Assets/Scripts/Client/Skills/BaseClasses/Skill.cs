using UnityEngine;
using System.Collections;


public abstract class Skill : SkillBase
{
	public float cooldown = 2F;			// The time it takes the skill to recharge

    public Button Button;

    private float _currentCooldown;
    public float CurrentCooldown
    {
        get
        {
            return _currentCooldown;
        }
    }

    private bool _isReady;
    public bool isReady
    {
        get
        {
            return _isReady;
        }
    }

    //TODO:
    // Add icon to draw in the HUD

    protected void Start()
    {
        _isReady = true;
        _currentCooldown = 0.0f;
    }

    /// <summary>
    /// Each update we check if our button has been pressed
    /// </summary>
    protected void Update()
    {
        // Check if the button is being pressed OBS: DO THIS BETTER .TOSTRING IS EXPENSIVE!!!!!!!!!!!!!!!
        if (Input.GetButtonDown(Button.ToString()))
        {
            Debug.Log("BUTTON DOWN");
            ButtonPressed();
        }
        else if (Input.GetButtonUp(Button.ToString()))
        {
            Debug.Log("BUTTON UP");
            ButtonReleased();
        }
    }

    /// <summary>
    /// Starts thte coroutine that performs the cooldown
    /// </summary>
    protected void applyCooldown()
    {
        Debug.Log("APPLYING CD");
        _isReady = false;
        _currentCooldown = cooldown;
        StartCoroutine(countdown());
    }

    /// <summary>
    /// Counts down until the cooldown has passed
    /// </summary>
    /// <returns></returns>
    private IEnumerator countdown()
    {
        while (_currentCooldown > 0.0F)
        {
            Debug.Log("_currentCoolDown = " + _currentCooldown);
            _currentCooldown -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("After While");
        _isReady = true;
    }

    protected abstract void ButtonPressed();
    protected abstract void ButtonReleased();
}
