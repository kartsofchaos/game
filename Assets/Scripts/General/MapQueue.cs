using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class has several helper functions to deal with the map queue
/// </summary>
public class MapQueue
{
    /// <summary>
    /// Converts a map queue into a string which can be shared by Photon
    /// </summary>
    /// <param name="mapQueue">The map queue.</param>
    /// <returns>
    /// A string representation of the map queue that can be shared and turned back into a list by other clients
    /// </returns>
    public static string ListToString(List<MapQueueEntry> mapQueue)
    {
        //Create an array of strings that each represent one entry of the map queue
        string[] mapSegments = new string[mapQueue.Count];

        //Convert each entry into its string representation
        for (int i = 0; i < mapQueue.Count; ++i)
        {
            mapSegments[i] = EntryToString(mapQueue[i]);
        }

        //Glue everything together into one big string
        return string.Join("~", mapSegments);
    }

    /// <summary>
    /// Convert a map queue string back into a list
    /// </summary>
    /// <param name="mapQueueString">The map queue string.</param>
    /// <returns></returns>
    public static List<MapQueueEntry> StringToList(string mapQueueString)
    {
        List<MapQueueEntry> mapQueue = new List<MapQueueEntry>();

        //First, split the big string into it's entry segments
        string[] mapSegments = mapQueueString.Split('~');

        for (int i = 0; i < mapSegments.Length; ++i)
        {
            //And then convert each segment into a map queue entry
            MapQueueEntry newQueueEntry = StringToEntry(mapSegments[i]);

            mapQueue.Add(newQueueEntry);
        }

        return mapQueue;
    }

    /// <summary>
    /// Convert a single map queue entry string to a MapQueueEntry object
    /// </summary>
    /// <param name="mapQueueEntryString">The map queue entry string.</param>
    /// <returns></returns>
    public static MapQueueEntry StringToEntry(string mapQueueEntryString)
    {
        string[] mapData = mapQueueEntryString.Split('#');

        MapQueueEntry queueEntry = new MapQueueEntry
        {
            Name = mapData[0],
            Mode = (Gamemode)(int.Parse(mapData[1]))
        };

        return queueEntry;
    }

    /// <summary>
    /// Convert a single MapQueueEntry to a string representation
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <returns></returns>
    public static string EntryToString(MapQueueEntry entry)
    {
        return entry.Name + "#" + (int)entry.Mode;
    }

    /// <summary>
    /// Gets the length of the current map queue.
    /// </summary>
    /// <returns>Number of entries in the current map queue</returns>
    public static int GetCurrentMapQueueLength()
    {
        if (PhotonNetwork.room == null)
        {
            Debug.LogError("Can't get current map if player hasn't joined a room yet.");
            return 0;
        }

        if (PhotonNetwork.room.customProperties == null)
        {
            Debug.LogError("Current room doesn't have custom properties. Can't find current map.");
            return 0;
        }

        if (PhotonNetwork.room.customProperties.ContainsKey(RoomProperty.MapQueue) == false)
        {
            Debug.LogError("Couldn't find map queue in room properties.");
            return 0;
        }

        string mapQueueString = (string)PhotonNetwork.room.customProperties[RoomProperty.MapQueue];
        string[] mapSegments = mapQueueString.Split('~');

        return mapSegments.Length;
    }

    /// <summary>
    /// Gets a single entry in a map queue string.
    /// </summary>
    /// <param name="mapQueueString">The map queue string.</param>
    /// <param name="mapIndex">Index of the entry we want to retrieve.</param>
    /// <returns></returns>
    public static MapQueueEntry GetSingleEntryInMapQueue(string mapQueueString, int mapIndex)
    {
        string[] mapSegments = mapQueueString.Split('~');

        return StringToEntry(mapSegments[mapIndex % mapSegments.Length]);
    }

    /// <summary>
    /// Gets the map queue entry currently running in the joined room
    /// </summary>
    /// <returns></returns>
    public static MapQueueEntry GetCurrentMap()
    {
        return GetRoomMap(0);
    }

    /// <summary>
    /// Gets the map queue entry that is next in the joined room
    /// </summary>
    /// <returns></returns>
    public static MapQueueEntry GetNextMap()
    {
        return GetRoomMap(1);
    }

    /// <summary>
    /// Gets a map queue entry of the currently joined room
    /// </summary>
    /// <param name="mapIndexOffset">The offset of the map index we want to get, relative to the currently played one</param>
    /// <returns></returns>
    static MapQueueEntry GetRoomMap(int mapIndexOffset)
    {
        if (PhotonNetwork.room == null)
        {
            Debug.LogError("Can't get current map if player hasn't joined a room yet.");
            return MapQueueEntry.None;
        }

        if (PhotonNetwork.room.customProperties == null)
        {
            Debug.LogError("Current room doesn't have custom properties. Can't find current map.");
            return MapQueueEntry.None;
        }

        if (PhotonNetwork.room.customProperties.ContainsKey(RoomProperty.MapQueue) == false)
        {
            Debug.LogError("Couldn't find map queue in room properties.");
            return MapQueueEntry.None;
        }

        if (PhotonNetwork.room.customProperties.ContainsKey(RoomProperty.MapIndex) == false)
        {
            Debug.LogError("Couldn't find map index in room properties.");
            return MapQueueEntry.None;
        }

        string mapQueueString = (string)PhotonNetwork.room.customProperties[RoomProperty.MapQueue];
        int mapIndex = (int)PhotonNetwork.room.customProperties[RoomProperty.MapIndex] + mapIndexOffset;

        return MapQueue.GetSingleEntryInMapQueue(mapQueueString, mapIndex);
    }
}
