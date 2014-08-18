using UnityEngine;

using System.Collections;

namespace MyMinimap
{
	public class MapHandler
	{
		#region Constants and Fields

		private readonly IMapLoader mapLoader;
		private readonly AssetBundle bundle;
		private readonly MapSettings mapSettings;

		private readonly MapSegment[,] loadedSegments = new MapSegment[2, 2];

		private readonly int mapLayer;

		Vector3 mapOffset;
		Rect mapBounds;

		#endregion

		#region Constructors and Destructors

		public MapHandler (IMapLoader mapLoader, AssetBundle mapAsset, MapSettings mapSettings, int mapLayer) {
			this.mapLoader = mapLoader;
			this.bundle = mapAsset;
			this.mapSettings = mapSettings;
			this.mapLayer = mapLayer;

			this.mapOffset = new Vector3 (mapSettings.length / 2, 200, mapSettings.width / 2);
			this.mapBounds = new Rect ();
		}

		#endregion

		#region Properties

		MapSegment bottomLeft { get { return this.loadedSegments[0, 0]; }}
		MapSegment bottomRight { get { return this.loadedSegments[1, 0]; }}
		MapSegment topLeft { get { return this.loadedSegments[0, 1]; }}
		MapSegment topRight { get { return this.loadedSegments[1, 1]; }}

		#endregion

		#region Public Methods
		
		/// <summary>
		/// Starts the map.
		/// </summary>
		/// <param name="position">Position.</param>
		public void Start(Vector3 position) {
			this.PrepareMapAt (position);
		}

		/// <summary>
		/// Updates the map.
		/// </summary>
		/// <param name="position">Position.</param>
		public void UpdateMap(Vector3 position) {
			this.UpdateMapAt (position);
		}

		/// <summary>
		/// Unload thr map.
		/// </summary>
		public void Unload() {
			this.bundle.Unload (true);
		}

		#endregion

		#region Local Methods

		void PrepareMapAt(Vector3 position) {
            this.mapBounds = this.GetMapBoundsForPosition(position);

			this.loadedSegments [0, 0] = new MapSegment () {gameObject = LoadAndCreateSegmentAt(mapBounds.xMin, mapBounds.yMin), state = SegmentState.Active };
			this.loadedSegments [1, 0] = new MapSegment () {gameObject = LoadAndCreateSegmentAt(mapBounds.xMax, mapBounds.yMin), state = SegmentState.Active };
			this.loadedSegments [0, 1] = new MapSegment () {gameObject = LoadAndCreateSegmentAt(mapBounds.xMin, mapBounds.yMax), state = SegmentState.Active };
			this.loadedSegments [1, 1] = new MapSegment () {gameObject = LoadAndCreateSegmentAt(mapBounds.xMax, mapBounds.yMax), state = SegmentState.Active };
		}

		void UpdateMapAt(Vector3 position) {
			var newMapBounds = this.GetMapBoundsForPosition (position);

			bool changed = false;
			if (newMapBounds.xMin < mapBounds.xMin) {
				this.topRight.Replace(topLeft);
				this.bottomRight.Replace(bottomLeft);
				this.topLeft.Reset();
				this.bottomLeft.Reset();
				changed = true;
			}
			else if(newMapBounds.xMax > mapBounds.xMax) {
				this.topLeft.Replace(topRight);
				this.bottomLeft.Replace(bottomRight);
				this.topRight.Reset();
				this.bottomRight.Reset();
				changed = true;
			}

			if (newMapBounds.yMin < mapBounds.yMin) {
				this.topLeft.Replace(bottomLeft);
				this.topRight.Replace(bottomRight);
				this.bottomLeft.Reset();
				this.bottomRight.Reset();
				changed = true;
			}
			else if(newMapBounds.yMax > mapBounds.yMax) {
				this.bottomLeft.Replace(topLeft);
				this.bottomRight.Replace(topRight);
				this.topLeft.Reset();
				this.topRight.Reset();
				changed = true;
			}

            Debug.Log("CHANGED: " + changed);

			if (changed) {
				this.mapBounds = newMapBounds;
				this.HandleSegmentAt(bottomLeft, mapBounds.xMin, mapBounds.yMin);
				this.HandleSegmentAt(bottomRight, mapBounds.xMax, mapBounds.yMin);
				this.HandleSegmentAt(topLeft, mapBounds.xMin, mapBounds.yMax);
				this.HandleSegmentAt(topRight, mapBounds.xMax, mapBounds.yMax);
			}

		}

