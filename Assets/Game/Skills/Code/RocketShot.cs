using UnityEngine;

public class RocketShot : AimSkill
{
	public GameObject rocketPrefab;
    public Transform rocketSpawn;
    public float fireRate;
    private float nextFire;
	
	protected override void _aim ()
	{

	}

    protected override void _stopAiming()
    {

    }

	protected override void _fire ()
	{

	}

    void Update()
    {

        if (Input.GetButton(SkillConstants.KEY_SKILL_TWO) && Time.time > nextFire) 
        {
            nextFire = Time.time + fireRate;
            Instantiate(rocketPrefab, rocketSpawn.position, rocketSpawn.rotation);
        }
    }
}
