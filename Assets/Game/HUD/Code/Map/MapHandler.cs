using UnityEngine;

using System.Collections;

public class MapHandler
{
    private readonly IMapLoader mapLoader;
    private readonly AssetBundle bundle;
    private readonly MapSettings mapSettings;

    private readonly MapSegment[,] loadedSegments = new MapSegment[3, 3];
    private readonly int mapLayer;

    private Vector3 mapOffset;
    private Rect mapBounds;

    public MapHandler(IMapLoader mapLoader, AssetBundle mapAsset, MapSettings mapSettings, int mapLayer)
    {
        this.mapLoader = mapLoader;
        this.bundle = mapAsset;
        this.mapSettings = mapSettings;
        this.mapLayer = mapLayer;

        this.mapOffset = new Vector3(mapSettings.length / 2, HUDConstants.MAP_HEIGHT, mapSettings.width / 2);
        this.mapBounds = new Rect();
    }

    /// <summary>
    /// Starts the map.
    /// </summary>
    /// <param name="position">Position.</param>
    public void Start(Vector3 position)
    {
        Debug.Log("xMin: " + mapSettings.xMin);
        Debug.Log("xMax: " + mapSettings.xMax);
        Debug.Log("zMin: " + mapSettings.zMin);
        Debug.Log("zMax: " + mapSettings.zMax);
        this.PrepareMapAt(position);
    }

    /// <summary>
    /// Updates the map.
    /// </summary>
    /// <param name="position">Position.</param>
    public void UpdateMap(Vector3 position)
    {
        this.UpdateMapAt(position);
    }

    /// <summary>
    /// Unload thr map.
    /// </summary>
    public void Unload()
    {
        this.bundle.Unload(true);
    }

    // Properties
    MapSegment BottomLeft { get { return this.loadedSegments[0, 0]; } }
    MapSegment Left { get { return this.loadedSegments[0, 1]; } }
    MapSegment TopLeft { get { return this.loadedSegments[0, 2]; } }
    MapSegment Top { get { return this.loadedSegments[1, 2]; } }
    MapSegment TopRight { get { return this.loadedSegments[2, 2]; } }
    MapSegment Right { get { return this.loadedSegments[2, 1]; } }
    MapSegment BottomRight { get { return this.loadedSegments[2, 0]; } }
    MapSegment Bottom { get { return this.loadedSegments[1, 0]; } }
    MapSegment Center { get { return this.loadedSegments[1, 1]; } }


    void PrepareMapAt(Vector3 position)
    {
        // Use static map for now
        for (int x = mapSettings.xMin; x <= mapSettings.xMax; x += mapSettings.length)
        {
            for (int z = mapSettings.zMin; z <= mapSettings.zMax; z += mapSettings.width)
            {
                var segment = this.LoadSegmentAt((int)x, (int)z);
                var go = GameObject.Instantiate(segment) as GameObject;
                go.transform.position = new Vector3(x - mapOffset.x, mapOffset.y, z - mapOffset.z);
                go.layer = mapLayer;
            }

        }

        /*
        this.mapBounds = this.GetMapBoundsForPosition(position);
        Debug.Log("Current map bounds: " + mapBounds.xMin + "," + mapBounds.xMax + "," + mapBounds.xMax + "," + mapBounds.yMax + ")");
        this.loadedSegments[0, 0] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMin, mapBounds.yMin), State = SegmentState.Active };
        this.loadedSegments[0, 1] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMin, mapBounds.yMin+mapSettings.width), State = SegmentState.Active };
        this.loadedSegments[0, 2] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMin, mapBounds.yMax), State = SegmentState.Active };
        this.loadedSegments[1, 2] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMin+mapSettings.length, mapBounds.yMax), State = SegmentState.Active };
        this.loadedSegments[2, 2] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMax, mapBounds.yMax), State = SegmentState.Active };
        this.loadedSegments[2, 1] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMax, mapBounds.yMin+mapSettings.width), State = SegmentState.Active };
        this.loadedSegments[2, 0] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMax, mapBounds.yMin), State = SegmentState.Active };
        this.loadedSegments[1, 0] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMin+mapSettings.length, mapBounds.yMin), State = SegmentState.Active };
        this.loadedSegments[1, 1] = new MapSegment() {SegmentGameObject = LoadAndCreateSegmentAt(mapBounds.xMin+mapSettings.length, mapBounds.yMin+mapSettings.width), State = SegmentState.Active };
        */
    }

