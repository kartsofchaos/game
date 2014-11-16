using UnityEngine;
using System.Collections;

public class DisableAudioInEditor : MonoBehaviour {
	void Start () {
		if (BoltNetwork.isServer)
		GetComponent<AudioListener> ().enabled = false;
	}
}
