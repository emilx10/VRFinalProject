using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoTargetSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject ammoPrefab;

    [Header("Reference Cube")]
    [SerializeField] private Transform referenceCube;

    [Header("Pool")]
    [SerializeField] private int poolSize = 5;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float targetLifetime = 3f;

    [Header("Z Range (LOCAL SPACE)")]
    [SerializeField] private float minZ = -5.9f;
    [SerializeField] private float maxZ = 4.9f;

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
        if (referenceCube == null) return;

        GameObject obj = GetFromPool();
        if (obj == null) return;

        //  random Z between your values
        float randomZ = Random.Range(minZ, maxZ);

        //  build position in LOCAL cube space
        Vector3 localOffset = new Vector3(0f, 0f, randomZ);

        //  convert to world space using cube transform
        Vector3 spawnPos =
            referenceCube.position +
            referenceCube.TransformDirection(localOffset);

        obj.transform.position = spawnPos;
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