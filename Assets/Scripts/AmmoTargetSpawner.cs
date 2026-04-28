using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoTargetSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject ammoPrefab;

    [Header("Spawn Points (ONLY THESE 2)")]
    [SerializeField] private Transform spawnPointA;
    [SerializeField] private Transform spawnPointB;

    [Header("Pool")]
    [SerializeField] private int poolSize = 5;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float targetLifetime = 3f;

    private List<GameObject> pool = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ammoPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Spawn()
    {
        if (spawnPointA == null || spawnPointB == null) return;

        GameObject obj = GetFromPool();
        if (obj == null) return;

        //  pick one of the two points
        Transform chosenPoint = Random.value < 0.5f ? spawnPointA : spawnPointB;

        obj.transform.position = chosenPoint.position;
        obj.transform.rotation = chosenPoint.rotation; // optional

        obj.SetActive(true);

        StartCoroutine(DisableAfterTime(obj, targetLifetime));
    }

    private IEnumerator DisableAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        if (obj.activeSelf)
            obj.SetActive(false);
    }

    private GameObject GetFromPool()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        return null;
    }
}