    // Updates the map depending of the position in the world, 
    // changing segments if position is outside current segments
    void UpdateMapAt(Vector3 position)
    {
        var newMapBounds = this.GetMapBoundsForPosition(position);
        Debug.Log("Current map bounds: " + mapBounds.xMin + "," + mapBounds.xMax + "," + mapBounds.xMax + "," + mapBounds.yMax + ")");
        Debug.Log("New map bounds: " + newMapBounds.xMin + "," + newMapBounds.yMin + "," + newMapBounds.xMax + "," + newMapBounds.yMax + ")");

        bool changed = false;
        // West
        if (newMapBounds.xMin < mapBounds.xMin)
        {
            Debug.Log("Going west!");
            if (newMapBounds.yMax <= mapSettings.zMax)
            {

                Top.Replace(TopRight);
                Center.Replace(Right);
                Bottom.Replace(BottomRight);

                TopLeft.Replace(Top);
                Left.Replace(Center);
                BottomLeft.Replace(Bottom);
                TopRight.Reset();
                Right.Reset();
                BottomRight.Reset();
            }
            changed = true;
        }
        // East
        else if (newMapBounds.xMax > mapBounds.xMax)
        {
            Debug.Log("Going east!");
            if (newMapBounds.xMin >= mapSettings.xMin)
            {
                Top.Replace(TopLeft);
                Center.Replace(Left);
                Bottom.Replace(BottomLeft);

                TopRight.Replace(Top);
                Right.Replace(Center);
                BottomRight.Replace(Bottom);

                TopLeft.Reset();
                Left.Reset();
                BottomLeft.Reset();

            }
            changed = true;
        }

        // South
        if (newMapBounds.yMin < mapBounds.yMin)
        {
            Debug.Log("Going south!");
            if (newMapBounds.yMin >= mapSettings.zMin)
            {
                Left.Replace(BottomLeft);
                Center.Replace(Bottom);
                Right.Replace(BottomRight);

                TopLeft.Replace(Left);
                Top.Replace(Center);
                TopRight.Replace(Right);

                BottomLeft.Reset();
                Bottom.Reset();
                BottomRight.Reset();

            }
            changed = true;
        }
        // North
        else if (newMapBounds.yMax > mapBounds.yMax)
        {
            Debug.Log("Going north!");
            if (newMapBounds.yMax <= mapSettings.zMax)
            {

                Left.Replace(TopLeft);
                Center.Replace(Top);
                Right.Replace(TopRight);

                BottomLeft.Replace(Left);
                Bottom.Replace(Center);
                BottomRight.Replace(Right);

                TopLeft.Reset();
                Top.Reset();
                TopRight.Reset();
            }

            changed = true;
        }

        if (changed)
        {
            this.mapBounds = newMapBounds;
            this.HandleSegmentAt(BottomLeft, mapBounds.xMin, mapBounds.yMin);
            this.HandleSegmentAt(Left, mapBounds.xMin, mapBounds.yMin + mapSettings.width);
            this.HandleSegmentAt(TopLeft, mapBounds.xMin, mapBounds.yMax);
            this.HandleSegmentAt(Top, mapBounds.xMin + mapSettings.length, mapBounds.yMax);
            this.HandleSegmentAt(TopRight, mapBounds.xMax, mapBounds.yMax);
            this.HandleSegmentAt(Right, mapBounds.xMax, mapBounds.yMin + mapSettings.width);
            this.HandleSegmentAt(BottomRight, mapBounds.xMax, mapBounds.yMin);
            this.HandleSegmentAt(Bottom, mapBounds.xMin + mapSettings.length, mapBounds.yMin);
            this.HandleSegmentAt(Center, mapBounds.xMin + mapSettings.length, mapBounds.yMin + mapSettings.width);
        }

    }

    void HandleSegmentAt(MapSegment segment, float x, float z)
    {
        if (segment.State == SegmentState.Destroyed)
        {
            this.mapLoader.StartAsyncMethod(WaitUntilSegmentLoadAt(segment, x, z));
        }
    }

    IEnumerator WaitUntilSegmentLoadAt(MapSegment segment, float x, float z)
    {
        var segmentCoordPosition = this.GetSegmentCoorsForPosition(x, z);
        if (segmentCoordPosition.x > mapSettings.xMax || segmentCoordPosition.x < mapSettings.xMin ||
            segmentCoordPosition.y > mapSettings.zMax || segmentCoordPosition.y < mapSettings.zMin)
        {
            yield break;
        }
        segment.State = SegmentState.Loading;
        var asyncRequest = this.LoadSegmentAsyncAt((int)segmentCoordPosition.x, (int)segmentCoordPosition.y);

        yield return asyncRequest;

        segment.SegmentGameObject = CreateSegmentAt(segmentCoordPosition, asyncRequest.asset as GameObject);
        segment.State = SegmentState.Active;
    }

