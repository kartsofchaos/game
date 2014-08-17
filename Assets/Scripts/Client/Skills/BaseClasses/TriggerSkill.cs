using UnityEngine;
using System.Collections;

public abstract class TriggerSkill : Skill 
{
    /// <summary>
    /// Wrapper function around the abstract activate function to make sure that all skills follow the cooldown
    /// </summary>w1
    private void trigger()
    {
        Debug.Log("Trigger called, isready: " + base.isReady);
        if (base.isReady)
        {
            Debug.Log("trigger ready");
            _trigger();
            applyCooldown();
        }
    }

    void Start()
    {
        if (Player.IsLocalPlayer)
        {
            base.Start();
        }
    }

    void Update()
    {
        if (Player.IsLocalPlayer)
        {
            base.Update();
        }
    }

    protected override void ButtonPressed()
    {
        if (Player.IsLocalPlayer)
        {

        }
    }

    protected override void ButtonReleased()
    {
        if (Player.IsLocalPlayer)
        {
            trigger();
        }
    }

    protected abstract void _trigger();
}
