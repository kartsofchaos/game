using UnityEngine;

public class Jump : TriggerSkill
{

	protected override void _trigger ()
	{
        Debug.Log("JUMP AROUND");
		CarRigidBody.AddForce(CarTransform.up * 450000);
	}
}
