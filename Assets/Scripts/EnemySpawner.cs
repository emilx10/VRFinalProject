using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnInterval = 1.5f;

    [Header("Lanes Settings")]
    [SerializeField]
    private Vector3[] lanes = new Vector3[]
    {
        new Vector3(-2f, 0f, 5f),
        new Vector3(0f, 0f, 5f),
        new Vector3(2f, 0f, 5f)
    };

    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private float gizmoRadius = 0.3f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnInterval);
    }

    void Spawn()
    {
        if (lanes.Length == 0) return;

        Vector3 laneOffset = lanes[Random.Range(0, lanes.Length)];
        Vector3 spawnPos = player.position + player.forward * laneOffset.z + player.right * laneOffset.x;
        spawnPos.y = laneOffset.y; // use lane Y if you want vertical offsets

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    // Draw Gizmos in the editor to visualize lanes
    void OnDrawGizmos()
    {
        if (player == null || lanes == null) return;

        Gizmos.color = gizmoColor;
        foreach (Vector3 lane in lanes)
        {
            Vector3 gizmoPos = player.position + (player.forward * lane.z) + (player.right * lane.x);
            gizmoPos.y = lane.y;
            Gizmos.DrawSphere(gizmoPos, gizmoRadius);
        }
    }
}
