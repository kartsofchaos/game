using UnityEngine;
using System.Collections;

public abstract class AimSkill : Skill {

    private bool isAiming;

	// Use this for initialization
	void Start ()
    {
        isAiming = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        listenForFire();
	}

    /// <summary>
    /// Wrapper function around the abstract aim function to make sure that all skills follow the cooldown
    /// </summary>
    private void aim()
    {
        if (isReady)
        {
            isAiming = true;
            _aim();
        }
    }

    private void stopAiming()
    {
        isAiming = false;
        _stopAiming();
    }

    private void listenForFire()
    {
        if (isAiming && Input.GetButtonUp(Constants.KEY_MAIN_FIRE))
        {
            applyCooldown();
            stopAiming();
            _fire();
        }
    }

    protected override void ButtonPressed()
    {
        aim();
    }

    protected override void ButtonReleased()
    {
        stopAiming();
    }

    protected abstract void _aim();
    protected abstract void _stopAiming();
    protected abstract void _fire();
}
