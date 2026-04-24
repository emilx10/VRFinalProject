using System;

[Serializable]
public class Note
{
    public int X;
    public int Y;
    public long TimeMs;

    public Note(int x, int y, long timeMs)
    {
        X = x;
        Y = y;
        TimeMs = timeMs;
    }
}