using UnityEngine;

public class MapSegment {

	public GameObject SegmentGameObject;
	public SegmentState State;

	public void Destroy(bool resetState) {
		if (SegmentGameObject != null) {
			GameObject.Destroy(SegmentGameObject);
			if(resetState)
				State = SegmentState.Destroyed;
		}
	}

	public void Reset() {
		this.SegmentGameObject = null;
		this.State = SegmentState.Destroyed;
	}

	public void Replace(MapSegment segment) {
		if (SegmentGameObject != null)
			GameObject.Destroy (SegmentGameObject);
		this.SegmentGameObject = segment.SegmentGameObject;
		this.State = segment.State;
	}

}
