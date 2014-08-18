using UnityEngine;

namespace MyMinimap {
	public class MapSegment {
		public GameObject gameObject { get; set; }
		public SegmentState state { get; set; }

		public void Destroy(bool resetState) {
			if (gameObject != null) {
				GameObject.Destroy(gameObject);
				if(resetState)
					state = SegmentState.Destroyed;
			}
		}
		public void Reset() {
			this.gameObject = null;
			this.state = SegmentState.Destroyed;
		}

		public void Replace(MapSegment segment) {
			if (gameObject != null)
				GameObject.Destroy (gameObject);
			this.gameObject = segment.gameObject;
			this.state = segment.state;
		}

	}

}