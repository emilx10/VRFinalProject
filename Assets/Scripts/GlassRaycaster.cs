using UnityEngine;

public class GlassRaycaster : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayDistance = 50f;
    [SerializeField] private float rayRadius = 0.3f;
    [SerializeField] private LayerMask glassLayer;

    public void ShootRay()
    {
        RaycastHit hit;
        Vector3 origin = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;
        Vector3 halfExtents = new Vector3(rayRadius, rayRadius, 0.01f);

        // Using BoxCast to make it easier to hit the notes
        if (Physics.BoxCast(
                origin,
                halfExtents,
                direction,
                out hit,
                rayOrigin.rotation,
                rayDistance,
                glassLayer,
                QueryTriggerInteraction.Collide))
        {
            // Get the component
            ShootableNote note = hit.collider.GetComponent<ShootableNote>();

            if (note != null)
            {
                // TryHit handles the success logic and returns true/false
                bool hitSuccess = note.TryHit();

                if (hitSuccess)
                {
                    Debug.Log("<Color=Green>Rhythm Hit!</Color>");
                }
                else
                {
                    Debug.Log("<Color=Yellow>Too Early!</Color>");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (rayOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * rayDistance);
    }
}