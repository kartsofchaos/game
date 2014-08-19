using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------------------------------
// Name	:	CarAudio
// Desc  :  This script should be attached to the top level car object.
//          It reads the speed, gear, and rpm from the CarHandling.cs script to 
//			apply carsounds to the scene
// ------------------------------------------------------------------------------------------
public class CarAudio : CarBase
{
	public AudioClip EngineSoundClip = null;	// Select the engine sound clip in the inspector
	private AudioSource EngineSound = null;
	public float enginePitch;
	public float EngineVolume;

	void Awake()
	{
		EngineSound = gameObject.AddComponent<AudioSource> ();
		EngineSound.clip = EngineSoundClip;
		EngineSound.loop = true;
		EngineSound.Play();
		EngineSound.volume = EngineVolume;
	}

	void Update()
	{
		enginePitch = 1.1F + CarHandling.GetCurrentRPM () / 10000F * 2.2F;
		EngineVolume = Mathf.Clamp(0.4F + CarHandling.GetCurrentRPM () / 10000F , 0F, 1F);

		EngineSound.volume = EngineVolume;
		EngineSound.pitch = enginePitch;
	}
}
