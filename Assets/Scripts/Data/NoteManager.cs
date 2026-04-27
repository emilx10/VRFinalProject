using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    [Header("Map & Audio")]
    [SerializeField] private MapSettings map;
    [SerializeField] private AudioSource music;

    [Header("Positions")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform hitPoint;

    [Header("Settings")]
    [SerializeField] private float noteSpeed = 15f;
    [SerializeField] private float spawnZMin = -5f;
    [SerializeField] private float spawnZMax = 5f;

    private List<Note> notes;
    private double startDsp;
    private bool spawningStarted;

    private void Start()
    {
        StartCoroutine(Begin());
    }

    private IEnumerator Begin()
    {
        if (spawningStarted) yield break;
        spawningStarted = true;

        notes = map.GetParsedNotes();
        NotePool.Instance.PreloadNotes(notes.Count);

        yield return new WaitForSeconds(1f);

        // Calculate travel time so notes hit the 'hitPoint' on the beat
        float travelTime = Mathf.Abs(spawnPoint.position.x - hitPoint.position.x) / noteSpeed;
        
        startDsp = AudioSettings.dspTime + 0.5;
        music.clip = map.Music;
        music.PlayScheduled(startDsp);

        int i = 0;
        while (i < notes.Count)
        {
            double noteTimeInMusic = notes[i].TimeMs / 1000.0;
            double spawnDspTime = startDsp + noteTimeInMusic - travelTime;

            if (AudioSettings.dspTime >= spawnDspTime)
            {
                SpawnNote();
                i++;
            }
            yield return null;
        }
    }

    private void SpawnNote()
    {
        GameObject obj = NotePool.Instance.GetNote();

        // Randomize Z axis here
        float randomZ = Random.Range(spawnZMin, spawnZMax);

        obj.transform.position = new Vector3(
            spawnPoint.position.x,
            spawnPoint.position.y,
            randomZ
        );

        // Pass the target X and speed to the note
        obj.GetComponent<ShootableNote>().Init(noteSpeed, hitPoint.position.x);
    }
}