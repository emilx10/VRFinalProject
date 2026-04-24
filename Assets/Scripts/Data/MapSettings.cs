using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Logic,
}


[CreateAssetMenu(fileName = "MapSettings", menuName = "Scriptable Objects/MapSettings")]
public class MapSettings : ScriptableObject
{
    [Header("Metadata")]
    public string MapCreatorName;
    public string MusicName;
    public string MusicCreatorName;

    public Difficulty Difficulty;

    public List<string> Genres;

    public AudioClip Music;

    public long Offset;

    [Header("Map Data")]
    [TextArea]
    public string MapData;

    private List<Note> cachedNotes;

    /// <summary>
    /// Parses the MapData string into a list of Note objects.
    /// Caches the result to avoid unnecessary parsing.
    /// </summary>
    /// 
    public List<Note> GetParsedNotes()
    {
        if (cachedNotes == null || cachedNotes.Count == 0)
        {
            cachedNotes = ParseMapData(MapData);
        }
        return cachedNotes;
    }

    private List<Note> ParseMapData(string mapData)
    {
        List<Note> parsedNotes = new List<Note>();
        if (string.IsNullOrEmpty(mapData)) return parsedNotes;

        string[] noteEntries = mapData.Split(',');

        foreach (string entry in noteEntries)
        {
            string[] parts = entry.Split('|');
            if (parts.Length == 3 &&
                int.TryParse(parts[0], out int x) &&
                int.TryParse(parts[1], out int y) &&
                long.TryParse(parts[2], out long timeMs))
            {
                parsedNotes.Add(new Note(x, 2 - y, timeMs));
            }
        }

        return parsedNotes;
    }
}