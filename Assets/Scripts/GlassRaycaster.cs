using UnityEngine;

public class GlassRaycaster : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] Transform rayOrigin;
    [SerializeField] float rayDistance = 50f;
    [SerializeField] float rayRadius = 0.2f;
    [SerializeField] LayerMask glassLayer;

    public void ShootRay()
    {
        RaycastHit hit;

        Vector3 origin = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;

        // Half size of the box (X = width, Y = height, Z doesn't matter much)
        Vector3 halfExtents = new Vector3(rayRadius, rayRadius, 0.01f);

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
            Debug.Log("Hit glass: " + hit.collider.name);
            hit.collider.SendMessage("OnGlassHit", SendMessageOptions.DontRequireReceiver);

            Destroy(hit.collider.gameObject);
        }
    }


    // Always draw in Scene view
    private void OnDrawGizmos()
    {
        if (rayOrigin == null) return;

        Gizmos.color = Color.yellow;

        Vector3 origin = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;
        Vector3 halfExtents = new Vector3(rayRadius, rayRadius, 0.01f);

        Vector3 end = origin + direction * rayDistance;

        // Draw start box
        Matrix4x4 startMatrix = Matrix4x4.TRS(origin, rayOrigin.rotation, Vector3.one);
        Gizmos.matrix = startMatrix;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);

        // Draw end box
        Matrix4x4 endMatrix = Matrix4x4.TRS(end, rayOrigin.rotation, Vector3.one);
        Gizmos.matrix = endMatrix;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);

        Gizmos.matrix = Matrix4x4.identity;

        // Draw connecting edges (beam body)
        DrawBoxCastBody(origin, end, rayOrigin.rotation, halfExtents);
    }

    void DrawBoxCastBody(Vector3 start, Vector3 end, Quaternion rotation, Vector3 halfExtents)
    {
        Vector3 right = rotation * Vector3.right * halfExtents.x;
        Vector3 up = rotation * Vector3.up * halfExtents.y;

        // 4 corners at start
        Vector3 s1 = start + right + up;
        Vector3 s2 = start + right - up;
        Vector3 s3 = start - right + up;
        Vector3 s4 = start - right - up;

        // 4 corners at end
        Vector3 e1 = end + right + up;
        Vector3 e2 = end + right - up;
        Vector3 e3 = end - right + up;
        Vector3 e4 = end - right - up;

        Gizmos.DrawLine(s1, e1);
        Gizmos.DrawLine(s2, e2);
        Gizmos.DrawLine(s3, e3);
        Gizmos.DrawLine(s4, e4);
    }

}