    Rect GetMapBoundsForPosition(Vector3 position)
    {
        return this.GetMapBoundsForPosition(position.x, position.z);
    }

    // Gets a corner coordinates from the "first" corner of a segment
    Rect GetMapBoundsForPosition(float x, float z)
    {
        // Get center segment
        var segmentCoord = this.GetSegmentCoorsForPosition(x, z);
        //Debug.Log("Center segment, center coord: " + segmentCoord);

        // Create a starting point with coords and offsets
        Rect bounds = new Rect();
        bounds.xMin = segmentCoord.x - mapSettings.length;
        bounds.xMax = segmentCoord.x + mapSettings.length;
        bounds.yMin = segmentCoord.y - mapSettings.width;
        bounds.yMax = segmentCoord.y + mapSettings.width;
        //Debug.Log("Theorectial bounds: " + bounds);
        /*
        bounds.xMin = Mathf.Max(mapSettings.xMin, bounds.xMin);
        bounds.yMin = Mathf.Max(mapSettings.zMin, bounds.yMin);
        bounds.xMax = Mathf.Min(mapSettings.xMax, bounds.xMax);
        bounds.yMax = Mathf.Min(mapSettings.zMax, bounds.yMax);*/
        //Debug.Log("Bounds: (" + bounds.xMin + "," + bounds.xMax + "," + bounds.yMin + "," + bounds.yMax + ")");
        return bounds;
    }

    Vector2 GetSegmentCoorsForPosition(Vector2 position)
    {
        return this.GetSegmentCoorsForPosition(position.x, position.y);
    }

    // Gets the "left-bottom" corner-coordinate of the closets segment
    public Vector2 GetSegmentCoorsForPosition(float x, float z)
    {
        bool xNeg = false;
        bool zNeg = false;

        //Debug.Log("GetSegmentCoorsForPosition: (" + x + "," + z + ")");

        if (x < 0)
        {
            x *= -1;
            xNeg = true;
        }
        if (z < 0)
        {
            z *= -1;
            zNeg = true;
        }

        //Debug.Log("GetSegmentCoorsForPosition: (" + x + "," + z + ")");

        // Get bottom-left coord
        var segmentX = (int)(x / mapSettings.length);
        var segmentZ = (int)(z / mapSettings.width);

        // Add half of length to reach center
        var segCoordX = segmentX * mapSettings.length + mapOffset.x;
        var segCoordZ = segmentZ * mapSettings.width + mapOffset.z;
        //Debug.Log("New-GetSegmentCoorsForPosition: (" + segCoordX + "," + segCoordZ + ")");
        if (xNeg)
            segCoordX *= -1;
        if (zNeg)
            segCoordZ *= -1;
        return new Vector2(segCoordX, segCoordZ);
    }

    GameObject CreateSegmentAt(Vector2 coord, GameObject segment)
    {
        var go = MonoBehaviour.Instantiate(segment) as GameObject;
        go.transform.position = new Vector3(coord.x - mapOffset.x, mapOffset.y, coord.y - mapOffset.z);
        go.layer = mapLayer;
        return go;
    }

    AssetBundleRequest LoadSegmentAsyncAt(int x, int z)
    {
        return this.bundle.LoadAsync(string.Format(HUDConstants.MAP_SEGMENT_PATTERN, x, z, mapSettings.segmentName), typeof(GameObject));
    }

    GameObject LoadAndCreateSegmentAt(float x, float z)
    {
        if (x > mapSettings.xMax || x < mapSettings.xMin ||
            z > mapSettings.zMax || z < mapSettings.zMin)
            return null;
        Debug.Log("Load: (" + x + "," + z + ")");
        var segment = this.LoadSegmentAt((int)x, (int)z);
        var go = GameObject.Instantiate(segment) as GameObject;
        go.transform.position = new Vector3(x - mapOffset.x, mapOffset.y, z - mapOffset.z);
        go.layer = mapLayer;
        return go;
    }

    GameObject LoadSegmentAt(int x, int z)
    {
        return bundle.Load(string.Format(HUDConstants.MAP_SEGMENT_PATTERN, x, z, mapSettings.segmentName), typeof(GameObject)) as GameObject;
    }

}
