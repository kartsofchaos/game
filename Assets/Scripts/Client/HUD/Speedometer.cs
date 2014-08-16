using UnityEngine;
using System.Collections;

public class Speedometer : HUDBase
{
	public Texture2D speedOmeter;
	public Texture2D pointer;
	public float zeroAngle = -60f;
	public float topSpeedAngle = 240f;

	void OnGUI()
	{
		int padding = 20;
		int speedOMeterX = Screen.width - speedOmeter.width - padding;
		int speedOMeterY = Screen.height - speedOmeter.height - padding;
		GUI.DrawTexture( new Rect(speedOMeterX, speedOMeterY, speedOmeter.width, speedOmeter.height), speedOmeter);

		float topSpeed = Mathf.Abs(CarHandling.GetTopSpeed());
		float speed = Mathf.Abs(CarHandling.GetCurrentKPH());

		if (speed < 0.5)
			speed = 0;
		
		float rpm = Mathf.Abs(CarHandling.GetCurrentRPM());
		if (rpm < 0.5)
			rpm = 0;

		Vector2 centre= new Vector2(Screen.width - (speedOmeter.width/2) - padding, Screen.height - (speedOmeter.height/2) - padding);

		float speedFraction = speed / topSpeed;
		float needleAngle= Mathf.Lerp(zeroAngle, topSpeedAngle, speedFraction);
		
		GUIUtility.RotateAroundPivot(needleAngle, centre);
		GUI.DrawTexture( new Rect(speedOMeterX, speedOMeterY, pointer.width, pointer.height), pointer);

	}

}
