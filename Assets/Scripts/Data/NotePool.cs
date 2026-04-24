using UnityEngine;
using System.Collections.Generic;

public class NotePool : MonoBehaviour
{
    public static NotePool Instance;

    public GameObject notePrefab;
    public GameObject noteFolder;

    private Queue<GameObject> notePool = new Queue<GameObject>();
    //private List<Color> playerColors = new List<Color>();

    private void Awake()
    {
        Instance = this;
    }

    public void PreloadNotes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject note = Instantiate(notePrefab, noteFolder.transform);

            note.SetActive(false);
            notePool.Enqueue(note);
        }
    }

    public GameObject GetNote()
    {
        if (notePool.Count > 0)
        {
            GameObject note = notePool.Dequeue();
            note.SetActive(true);
            return note;
        }
        else
        {
            return Instantiate(notePrefab, noteFolder.transform);
        }
    }

    public void ReturnNote(GameObject note)
    {
        note.SetActive(false);
        notePool.Enqueue(note);
    }
}