using UnityEngine;
using System.Collections;

public class GameModeVirus : MonoBehaviour 
{
    public GameObject[] burningPersons;
    public float TotalRoundTime;
    int index = 0;

    void Start()
    {
        OnSetup();
    }

    public void OnSetup()
    {
        // Take 3 random players and set on FIRE!
    }

    void Update()
    {
        // Get all persons in scene ??
        // Update time for each players
    }

    void isPlayerAlive()
    {
        // How much time is left on the burning effect?
    }

    public void IncreaseTeamScore(Team team)
    {
        if (team == Team.Red)
        {
            // increase red score
        }
        else if (team == Team.Blue)
        {
            // increase blue score
        }
    }

    public bool IsRoundFinished()
    {
        return TotalRoundTime <= 0;
    }
}
