using System.Collections;
using UnityEngine;

public class GlassRaycaster : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayDistance = 50f;
    [SerializeField] private float rayRadius = 0.3f;
    [SerializeField] private LayerMask glassLayer;

    [Header("Gun Settings")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireInterval = 0.2f; //  NEW

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f; //  NEW

    private void Start()
    {
        currentAmmo = magazineSize;
    }

    public void ShootRay()
    {
        //  Can't shoot while reloading
        if (isReloading)
            return;

        //  Fire rate limit
        if (Time.time < nextFireTime)
            return;

        //  No ammo  start reload
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        //  Set next allowed fire time
        nextFireTime = Time.time + fireInterval;

        //  Consume ammo
        currentAmmo--;

        RaycastHit hit;
        Vector3 origin = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;
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
            ShootableNote note = hit.collider.GetComponent<ShootableNote>();

            if (note != null)
            {
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

        Debug.Log($"Ammo: {currentAmmo}/{magazineSize}");
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Reload Complete!");
    }

    private void OnDrawGizmos()
    {
        if (rayOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * rayDistance);
    }
}