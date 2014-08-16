using UnityEngine;
using System.Collections;

/// <summary>
/// The map queue is defined as a list of this struct
/// It simply holds the name of a level and a gamemode
/// </summary>
[System.Serializable]
public struct MapQueueEntry
{
    public string Name;
    public Gamemode Mode;

    public static MapQueueEntry None
    {
        get
        {
            return new MapQueueEntry { Name = "", Mode = Gamemode.Count };
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is MapQueueEntry)
        {
            MapQueueEntry otherEntry = (MapQueueEntry)obj;

            return Name == otherEntry.Name && Mode == otherEntry.Mode;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode() + Mode.GetHashCode();
    }

    public override string ToString()
    {
        return Name + " (" + Mode.ToString() + ")";
    }
}