		void HandleSegmentAt(MapSegment segment, float x, float z) {
			if (segment.state == SegmentState.Destroyed) {
				this.mapLoader.StartAsyncMethod(WaitUntilSegmentLoadAt(segment, x, z));
			}
		}

		IEnumerator WaitUntilSegmentLoadAt(MapSegment segment, float x, float z) {
			segment.state = SegmentState.Loading;
			var segmentCoordPosition = this.GetSegmentCoorsForPosition (x, z);
			var asyncRequest = this.LoadSegmentAsyncAt ((int)segmentCoordPosition.x, (int)segmentCoordPosition.y);
		
			yield return asyncRequest;

			segment.gameObject = CreateSegmentAt (segmentCoordPosition, asyncRequest.asset as GameObject);
			segment.state = SegmentState.Active;
		}

		Rect GetMapBoundsForPosition(Vector3 position) {
			return this.GetMapBoundsForPosition (position.x, position.z);
		}

		Rect GetMapBoundsForPosition(float x, float z) {
            var currSegmentCoord = this.GetSegmentCoorsForPosition(x, z);

			Rect bounds = new Rect (currSegmentCoord.x, currSegmentCoord.y, mapSettings.length, mapSettings.width);
			if (bounds.xMax > mapSettings.xMax) {
				var val = bounds.xMax - mapSettings.xMax;
				bounds.xMin = this.GetSegmentCoorsForPosition(bounds.xMin - val, z).x;
				bounds.xMax = this.GetSegmentCoorsForPosition(bounds.xMax - val, z).x;
			}
			else if (x < currSegmentCoord.x) {
				bounds.xMin -= mapSettings.length;
				bounds.xMax -= mapSettings.length;
			}
			if (bounds.yMax > mapSettings.zMax) {
				var val = bounds.yMax - mapSettings.zMax;
				bounds.yMin = this.GetSegmentCoorsForPosition(x, bounds.yMin - val).y;
				bounds.yMax = this.GetSegmentCoorsForPosition(x, bounds.yMax - val).y;
			}
			else if (z < currSegmentCoord.y) {
				bounds.yMin -= mapSettings.width;
				bounds.yMax -= mapSettings.width;
			}
			return bounds;
		}

		Vector2 GetSegmentCoorsForPosition(Vector2 position) {
			return this.GetSegmentCoorsForPosition (position.x, position.y);
		}

        Vector2 GetSegmentCoorsForPosition(float x, float z)
        {
            float offX = mapOffset.x, offZ = mapOffset.z;
            if (x < 0)
                offX *= -1;
            if (z < 0)
                offZ *= -1;

            var nX = Mathf.Clamp(x + offX, mapSettings.xMin, mapSettings.xMax);
            var nZ = Mathf.Clamp(z + offZ, mapSettings.xMin, mapSettings.xMax);

			var pX = (int) (nX / mapSettings.length);
            var pZ = (int) (nZ / mapSettings.width);

			var aX = pX * mapSettings.length;
            var aZ = pZ * mapSettings.width;

			return new Vector2 (aX, aZ);
		}

		GameObject CreateSegmentAt(Vector2 coord, GameObject segment) {
			var go = MonoBehaviour.Instantiate (segment) as GameObject;

			go.transform.position = new Vector3 (coord.x - mapOffset.x, mapOffset.y, coord.y - mapOffset.z);
			go.layer = mapLayer;

			return go;
		}

		AssetBundleRequest LoadSegmentAsyncAt(int x, int z) {
			return this.bundle.LoadAsync (string.Format ("{0}-{1}.{2}", x, z, mapSettings.segmentName), typeof(GameObject));
		}

		GameObject LoadAndCreateSegmentAt(float x, float z) {
			var segmentCoord = this.GetSegmentCoorsForPosition (x, z);
			var segment = this.LoadSegmentAt ((int)segmentCoord.x, (int)segmentCoord.y);

			var go = GameObject.Instantiate (segment) as GameObject;
			go.transform.position = new Vector3 (x - mapOffset.x, mapOffset.y, z - mapOffset.z);
			go.layer = mapLayer;

			return go;
		}

		GameObject LoadSegmentAt (int x, int z)
		{
			return bundle.Load (string.Format ("{0}-{1}.{2}", x, z, mapSettings.segmentName), typeof(GameObject)) as GameObject;
		}

		#endregion
	}
}

