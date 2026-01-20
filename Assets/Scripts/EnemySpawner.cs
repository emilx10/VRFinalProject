using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnRadius = 8f;
    public float spawnInterval = 1.5f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnInterval);
    }

    void Spawn()
    {
        Vector3 dir = Random.onUnitSphere;
        dir.y = Mathf.Clamp(dir.y, -0.3f, 0.7f);

        Vector3 pos = player.position + dir.normalized * spawnRadius;
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}