using System.Collections;
using UnityEngine;

public class GlassRaycaster : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayDistance = 50f;
    [SerializeField] private float rayRadius = 0.05f;
    [SerializeField] private LayerMask hitLayer; // include BOTH glass + ammo

    [Header("Gun Settings")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireInterval = 0.2f;

    [Header("Visual")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform hitMarker; // optional small sphere / dot

    [SerializeField] GameObject ghostObject;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;

    private void Start()
    {
        currentAmmo = magazineSize;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
        }
    }

    private void Update()
    {
        DrawAim(); // Always show where the gun is aiming
    }

    public void ShootRay()
    {
        if (isReloading)
            return;

        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireInterval;

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
                hitLayer,
                QueryTriggerInteraction.Collide))
        {
            Debug.Log("Hit: " + hit.collider.name);
            // PRIORITY: Ammo target
            AmmoTarget ammoTarget = hit.collider.GetComponent<AmmoTarget>();
            if (ammoTarget != null)
            {
                ammoTarget.Hit(this);
                Debug.Log("<Color=Cyan>Ammo Target Hit!</Color>");
                return;
            }

            // Only consume ammo if NOT ammo target
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

            currentAmmo--;
            Destroy(ghostObject);

            ShootableNote note = hit.collider.GetComponentInParent<ShootableNote>();

            if (note != null)
            {
                bool hitSuccess = note.TryHit();

                if (hitSuccess)
                    Debug.Log("<Color=Green>Rhythm Hit!</Color>");
                else
                    Debug.Log("<Color=Yellow>Too Early!</Color>");
            }
        }

        Debug.Log($"Ammo: {currentAmmo}/{magazineSize}");
    }

    //  VISUAL AIM SYSTEM
    private void DrawAim()
    {
        if (rayOrigin == null || lineRenderer == null)
            return;

        Vector3 origin = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;
        Vector3 halfExtents = new Vector3(rayRadius, rayRadius, 0.01f);

        RaycastHit hit;
        Vector3 endPoint;

        if (Physics.BoxCast(
                origin,
                halfExtents,
                direction,
                out hit,
                rayOrigin.rotation,
                rayDistance,
                hitLayer,
                QueryTriggerInteraction.Collide))
        {
            endPoint = hit.point;

            // Move hit marker
            if (hitMarker != null)
            {
                hitMarker.position = hit.point;
                hitMarker.gameObject.SetActive(true);
            }

            // Optional color feedback
            AmmoTarget ammo = hit.collider.GetComponent<AmmoTarget>();
            ShootableNote note = hit.collider.GetComponent<ShootableNote>();

            if (ammo != null)
                lineRenderer.material.color = Color.cyan;
            else if (note != null)
                lineRenderer.material.color = Color.green;
            else
                lineRenderer.material.color = Color.red;
        }
        else
        {
            endPoint = origin + direction * rayDistance;

            if (hitMarker != null)
                hitMarker.gameObject.SetActive(false);

            lineRenderer.material.color = Color.red;
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);
    }

    //  ADD AMMO
    public void AddAmmo(int amount)
    {
        if (isReloading)
            isReloading = false; // cancel reload

        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, magazineSize);

        Debug.Log($"<Color=Cyan>Picked Ammo! {currentAmmo}/{magazineSize}</Color>");
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

    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;
}