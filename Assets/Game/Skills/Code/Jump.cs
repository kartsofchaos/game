using UnityEngine;

public class Jump : TriggerSkill
{

	protected override void _trigger ()
	{
		CarRigidBody.AddForce(CarTransform.up * 450000);
	}
}
