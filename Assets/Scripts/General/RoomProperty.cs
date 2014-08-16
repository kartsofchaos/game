using UnityEngine;
using System.Collections;

/// <summary>
/// Photons Custom Room properties are named in strings. 
/// By defining them as constants here, I'm making sure that I won't have any spelling errors when using them in different parts of the project
/// </summary>
public class RoomProperty
{
    public const string RedScore = "RS";
    public const string BlueScore = "BS";
    public const string StartTime = "ST";
    public const string MapQueue = "MQ";
    public const string MapIndex = "MI";
    public const string Map = "C0";
    public const string Mode = "C1";
    public const string SkillLevel = "C2";
}