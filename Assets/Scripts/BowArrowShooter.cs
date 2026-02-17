using UnityEngine;

public class BowArrowShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform arrowSpawnPoint;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform stringGrabPoint; // the pulled empty

    [Header("Bow Settings")]
    [SerializeField] float maxPullZ = -0.4f;   // how far back the string can be pulled
    [SerializeField] float shootForce = 30f;   // base force multiplier

    GameObject currentArrow;

    void Update()
    {
        // Optional: keep arrow snapped while pulling
        if (currentArrow != null)
        {
            currentArrow.transform.position = arrowSpawnPoint.position;
            currentArrow.transform.rotation = arrowSpawnPoint.rotation;
        }
    }

    public void SpawnArrow()
    {
        if (currentArrow != null) return;

        currentArrow = Instantiate(
            arrowPrefab,
            arrowSpawnPoint.position,
            arrowSpawnPoint.rotation
        );

        // Freeze until released
        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void ReleaseArrow()
    {
        if (currentArrow == null) return;

        float pullAmount = GetPull01();

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        // Shoot forward
        rb.linearVelocity = arrowSpawnPoint.forward * (pullAmount * shootForce);

        currentArrow = null;
    }

    float GetPull01()
    {
        float localZ = transform.InverseTransformPoint(stringGrabPoint.position).z;

        return Mathf.Clamp01(Mathf.Abs(localZ / maxPullZ));
    }
}
